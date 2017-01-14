using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Configuration;
using SharpArch.Web.NHibernate;
using HorseLeague.Models.Domain;
using SharpArch.Core.PersistenceSupport;

namespace HorseLeague.Controllers
{
    public class FileController : HorseLeagueController
    {
        private readonly string configPath;
        private readonly string strippedConfigPath;

        private readonly IRepository<LeagueRace> leagueRaceRepository;

        public FileController(IRepository<LeagueRace> leagueRaceRepository)
        {
            this.leagueRaceRepository = leagueRaceRepository;

            configPath = ConfigurationManager.AppSettings["fileConfig"];
            strippedConfigPath = configPath.Substring(0, configPath.Length - 1);
        }
        
        [Authorize]
        public ActionResult Get(string fileName)
        {
            return this.File(fullFilePath(fileName), "application/pdf");
        }

        [Authorize(Users = "kurt,stephanie")]
        public ActionResult Upload(int id)
        {
            return View();
        }

        [Authorize(Users = "kurt,stephanie")]
        [AcceptVerbs(HttpVerbs.Post)]
        [Transaction]
        public ActionResult Upload(int id, FormCollection collection)
        {
            if(this.Request.Files.Count == 0)
            {
                ModelState.AddModelError("Form", "The upload form was blank");
                return View();
            }

            HttpPostedFileBase file = Request.Files["fileForm"];
            if ((file == null) || (file.ContentLength == 0) || string.IsNullOrEmpty(file.FileName))
            {
                ModelState.AddModelError("Form", "The upload form was either 0 length or the file name was blank.");
                return View();
            }

            string fileName = file.FileName;
            saveFile(file, fileName);

            LeagueRace leagueRace = this.UserLeague.League.GetLeagueRace(id);
            leagueRace.FormUrl = getUrl(fileName);

            this.leagueRaceRepository.SaveOrUpdate(leagueRace);

            return RedirectToAction("ViewLeagueRace", "Admin", new { id = id });
        }

        private void saveFile(HttpPostedFileBase file, string fileName)
        {
 
            string fileContentType = file.ContentType;
            byte[] fileBytes = new byte[file.ContentLength];
            file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

            file.SaveAs(fullFilePath(fileName));
        }

        private string FileRoot
        {
            get 
            {
                return String.Format("{0}{1}", this.Server.MapPath(this.Request.ApplicationPath), configPath); 
            }
        }
        private string fullFilePath(string fileName)
        {
            return String.Format("{0}{1}", FileRoot, fileName);
        }

        private string getUrl(string fileName)
        {
            //http://localhost:3643/file/get?filename=2015_bc_r3.pdf

            return String.Format("/file/get?filename={0}", fileName);
        }
    }
}
