using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using HorseLeague.Models.DataAccess;
using Microsoft.Practices.ServiceLocation;
using HorseLeague.Models.Domain;
using SharpArch.Web.NHibernate;
using HorseLeague.Logger;
using HorseLeague.Services;
using System.Configuration;
using SharpArch.Core.PersistenceSupport;
using HorseLeague.Email;

namespace HorseLeague.Controllers
{

    [HandleError]
    public class AccountController : Controller
    {

        // This constructor is used by the MVC framework to instantiate the controller using
        // the default forms authentication and membership providers.

        public AccountController()
            : this(null, null, null) { }

        // This constructor is not used by the MVC framework but is instead provided for ease
        // of unit testing this type. See the comments at the end of this file for more
        // information.
        public AccountController(IFormsAuthentication formsAuth, IMembershipService service,
            ILogger logger) : this(formsAuth, service, logger, null, null) { }
        
        public AccountController(IFormsAuthentication formsAuth, IMembershipService service,
            ILogger logger, IPaypalService paypalService, IEncryptor encryptor)
        {
            FormsAuth = formsAuth ?? new FormsAuthenticationService();
            MembershipService = service ?? new AccountMembershipService();
            Logger = logger ?? new Logger.Logger();
            PaypalService = paypalService ?? new PaypalService();
            Encryptor = encryptor ?? new Encryptor(ConfigurationManager.AppSettings["AESKey"] == null ? String.Empty : ConfigurationManager.AppSettings["AESKey"].ToString());
        }
        
        public IFormsAuthentication FormsAuth
        {
            get;
            private set;
        }

        public IMembershipService MembershipService
        {
            get;
            private set;
        }

        public ILogger Logger
        {
            get;
            private set;
        }

        public IPaypalService PaypalService
        {
            get;
            private set;
        }

        public IEncryptor Encryptor
        {
            get;
            private set;
        }

