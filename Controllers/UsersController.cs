using SharpArch.Web.NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HorseLeague.Controllers
{
     [Authorize(Users = "kurt,stephanie")]
    public class UsersController : HorseLeagueController
    {
         private readonly IMembershipService membershipService;
         public UsersController(IMembershipService membershipService) : base()
         {
             this.membershipService = membershipService;
         }

         public ActionResult Index()
         {
             this.ViewData["Users"] = this.UserRepository.GetAll();
            
             return View();
         }

         public ActionResult View(string userName)
         {
             var user = this.UserRepository.GetByUserName(userName);
             user.SecurityUser = this.membershipService.GetUser(userName);

             this.ViewData.Model = user;
             
             return View();
         }

         [Transaction]
         public ActionResult UnlockUser(string userName)
         {
             this.membershipService.UnlockUser(userName);

             return RedirectToAction("View", new { userName = userName });
         }

         [Transaction]
         public ActionResult SetPayStatus(string userName, int userLeagueId, bool payStatus)
         {
             this.membershipService.UpdatePaid(userLeagueId, payStatus);

             return RedirectToAction("View", new { userName = userName });
         }
    }
}