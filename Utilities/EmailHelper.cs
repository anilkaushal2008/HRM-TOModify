using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace HRM.Utilities
{
    public static class EmailHelper
    {
        public static bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                var fromEmail = ConfigurationManager.AppSettings["SMTPUser"];
                var smtpHost = ConfigurationManager.AppSettings["SMTPHost"];
                var smtpPort = 587; //Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                var smtpPass = ConfigurationManager.AppSettings["SMTPPassword"];
                var enableSSL = bool.Parse(ConfigurationManager.AppSettings["EnableSSL"]);
                var mail = new MailMessage();               
                mail.From = new MailAddress(fromEmail);
                mail.Subject = subject;
                mail.Body = body;
                //mail.Body = "Test";
                mail.IsBodyHtml = true;
                //mail.To.Add(toEmail);                
                mail.To.Add("anil@indushealthcare.in");
                ServicePointManager.ServerCertificateValidationCallback =
                 (sender, certificate, chain, sslPolicyErrors) => true;
                var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail, smtpPass),
                    EnableSsl = true // Enables STARTTLS  
                };
                smtpClient.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                if (ex.InnerException != null)
                {
                    // Log error if needed
                    error += " | Inner Exception: " + ex.InnerException.Message;
                }
                return false;
            }
        }
    }
}
