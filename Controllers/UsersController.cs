using SharpArch.Web.NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HorseLeague.Models.Domain;


namespace HorseLeague.Controllers
{
    public class UsersController : HorseLeagueController
    {
         private readonly IMembershipService membershipService;
         public UsersController(IMembershipService membershipService) : base()
         {
             this.membershipService = membershipService;
         }

         [Authorize(Users = "kurt,stephanie")]
         public ActionResult Index()
         {
             this.ViewData["Users"] = this.UserRepository.GetAll();
            
             return View();
         }

         [Authorize(Users = "kurt,stephanie")]
         public ActionResult User(string userName)
         {
             var user = this.UserRepository.GetByUserName(userName);
             user.SecurityUser = this.membershipService.GetUser(userName);

             this.ViewData.Model = user;
             
             return View();
         }

         [Authorize(Users = "kurt,stephanie")]
         [AcceptVerbs(HttpVerbs.Post)]
         [Transaction]
         public ActionResult User(string userName, FormCollection collection)
         {
             var user = this.UserRepository.GetByUserName(userName);
             user.SecurityUser = this.membershipService.GetUser(userName);
             this.ViewData.Model = user;
             
             int userLeagueId = Convert.ToInt32(collection["txtUserLeagueId"]);

             bool? hasPaid = collection["chkHasPaid"].Contains("true") ? true : false;
             UserLeague.PaymentTypes paymentType = (UserLeague.PaymentTypes)Enum.Parse(typeof(UserLeague.PaymentTypes), 
                 collection["cmbPaymentType"], true);

             if((paymentType != UserLeague.PaymentTypes.NotPaid && !hasPaid.GetValueOrDefault(false)) ||
                  (paymentType == UserLeague.PaymentTypes.NotPaid && hasPaid.GetValueOrDefault(false))) {
                 ModelState.AddModelError("_FORM", "HasPaid and PaymentType are inconsistent.  They both need to reflect paying or not paying.");
             
                 return View();
             }
             var userLeague = user.UserLeagues.Where(ul => ul.Id == userLeagueId).SingleOrDefault();

             updateUserPayment(userLeague, hasPaid, paymentType, collection["txtPayPalToken"], collection["txtPayPalPayer"], collection["txtPayPalId"]);
                     
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