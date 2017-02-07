using HorseLeague.Models.Domain;
using SharpArch.Core.PersistenceSupport;
using SharpArch.Web.NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HorseLeague.Controllers
{
     [Authorize(Users = "kurt,stephanie")]
    public class LogsController : HorseLeagueController
    {
         private readonly IRepository<LogItem> logRepository;

         public LogsController(IRepository<LogItem> logRepo) : base() 
         {
             this.logRepository = logRepo;
         }
         
         public ActionResult Index()
         {
             this.ViewData["Logs"] = this.logRepository.GetAll().OrderByDescending(x => x.Date).Take(50).ToList();
            
             return View();
         }

    }
}