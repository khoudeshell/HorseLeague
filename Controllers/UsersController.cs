using SharpArch.Web.NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HorseLeague.Models.Domain;
using HorseLeague.Models.DataAccess;


namespace HorseLeague.Controllers
{
    public class UsersController : HorseLeagueController
    {
         private readonly IMembershipService membershipService;
         private readonly IUserReportRepository userReportRepository;

         public UsersController(IMembershipService membershipService, IUserReportRepository userReportRepository)
             : base()
         {
             this.membershipService = membershipService;
             this.userReportRepository = userReportRepository;
         }

         [Authorize(Users = "kurt,stephanie")]
         public ActionResult Index()
         {
             this.ViewData["Users"] = getAllUsers()
                 .Where(x => x.LastActivityDate > DateTime.Now.AddYears(-1));
            
             return View();
         }

         [Authorize(Users = "kurt,stephanie")]
         public ActionResult GetAll()
         {
             this.ViewData["Users"] = getAllUsers()
                 .OrderBy(x => x.UserName);

             return View("Index");
         }

         [Authorize(Users = "kurt,stephanie")]
         public ActionResult Unpaid()
         {
             this.ViewData["Users"] = getAllUsers()
                 .Where(x => x.HasPaid == false)
                 .OrderBy(x => x.UserName);

             return View("Index");
         }

         [Authorize(Users = "kurt,stephanie")]
         public ActionResult Activity()
         {
             this.ViewData["Users"] = getAllUsers()
                 .Where(x => x.LastActivityDate > DateTime.Now.AddYears(-1))
                 .OrderByDescending(x => x.LastActivityDate);

             return View("Index");
         }

         private IList<UserReport> getAllUsers() 
         {
             return this.userReportRepository.GetAllUsers(null);
         }

         [Authorize(Users = "kurt,stephanie")]
#pragma warning disable CS0108 // 'UsersController.User(string)' hides inherited member 'Controller.User'. Use the new keyword if hiding was intended.
         public ActionResult User(string userName)
#pragma warning restore CS0108 // 'UsersController.User(string)' hides inherited member 'Controller.User'. Use the new keyword if hiding was intended.
         {
             var user = this.UserRepository.GetByUserName(userName);
             user.SecurityUser = this.membershipService.GetUser(userName);

             this.ViewData.Model = user;
             
             return View();
         }

         [Authorize(Users = "kurt,stephanie")]
         [AcceptVerbs(HttpVerbs.Post)]
         [Transaction]
#pragma warning disable CS0108 // 'UsersController.User(string, FormCollection)' hides inherited member 'Controller.User'. Use the new keyword if hiding was intended.
         public ActionResult User(string userName, FormCollection collection)
#pragma warning restore CS0108 // 'UsersController.User(string, FormCollection)' hides inherited member 'Controller.User'. Use the new keyword if hiding was intended.
         {
             var user = this.UserRepository.GetByUserName(userName);
             user.SecurityUser = this.membershipService.GetUser(userName);
             this.ViewData.Model = user;
             bool? hasPaid = false;
             
             int userLeagueId = Convert.ToInt32(collection["txtUserLeagueId"]);

             UserLeague.PaymentTypes paymentType = (UserLeague.PaymentTypes)Enum.Parse(typeof(UserLeague.PaymentTypes), 
                 collection["cmbPaymentType"], true);

             if (paymentType != UserLeague.PaymentTypes.NotPaid)
             {
                 hasPaid = true;
             }
             var userLeague = user.UserLeagues.Where(ul => ul.Id == userLeagueId).SingleOrDefault();

             updateUserPayment(userLeague, hasPaid, paymentType, collection["txtPayPalToken"], 
                 collection["txtPayPalPayer"], collection["txtPayPalId"]);
                     
             return RedirectToAction("User", new { userName = userName });
         }

        /// <summary>
        /// Note This is the only non-administrative method on this controller.  Used in paypal flow for validation
        /// </summary>
        /// <param name="paymentToken"></param>
        /// <param name="payerID"></param>
        /// <param name="paymentID"></param>
        /// <returns></returns>
         [AcceptVerbs(HttpVerbs.Post)]
         [Transaction]
         public ActionResult RecordPaidStatus(string paymentToken, string payerID, string paymentID)
         {
             var userLeague = this.HorseUser.UserLeagues[0];
             var updateSuccess = false;

             try
             {
                 updateUserPayment(userLeague, true, UserLeague.PaymentTypes.PayPal, 
                     paymentToken, payerID, paymentID);   

                 updateSuccess = true;
             }
             catch (Exception e)
             {
                 Logger.LogError("Error recording payment info", e);
             }
             return new JsonResult()
             {
                 Data = new { success = updateSuccess }
             };
         }

         [Authorize(Users = "kurt,stephanie")]
         [Transaction]
         public ActionResult UnlockUser(string userName)
         {
             this.membershipService.UnlockUser(userName);

             return RedirectToAction("View", new { userName = userName });
         }

         private void updateUserPayment(UserLeague userLeague, bool? hasPaid, UserLeague.PaymentTypes paymentType, 
             string paymentToken, string payerID, string paymentID)
         {
             Logger.LogInfo("Updating user as paid: " + userLeague.Id);

             userLeague.HasPaid = hasPaid;
             userLeague.PaymentType = paymentType;
             userLeague.PayPalPaymentToken = paymentToken;
             userLeague.PayPalPaymentId = paymentID;
             userLeague.PayPalPayerId = payerID;

             this.membershipService.UpdatePaid(userLeague);
             Logger.LogInfo("Success updating pay status.  User: " + userLeague.Id);
         }
    }
}