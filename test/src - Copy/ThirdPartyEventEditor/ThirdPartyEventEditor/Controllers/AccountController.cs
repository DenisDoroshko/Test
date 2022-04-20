using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ThirdPartyEventEditor.App_Start;
using ThirdPartyEventEditor.Filters;
using ThirdPartyEventEditor.Models;

namespace ThirdPartyEventEditor.Controllers
{
    /// <summary>
    /// Represents actions for working with user accounts.
    /// </summary>
    [LogTimeFilter]
    [GlobalExceptionFilter]
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        public AccountController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class. 
        /// </summary>
        /// <param name="userManager">User manager.</param>
        /// <param name="signInManager">Sign in manager.</param>
        /// <param name="roleManager">Role manager.</param>
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        /// <summary>
        /// Provides access to sign in manager.
        /// </summary>
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }

            private set
            {
                _signInManager = value;
            }
        }

        /// <summary>
        /// Provides access to role manager.
        /// </summary>
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }

            private set
            {
                _roleManager = value;
            }
        }

        /// <summary>
        /// Provides access to user manager.
        /// </summary>
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// Gets the view for managing users.
        /// </summary>
        /// <returns>ManageUsers view.</returns>
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> ManageUsers()
        {
            var users = UserManager.Users.ToList();
            foreach (var user in users)
            {
                user.RolesList = (await UserManager.GetRolesAsync(user.Id)).ToList();
            }
            return View(users);
        }

        /// <summary>
        /// Gets the Login view.
        /// </summary>
        /// <param name="returnUrl">Url to return after logging.</param>
        /// <returns>Login view.</returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = "/")
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl});
        }

        /// <summary>
        /// Logins user.
        /// </summary>
        /// <param name="model">Login view model.</param>
        /// <returns>Redirect to returnUrl if credentials is valid otherwise Login view.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            switch (result)
            {
                case SignInStatus.Success:
                    return Redirect(model.ReturnUrl);
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        /// <summary>
        /// Gets the Register view.
        /// </summary>
        /// <returns>Register view.</returns>
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">Register model</param>
        /// <returns>Index view if operation was successful otherwise Register view.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(model);
        }


        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Add new role to user.
        /// </summary>
        /// <returns>ManageUsers view.</returns>
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> AddRole(string userId, string roleName)
        {
            var user = await UserManager.FindByIdAsync(userId);
            if (user != null && roleName != null)
            {
                await UserManager.AddToRoleAsync(user.Id, roleName);
            }

            return RedirectToAction(nameof(ManageUsers));
        }

        /// <summary>
        /// Deeltes role from user.
        /// </summary>
        /// <returns>ManageUsers view.</returns>
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> DeleteRole(string userId, string roleName)
        {
            var currentUserId = HttpContext.User.Identity.GetUserId();
            if (currentUserId != userId || roleName != Roles.Admin)
            {
                var user = await UserManager.FindByIdAsync(userId);
                if (user != null && roleName != null)
                {
                    await UserManager.RemoveFromRoleAsync(user.Id, roleName);
                }
            }

            return RedirectToAction(nameof(ManageUsers));
        }

        /// <summary>
        /// Deletes user by id.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>Result message in partial view.</returns>
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> Delete(string userId)
        {
            var currentUserId = HttpContext.User.Identity.GetUserId();
            if (userId != null && userId != currentUserId)
            {
                var user = await UserManager.FindByIdAsync(userId);
                var result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ManageUsers));
                }
            }

            return RedirectToAction("Error", "Home", new { message = "Can't delete this user" });
        }
    }
}