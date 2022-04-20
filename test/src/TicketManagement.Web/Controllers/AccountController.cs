using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.Web.Clients;
using TicketManagement.Web.Infrastructure.Filters;
using TicketManagement.Web.Infrastructure.Identity;
using TicketManagement.Web.JwtTokenAuth;
using TicketManagement.Web.Models;

namespace TicketManagement.Web.Controllers
{
    /// <summary>
    /// Represents actions for working with user accounts.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly IUserClient _userClient;
        private readonly IRolesClient _rolesClient;
        private readonly ILoginClient _loginClient;
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly IEventService _eventService;
        private readonly IEventAreaService _eventAreaService;
        private readonly IEventSeatService _eventSeatService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="userClient">User client.</param>
        /// <param name="rolesClient">Roles client.</param>
        /// <param name="loginClient">Login client.</param>
        /// <param name="localizer">Localizer.</param>
        /// <param name="eventService">Event service.</param>
        /// <param name="eventAreaService">Event area service.</param>
        /// <param name="eventSeatService">Event seat service.</param>
        public AccountController(IUserClient userClient, IRolesClient rolesClient, ILoginClient loginClient, IStringLocalizer<AccountController> localizer,
            IEventService eventService, IEventAreaService eventAreaService, IEventSeatService eventSeatService)
        {
            _userClient = userClient;
            _rolesClient = rolesClient;
            _loginClient = loginClient;
            _localizer = localizer;
            _eventService = eventService;
            _eventAreaService = eventAreaService;
            _eventSeatService = eventSeatService;
        }

        /// <summary>
        /// Gets a user balance.
        /// </summary>
        /// <returns>User balance view.</returns>
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult> Balance()
        {
            var currentUser = await GetCurrentUser();
            return View(currentUser.Balance);
        }

        /// <summary>
        /// Gets user's purchase history.
        /// </summary>
        /// <returns>Purchase history view.</returns>
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult> PurchaseHistory()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var seats = _eventSeatService.GetEventSeats().Where(s => s.UserId == currentUserId);
            var tickets = new List<TicketViewModel>();
            foreach (var seatDto in seats)
            {
                var areaDto = await _eventAreaService.GetEventAreaAsync(seatDto?.EventAreaId ?? 0);
                var eventDto = await _eventService.GetEventAsync(areaDto?.EventId ?? 0);
                if (eventDto != null && areaDto != null && seatDto != null)
                {
                    tickets.Add(new TicketViewModel
                    {
                        EventName = eventDto.Name,
                        EventStart = eventDto.Start,
                        Price = areaDto.Price,
                        UserId = currentUserId,
                        EventSeatId = seatDto.Id,
                        Row = seatDto.Number,
                        Number = seatDto.Row,
                    });
                }
            }

