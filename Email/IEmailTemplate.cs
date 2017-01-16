using HorseLeague.Models;
using HorseLeague.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace HorseLeague.Email
{
    public interface IEmailTemplate
    {
        string Subject { get;  }
        string Body { get;  }
    }

    public class RacePickSuccessEmailTemplate : IEmailTemplate
    {
        private string _raceName;
        private IList<UserRaceDetail> _picks;
        private int _userId;

        public RacePickSuccessEmailTemplate(string raceName, IList<UserRaceDetail> picks, int userId)
        {
            this._raceName = raceName;
            this._userId = userId;

            if(picks == null || picks.Count != 4)
            {
                throw new InvalidOperationException("The list of picks for the email does not have all the picks in it.");
            }

            this._picks = picks;
        }

        public string Subject
        {
            get 
            {
                return String.Format("Picks for {0}", _raceName);
            }
        }

        public string Body
        {
            get
            {
                StringBuilder emailBody = new StringBuilder();

                CreatePickText(emailBody, BetTypes.Win);
                CreatePickText(emailBody, BetTypes.Place);
                CreatePickText(emailBody, BetTypes.Show);
                CreatePickText(emailBody, BetTypes.Backup);

                emailBody.AppendLine("");
                emailBody.AppendLine("User id: " + this._userId.ToString());

                return emailBody.ToString();
            }
        }


        private void CreatePickText(StringBuilder output, BetTypes betType)
        {
            RaceDetail rd = this._picks.Where(x => x.BetType == betType).FirstOrDefault().RaceDetail;

            output.AppendLine(String.Format("{0}: {1}-{2}", betType.ToString(),
                rd.PostPosition, rd.Horse.Name));
        }

    }

    public class EmailTester : IEmailTemplate
    {
        public string Subject
        {
            get { return "Test Subject"; }
        }

        public string Body 
        {
            get { return "Test Body"; }
        }
    }

    public class ForgotUserNameEmailTemplate : IEmailTemplate
    {
        private MembershipUserCollection _users;

        public ForgotUserNameEmailTemplate(MembershipUserCollection users)
        {
            this._users = users;
        }

        public string Subject
        {
            get { return "Forgot User Name for Triple Crown Royal"; }
        }

        public string Body
        {
            get 
            {
                StringBuilder emailBody = new StringBuilder();

                emailBody.AppendLine("The follow users are registered to this email address:");
                foreach (MembershipUser user in _users)
                {
                    emailBody.AppendLine(user.UserName);
                }
                return emailBody.ToString();
            }
        }
    }
}
