using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace HorseLeague.Services
{
    public class CaptchaService : ICaptchaService
    {
        private readonly string secretKey;

        public CaptchaService() : this(ConfigurationManager.AppSettings["CaptchaServerKey"]) { }

        public CaptchaService(string key) 
        {
            this.secretKey = key;
        }

        public bool IsValid(string captcha)
        {
            using (WebClient client = new WebClient())
            {
                var reqparm = new System.Collections.Specialized.NameValueCollection();
                reqparm.Add("secret", this.secretKey);
                reqparm.Add("response", captcha);

                byte[] responsebytes = client.UploadValues("https://www.google.com/recaptcha/api/siteverify", "POST", reqparm);
                string responsebody = Encoding.UTF8.GetString(responsebytes);
                var obj = JObject.Parse(responsebody); 
            
                return (bool)obj.SelectToken("success"); 
            }
        }
    }
}