        #region LogOn
        public ActionResult LogOn()
        {

            return View();
        }


        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
            Justification = "Needs to take same parameter type as Controller.Redirect()")]
        public ActionResult LogOn(string userName, string password, bool rememberMe, string returnUrl)
        {

            if (!ValidateLogOn(userName, password))
            {
                ViewData["rememberMe"] = rememberMe;
                return View();
            }

            FormsAuth.SignIn(userName, rememberMe);
            Logger.LogInfo("Signed in user: " + userName);

            if (!String.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region LogOff
        public ActionResult LogOff()
        {

            FormsAuth.SignOut();

            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Register
        public ActionResult Register()
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Transaction]
        public ActionResult Register(string userName, string email, string password, string confirmPassword)
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            if (ValidateRegistration(userName, email, password, confirmPassword))
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(userName, password, email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    Logger.LogInfo(string.Format("Registered new user: {0}, email: {1}", userName, email));
 
                    FormsAuth.SignIn(userName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("_FORM", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }
        #endregion

        #region Forgot User Name
        public ActionResult ForgotUserName()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForgotUserName(string email)
        {

            if (String.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", "You must enter an email address.");
                return View();
            }

            MembershipUserCollection users = MembershipService.FindUsersByEmail(email);
            if(users == null || users.Count == 0)
            {
                ModelState.AddModelError("email", "No users were found for that email.");
                return View();
            }

            Emailer.SendEmail(new ForgotUserNameEmailTemplate(users), email);

            ViewData["email"] = email;

            return View("ForgotUserNameSuccess");
        }
        #endregion

        #region Change Password
        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            return View();
        }
        
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exceptions result in password not being changed.")]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            if (!ValidateChangePassword(currentPassword, newPassword, confirmPassword))
            {
                return View();
            }

            try
            {
                if (MembershipService.ChangePassword(User.Identity.Name, currentPassword, newPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                    return View();
                }
            }
            catch
            {
                ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                return View();
            }
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
        #endregion

        #region Reset Password
        [Authorize]
        public ActionResult ResetPassword(string userName)
        {
            ViewData["NewPassword"] = this.MembershipService.ResetPassword(userName);

            return View();
        }
        #endregion

        #region Record Paid Status
        [Transaction]
        public ActionResult RecordPaidStatus(string paypalCallback)
        {
            try
            {
                var paypalDTO = PaypalService.UnpackCallback(this.Encryptor, paypalCallback);

                if (paypalDTO.IsValid)
                {
                    if (!this.MembershipService.UpdatePaid(paypalDTO.ParsedUserLeagueId, true))
                    {
                        Logger.LogInfo("Updating user as paid: " + paypalDTO.UserLeagueId);
                        ModelState.AddModelError("_FORM", "The user was not found.");
                        Logger.LogInfo("Did not register paypal callback for user league id: " + paypalDTO.ParsedUserLeagueId.ToString());
                    }
                }
                else
                {
                    ModelState.AddModelError("_FORM", "The paypal return link was either invalid or expired.");
                    Logger.LogInfo(String.Format("The paypal return link was either invalid or expired. UserLeagueId: {0} ExpirationDate: {1}",
                        paypalDTO.UserLeagueId, paypalDTO.LinkDate));
                }
            }
            catch(Exception e)
            {
                Logger.LogError("Error recording paypal info", e);
                ModelState.AddModelError("_FORM", "The paypal return link was invalid.");
                    
            }
            return View();
        }
        #endregion

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity is WindowsIdentity)
            {
                throw new InvalidOperationException("Windows authentication is not supported.");
            }
        }

        #region Validation Methods

        private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (String.IsNullOrEmpty(currentPassword))
            {
                ModelState.AddModelError("currentPassword", "You must specify a current password.");
            }
            if (newPassword == null || newPassword.Length < MembershipService.MinPasswordLength)
            {
                ModelState.AddModelError("newPassword",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a new password of {0} or more characters.",
                         MembershipService.MinPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }

            return ModelState.IsValid;
        }

        private bool ValidateLogOn(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "You must specify a password.");
            }
            if (!MembershipService.ValidateUser(userName, password))
            {
                ModelState.AddModelError("_FORM", "The username or password provided is incorrect.");
            }

            return ModelState.IsValid;
        }

        private bool ValidateRegistration(string userName, string email, string password, string confirmPassword)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", "You must specify an email address.");
            }
            if (password == null || password.Length < MembershipService.MinPasswordLength)
            {
                ModelState.AddModelError("password",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a password of {0} or more characters.",
                         MembershipService.MinPasswordLength));
            }
            if (!String.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }
            return ModelState.IsValid;
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }

    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.

    public interface IFormsAuthentication
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthentication
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        string ResetPassword(string userName);
        MembershipUser GetUser(string userName);
        bool UpdatePaid(int userLeagueId, bool value);
        void UnlockUser(string userName);
        MembershipUserCollection FindUsersByEmail(string email);
    }

    public class AccountMembershipService : IMembershipService
    {
        private MembershipProvider _provider;
        private readonly ILeagueRepository leagueRepository;
        private readonly IUserRepository userRepository;
        private readonly IRepository<UserLeague> userLeagueRepository; 

        public AccountMembershipService()
            : this(null, ServiceLocator.Current.GetInstance<ILeagueRepository>(),
                    ServiceLocator.Current.GetInstance<IUserRepository>(),
                    ServiceLocator.Current.GetInstance<IRepository<UserLeague>>())
        {
        }

        public AccountMembershipService(MembershipProvider provider, 
            ILeagueRepository leagueRepository,
            IUserRepository userRepository,
            IRepository<UserLeague> userLeagueRepository)
        {
            _provider = provider ?? Membership.Provider;
            this.leagueRepository = leagueRepository;
            this.userRepository = userRepository;
            this.userLeagueRepository = userLeagueRepository;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            if (status == MembershipCreateStatus.Success)
            {
                League league = this.leagueRepository.GetDefaultLeague();
                User u = userRepository.GetByUserName(userName);
                u.UserLeagues.Add(new UserLeague()
                {
                    League = league,
                    User = u,
                    HasPaid = false
                });
                userRepository.SaveOrUpdate(u);
            }
            return status;
        }
        
        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
            return currentUser.ChangePassword(oldPassword, newPassword);
        }

        public string ResetPassword(string userName)
        {
            return _provider.ResetPassword(userName, null);
        }

        public MembershipUser GetUser(string userName)
        {
            return _provider.GetUser(userName, false);
        }

        public MembershipUserCollection FindUsersByEmail(string email)
        {
            int countOfUsers;
            
            return _provider.FindUsersByEmail(email, 0, 100, out countOfUsers);
        }

        public bool UpdatePaid(int userLeagueId, bool value)
        {
            var userLeague = userLeagueRepository.Get(userLeagueId);

            if (userLeague == null) return false;

            userLeague.HasPaid = value;
            userLeagueRepository.SaveOrUpdate(userLeague);
        
            return true;
        }

        public void UnlockUser(string userName)
        {
            _provider.UnlockUser(userName);
        }
    }
}
