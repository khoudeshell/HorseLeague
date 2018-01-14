using SharpArch.Web.NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
         public ActionResult View(string userName)
         {
             var user = this.UserRepository.GetByUserName(userName);
             user.SecurityUser = this.membershipService.GetUser(userName);

             this.ViewData.Model = user;
             
             return View();
         }

         [Authorize(Users = "kurt,stephanie")]
         [Transaction]
         public ActionResult UnlockUser(string userName)
         {
             this.membershipService.UnlockUser(userName);

             return RedirectToAction("View", new { userName = userName });
         }

         [Authorize(Users = "kurt,stephanie")]
         [Transaction]
         public ActionResult SetPayStatus(string userName, int userLeagueId, bool payStatus)
         {
             this.membershipService.UpdatePaid(userLeagueId, payStatus);

             return RedirectToAction("View", new { userName = userName });
         }

         [AcceptVerbs(HttpVerbs.Post)]
         [Transaction]
         public ActionResult RecordPaidStatus(string paymentToken, string payerID, string paymentID)
         {
             var userLeagueId = this.HorseUser.UserLeagues[0].Id;
             var updateSuccess = false;

             try
             {
                 Logger.LogInfo("Updating user as paid: " + userLeagueId);

                 if (this.membershipService.UpdatePaid(userLeagueId, true))
                 {
                     Logger.LogInfo("Success updating pay status.  User: " + userLeagueId);
                     updateSuccess = true;
                 }
                 else
                 {
                     Logger.LogInfo("Did not register paypal transaction for league id: " + userLeagueId.ToString());
                 }
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
    }
}