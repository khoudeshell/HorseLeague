using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HorseLeague.Models.Domain;
using System.Net.Mail;
using System.Net;

namespace HorseLeague.Email
{
    public class Emailer
    {
        public static void SendEmail(IEmailTemplate emailTemplate, string toAddress)
        {
            MailMessage email = new MailMessage();
            email.From = new MailAddress("postmaster@triplecrownroyal.com");
            email.To.Add(new MailAddress(toAddress));
            email.Subject = emailTemplate.Subject;
            email.Body = emailTemplate.Body;
            
#if !DEBUG
            SmtpClient client = new SmtpClient();
            client.EnableSsl = false;
            client.Send(email);
#endif
        }
    }
}
