using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HorseLeague.Models;
using HorseLeague.Models.DataAccess;
using HorseLeague.Email;
using HorseLeague.Models.Domain;
using SharpArch.Web.NHibernate;
using Microsoft.Practices.ServiceLocation;
using SharpArch.Core.PersistenceSupport;
using HorseLeague.Logger;


namespace HorseLeague.Controllers
{
    [HandleError]
    public class HomeController : HorseLeagueController
    {
        private readonly IMembershipService membershipService;
        private readonly IRepository<UserLeague> userLeagueRepository;
        
        public HomeController(IMembershipService membershipService,
            IRepository<UserLeague> userLeagueRepository) : this(membershipService, userLeagueRepository, null, null) { }
            
        
        public HomeController(IMembershipService membershipService,
           IRepository<UserLeague> userLeagueRepository,
           IUserRepository dataRepository,
           ILogger logger) :
            base(dataRepository, logger)
        {
            this.membershipService = membershipService ?? new AccountMembershipService();
            this.userLeagueRepository = userLeagueRepository;
        }

        [Authorize]
        public ActionResult Index()
        {
            this.ViewData["ActiveRaces"] = this.UserLeague.League.ActiveRaces;
            this.ViewData["UserDomain"] = this.UserLeague;
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [Authorize]
        public ActionResult Picks(int id) 
        {
            LeagueRace leagueRace = this.UserLeague.League.GetLeagueRace(id);
            this.ViewData.Model = leagueRace;
            this.ViewData["UserDomain"] = this.UserLeague;
            this.ViewData["UserPicks"] = this.UserLeague.GetPicksForARace(leagueRace);

            return View(); 
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [Transaction]
        public ActionResult Picks(int id, FormCollection collection)
        {
            Logger.LogInfo(string.Format("User: {0}, form: {1}", this.User.Identity.Name,
                getFormCollection(collection)));

            LeagueRace leagueRace = this.UserLeague.League.GetLeagueRace(id);
            this.ViewData.Model = leagueRace;
            this.ViewData["UserDomain"] = this.UserLeague;
            this.ViewData["UserPicks"] = new List<UserRaceDetail>();

            if (!leagueRace.IsUpdateable)
            {
                ModelState.AddModelError("_FORM", "This race is no longer eligible to change");
                return View();
            }

            IList<UserRaceDetail> userSelections = new
                    List<UserRaceDetail>();
            try
            {
                parseUserPicksRequest(leagueRace, userSelections, collection);

                this.UserLeague.AddUserPicksForRace(leagueRace, userSelections);
            }
            catch(InvalidPicksForARaceException ex)
            {
                //The temporary picks need to be put in there so they arent lost on the return page
                this.ViewData["UserPicks"] = userSelections;

                ModelState.AddModelError("_FORM", ex.Message);
                ModelState.AddModelError("_FORM", "All picks must be entered for the race.");
                foreach (string dropDown in ex.MissingPicks)
                {
                    ModelState.AddModelError(dropDown, string.Format("{0} must have a selection",
                        dropDown.Replace("cmb", "")));
                }

                return View();
            }
            
            this.userLeagueRepository.SaveOrUpdate(this.UserLeague);
            this.ViewData["UserPicks"] = this.UserLeague.GetPicksForARace(leagueRace);
            this.ViewData["SuccessMessage"] = "Picks updated successfully";
            Logger.LogInfo(string.Format("Saved picks for User: {0}", this.User.Identity.Name));

            Emailer.SendEmail(new RacePickSuccessEmailTemplate(leagueRace.Race.Name, UserLeague.GetPicksForARace(leagueRace), UserLeague.Id), 
                membershipService.GetUser(this.HorseUser.UserName).Email);

            return View();
        }

        private void parseUserPicksRequest(LeagueRace leagueRace, IList<UserRaceDetail> userSelections, FormCollection collection)
        {
            InvalidPicksForARaceException invalidPicks = new InvalidPicksForARaceException();

            parseUserPickRequest(leagueRace, userSelections, collection, "cmbWin", BetTypes.Win, invalidPicks);
            parseUserPickRequest(leagueRace, userSelections, collection, "cmbPlace", BetTypes.Place, invalidPicks);
            parseUserPickRequest(leagueRace, userSelections, collection, "cmbShow", BetTypes.Show, invalidPicks);
            parseUserPickRequest(leagueRace, userSelections, collection, "cmbBackUp", BetTypes.Backup, invalidPicks);

            if (invalidPicks.MissingPicks.Count > 0) throw invalidPicks;
        }

        private void parseUserPickRequest(LeagueRace leagueRace, IList<UserRaceDetail> userSelections, FormCollection collection, 
            string dropDown, BetTypes betTypes, InvalidPicksForARaceException invalidPicks)
        {
            int raceDetailId = Convert.ToInt32(collection[dropDown]);
            
            if (raceDetailId == -1)
            {
                invalidPicks.MissingPicks.Add(dropDown);
            }
            else
            {
                UserRaceDetail urd = new UserRaceDetail()
                {
                    BetType = betTypes,
                    RaceDetail = leagueRace.RaceDetails.Where(x => x.Id == raceDetailId).First(),
                    UserLeague = this.UserLeague,
                    UpdateDate = DateTime.Now
                };
                userSelections.Add(urd);
            }
        }

        private string getFormCollection(FormCollection collection)
        {
            string vals = string.Empty;

            foreach (var key in collection.AllKeys)
            {
                vals += string.Format("key:{0},value{1};", key.ToString(), collection[key].ToString());
            }

            return vals;
        }
    }
}
