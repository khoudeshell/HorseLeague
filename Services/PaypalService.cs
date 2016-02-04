using HorseLeague.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseLeague.Services
{
    public class PaypalService : IPaypalService
    {
        public string GenerateCallbackValue(IEncryptor encryptor, UserLeague userLeague)
        {
            PaypalDTO dto = new PaypalDTO(userLeague.Id);

            return encryptor.Encrypt(string.Format("{0},{1}", dto.LinkDate, userLeague.Id));
        }

        public PaypalDTO UnpackCallback(IEncryptor encryptor, string callback)
        {
            if (!string.IsNullOrEmpty(callback))
            {
                string decrypted = encryptor.Decrypt(callback);
                string[] vals = decrypted.Split(',');

                if (vals.Length == 2)
                {
                    return new PaypalDTO()
                    {
                        LinkDate = vals[0],
                        UserLeagueId = vals[1]
                    };
                }
            }
            return new PaypalDTO();
        }

    }

    public class PaypalDTO
    {
        public PaypalDTO() {}

        public PaypalDTO(int userLeagueId)
        {
            this.UserLeagueId = userLeagueId.ToString();
            this.LinkDate = DateTime.UtcNow.ToShortDateString();
        }

        public string UserLeagueId { get; set; }
        public string LinkDate { get; set; }

        public int ParsedUserLeagueId { get; private set; }

        public bool IsValid
        {
            get
            {
                if(!String.IsNullOrEmpty(UserLeagueId) && !String.IsNullOrEmpty(LinkDate))
                {
                    int userId;
                    DateTime linkDate;

                    if(int.TryParse(UserLeagueId, out userId) && DateTime.TryParse(LinkDate, out linkDate))
                    {
                        if(linkDate.AddMonths(1) > DateTime.UtcNow)
                        {
                            this.ParsedUserLeagueId = userId;
                            return true;
                        }
                    }
                }

                return false; 
            }
        }
    }
}