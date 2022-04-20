using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TicketManagement.Web.Clients;
using TicketManagement.Web.Infrastructure.Extensions;
using TicketManagement.Web.Infrastructure.Identity;
using TicketManagement.Web.Models;

namespace TicketManagement.Web.Controllers
{
    /// <summary>
    /// Represents actions for shopping.
    /// </summary>
    [Authorize(Roles = Roles.User)]
    public class ShoppingController : Controller
    {
        private readonly IStringLocalizer<ShoppingController> _localizer;
        private readonly IUserClient _userClient;
        private readonly IEventManagerClient _eventClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShoppingController"/> class.
        /// </summary>
        /// <param name="localizer">Localizer.</param>
        /// <param name="userClient">User client.</param>
        /// <param name="eventClient">Event client.</param>
        public ShoppingController(IStringLocalizer<ShoppingController> localizer, IUserClient userClient,
            IEventManagerClient eventClient)
        {
            _localizer = localizer;
            _userClient = userClient;
            _eventClient = eventClient;
        }

        /// <summary>
        /// Gets user's cart.
        /// </summary>
        /// <param name="returnUrl">Url to return from cart.</param>
        /// <returns>Cart view.</returns>
        public ActionResult Cart(string returnUrl)
        {
            var cart = HttpContext.Session.GetJson<CartViewModel>("cart") ?? new CartViewModel();
            cart.ReturnUrl = returnUrl ?? "/";
            return View(cart);
        }

        /// <summary>
        /// Add ticket to user's cart.
        /// </summary>
        /// <param name="seatId">Seat id.</param>
        /// <returns>Cart view if operation was successful otherwise Error view.</returns>
        [HttpPost]
        public async Task<ActionResult> Cart(int seatId)
        {
            var cart = HttpContext.Session.GetJson<CartViewModel>("cart") ?? new CartViewModel();
            if (cart.Tickets.Any(t => t.EventSeatId == seatId))
            {
                return View(cart);
            }

            var seatModel = await _eventClient.GetEventSeat(seatId);
            var areaModel = await _eventClient.GetEventArea(seatModel?.EventAreaId ?? 0);
            var eventModel = await _eventClient.Get(areaModel?.EventId ?? 0);
            if (eventModel != null && areaModel != null && seatModel != null)
            {
                AddTicket(eventModel, areaModel, seatModel);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Pays for tickets from user balance.
        /// </summary>
        /// <returns>Cart view with validation messages.</returns>
        public async Task<ActionResult> Pay()
        {
            var cart = HttpContext.Session.GetJson<CartViewModel>("cart") ?? new CartViewModel();
            if (cart.Tickets.Count != 0)
            {
                var totalPrice = cart.Tickets.Sum(t => t.Price);
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUser = await _userClient.Get(currentUserId);
                if (currentUser != null && currentUser.Balance >= totalPrice)
                {
                    foreach (var ticket in cart.Tickets)
                    {
                        var eventSeat = await _eventClient.GetEventSeat(ticket.EventSeatId);
                        eventSeat.State = SeatStates.Booked;
                        eventSeat.UserId = ticket.UserId;
                        await _eventClient.Put(eventSeat);
                    }

                    currentUser.Balance -= totalPrice;
                    await _userClient.Put(currentUser);
                    HttpContext.Session.Remove("cart");
                    cart = new CartViewModel();
                    ViewData["SuccessMessage"] = _localizer["Successfully paid"];
                }
                else
                {
                    ModelState.AddModelError("", _localizer["You don't have enough money"]);
                }
            }
            else
            {
                ModelState.AddModelError("", _localizer["Your cart is empty"]);
            }

            return View(nameof(Cart), cart);
        }

        /// <summary>
        /// Deletes ticket form user's cart.
        /// </summary>
        /// <param name="seatId">Seat id.</param>
        /// <returns>Cart view.</returns>
        public ActionResult DeleteInEvent(int seatId)
        {
            var cart = HttpContext.Session.GetJson<CartViewModel>("cart") ?? new CartViewModel();
            cart.Tickets.RemoveAll(t => t.EventSeatId == seatId);
            HttpContext.Session.SetJson("cart", cart);
            return Ok();
        }

        /// <summary>
        /// Deletes ticket form user's cart.
        /// </summary>
        /// <param name="seatId">Seat id.</param>
        /// <returns>Cart view.</returns>
        public ActionResult DeleteInCart(int seatId)
        {
            var cart = HttpContext.Session.GetJson<CartViewModel>("cart") ?? new CartViewModel();
            cart.Tickets.RemoveAll(t => t.EventSeatId == seatId);
            HttpContext.Session.SetJson("cart", cart);
            return RedirectToAction("Cart");
        }

        private void AddTicket(EventViewModel eventModel, EventAreaViewModel areaModel, EventSeatViewModel seatModel)
        {
            var cart = HttpContext.Session.GetJson<CartViewModel>("cart") ?? new CartViewModel();
            var ticketModel = new TicketViewModel
            {
                EventName = eventModel.Name,
                EventStart = eventModel.Start,
                Price = areaModel.Price,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                EventSeatId = seatModel.Id,
                Row = seatModel.Number,
                Number = seatModel.Row,
            };
            cart.Tickets.Add(ticketModel);
            HttpContext.Session.SetJson("cart", cart);
        }
    }
}