            return View(tickets);
        }

        /// <summary>
        /// Gets the view for replenishing an user balance.
        /// </summary>
        /// <returns>View for replenishing user balance.</returns>
        [Authorize(Roles = Roles.User)]
        public ActionResult TopUp()
        {
            return View(new PaymentViewModel());
        }

        /// <summary>
        /// Updates the user's balance.
        /// </summary>
        /// <param name="paymentView">Payment view model.</param>
        /// <returns>Balance view if operation was successfull otherwise TopUp view.</returns>
        [HttpPost]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult> TopUp(PaymentViewModel paymentView)
        {
            if (paymentView.Month > 12)
            {
                ModelState.AddModelError("", _localizer["Incorrect month"]);
            }

            if (paymentView.Year < 20 || paymentView.Year > 99)
            {
                ModelState.AddModelError("", _localizer["Incorrect year"]);
            }

            if (paymentView.CVC < 100 || paymentView.CVC > 999)
            {
                ModelState.AddModelError("", _localizer["Incorrect CVC"]);
            }

            if (!Regex.IsMatch(paymentView.Name, @"^[A-Za-z]+\s+[A-Za-z]+$"))
            {
                ModelState.AddModelError("", _localizer["Incorrect name"]);
            }

            if (!Regex.IsMatch(paymentView.CardNumber, @"^\d{16}$"))
            {
                ModelState.AddModelError("", _localizer["Invalid card number"]);
            }

            if (paymentView.Amount <= 0)
            {
                ModelState.AddModelError("", _localizer["Incorrect amount"]);
            }

            if (ModelState.IsValid)
            {
                var currentUser = await GetCurrentUser();
                currentUser.Balance += (decimal)paymentView.Amount;
                await _userClient.Put(currentUser);
                return RedirectToAction(nameof(Balance));
            }

            return View(paymentView);
        }

        /// <summary>
        /// Gets the view for editing contact info.
        /// </summary>
        /// <returns>View for editing contact info.</returns>
        [Authorize]
        [ReactRedirectFilter]
        public async Task<ActionResult> EditContactInfo()
        {
            var currentUser = await GetCurrentUser();
            return View(currentUser);
        }

        /// <summary>
        /// Edits the user's contact info.
        /// </summary>
        /// <param name="userModel">User model.</param>
        /// <returns>Edit contact info view.</returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> EditContactInfo(UserViewModel userModel)
        {
            var user = await _userClient.Get(userModel.Id);
            if ((await _userClient.GetByEmail(userModel.Email)) != null && user?.Email != userModel.Email)
            {
                ModelState.AddModelError("", _localizer["Email already exists"]);
            }

            if (ModelState.IsValid && user != null)
            {
                user.Name = userModel.Name;
                user.Surname = userModel.Surname;
                user.Email = userModel.Email;
                user.TimeZoneId = userModel.TimeZoneId;
                var result = await _userClient.Put(user);
                if (result)
                {
                    ViewData["SuccessMessage"] = _localizer["Information has been edited successfully"];
                }
            }

            return View(userModel);
        }

        /// <summary>
        /// Gets the view for changing a password.
        /// </summary>
        /// <returns>ChangePassword view.</returns>
        [Authorize]
        [ReactRedirectFilter]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Changes user password.
        /// </summary>
        /// <param name="currentPassword">Current password.</param>
        /// <param name="newPassword">New password.</param>
        /// <returns>ChangePassword view.</returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            if (currentPassword != null && newPassword != null)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isChanged = await _userClient.ChangePassword(currentUserId, currentPassword, newPassword);
                if (isChanged)
                {
                    ViewData["SuccessMessage"] = _localizer["Password has been changed successfully"];
                }
                else
                {
                    ModelState.AddModelError("", _localizer["Error, try again"]);
                }
            }

            return View();
        }

        /// <summary>
        /// Gets the Login view.
        /// </summary>
        /// <param name="returnUrl">Url to return after logging.</param>
        /// <returns>Login view.</returns>
        [ReactRedirectFilter]
        public ActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl,
            });
        }

        /// <summary>
        /// Logins user.
        /// </summary>
        /// <param name="loginModel">Login view model.</param>
        /// <returns>Redirect to returnUrl if credentials is valid otherwise Login view.</returns>
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var token = await _loginClient.Login(loginModel);
                if (!string.IsNullOrEmpty(token))
                {
                    AppendTokenToCookies(token);
                    return Redirect(loginModel?.ReturnUrl ?? "/");
                }
            }

            ModelState.AddModelError("", _localizer["Invalid name or password"]);
            return View(loginModel);
        }

        /// <summary>
        /// Gets the Registration view.
        /// </summary>
        /// <returns>Registration view.</returns>
        [ReactRedirectFilter]
        public ActionResult Registration()
        {
            return View(new RegistrationViewModel());
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registrationModel">Registretion view model.</param>
        /// <returns>Index view if operation was successful otherwise Registration view.</returns>
        [HttpPost]
        public async Task<ActionResult> Registration(RegistrationViewModel registrationModel)
        {
            if (await _userClient.GetByName(registrationModel.Username) != null)
            {
                ModelState.AddModelError("", _localizer["Username already exists"]);
            }

            if (await _userClient.GetByEmail(registrationModel.Email) != null)
            {
                ModelState.AddModelError("", _localizer["Email already exists"]);
            }

            if (ModelState.IsValid)
            {
                var token = await _loginClient.Register(registrationModel);
                if (!string.IsNullOrEmpty(token))
                {
                    AppendTokenToCookies(token);
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(registrationModel);
        }

        /// <summary>
        /// Gets the view for adding a new user.
        /// </summary>
        /// <returns>Create view.</returns>
        [Authorize(Roles = Roles.VenueManager)]
        [ReactRedirectFilter]
        public ActionResult Create()
        {
            return View(new RegistrationViewModel());
        }

        /// <summary>
        /// Creates a user account.
        /// </summary>
        /// <param name="registrationModel">Registration model.</param>
        /// <returns>Redirect to ManageUsers view if operation was successful otherwise Create view.</returns>
        [Authorize(Roles = Roles.VenueManager)]
        [HttpPost]
        public async Task<ActionResult> Create(RegistrationViewModel registrationModel)
        {
            if (await _userClient.GetByName(registrationModel.Username) != null)
            {
                ModelState.AddModelError("", _localizer["Username already exists"]);
            }

            if (await _userClient.GetByEmail(registrationModel.Email) != null)
            {
                ModelState.AddModelError("", _localizer["Email already exists"]);
            }

            if (ModelState.IsValid)
            {
                var token = await _loginClient.Register(registrationModel);
                if (!string.IsNullOrEmpty(token))
                {
                    return RedirectToAction(nameof(ManageUsers));
                }
            }

            return View(registrationModel);
        }

        /// <summary>
        /// Gets the view for managing users.
        /// </summary>
        /// <returns>ManageUsers view.</returns>
        [Authorize(Roles = Roles.VenueManager)]
        [ReactRedirectFilter]
        public async Task<ActionResult> ManageUsers()
        {
            var users = await _userClient.Get();
            foreach (var user in users)
            {
                user.Roles = (await _rolesClient.GetUserRoles(user.Id)).ToList();
            }

            return View(users);
        }

        /// <summary>
        /// Add new role to user.
        /// </summary>
        /// <returns>ManageUsers view.</returns>
        [Authorize(Roles = Roles.VenueManager)]
        public async Task<ActionResult> AddRole(string userId, string roleName)
        {
            var user = await _userClient.Get(userId);
            if (user != null && roleName != null)
            {
                await _rolesClient.AddToTole(user.Id, roleName);
                if (user.Id == User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                {
                    var refreshedToken = await _loginClient.RefreshToken(user.Id);
                    AppendTokenToCookies(refreshedToken);
                }
            }

            return RedirectToAction(nameof(ManageUsers));
        }

        /// <summary>
        /// Deeltes role from user.
        /// </summary>
        /// <returns>ManageUsers view.</returns>
        [Authorize(Roles = Roles.VenueManager)]
        public async Task<ActionResult> DeleteRole(string userId, string roleName)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != userId || roleName != Roles.VenueManager)
            {
                var user = await _userClient.Get(userId);
                if (user != null && roleName != null)
                {
                    await _rolesClient.DeleteFromTole(user.Id, roleName);
                    if (user.Id == User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                    {
                        var refreshedToken = await _loginClient.RefreshToken(user.Id);
                        AppendTokenToCookies(refreshedToken);
                    }
                }
            }

            return RedirectToAction(nameof(ManageUsers));
        }

        /// <summary>
        /// Deletes user by id.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>Result message in partial view.</returns>
        [Authorize(Roles = Roles.VenueManager)]
        public async Task<ActionResult> Delete(string userId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var messages = new List<string>();
            if (userId != null && userId != currentUserId)
            {
                var user = await _userClient.Get(userId);
                var result = await _userClient.Delete(user.Id);
                if (result)
                {
                    messages.Add(_localizer["Successfull operation"]);
                }
            }
            else
            {
                messages.Add(_localizer["Can't delete current user"]);
            }

            ViewData["Header"] = _localizer["Account deleting"];

            return PartialView("_ModalViewPartial", messages);
        }

        /// <summary>
        /// Exits from user account.
        /// </summary>
        /// <param name="returnUrl">Url to return after exit.</param>
        /// <returns>Redirect to return url.</returns>
        [Authorize]
        public RedirectResult Logout(string returnUrl = "/")
        {
            HttpContext.Session.Remove("cart");
            HttpContext.Response.Cookies.Delete(JwtTokenConstants.JwtCookieKey);
            return Redirect(returnUrl);
        }

        private async Task<UserViewModel> GetCurrentUser()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _userClient.Get(currentUserId);
        }

        private void AppendTokenToCookies(string token)
        {
            HttpContext.Response.Cookies.Append(JwtTokenConstants.JwtCookieKey, token, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromMinutes(60),
            });
        }
    }
}
