using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace DAW_social_platform.Infrastructure
{
    public class EmailConfig
    {
        public void SendEmailNotification(string toEmail, string subject, string content)
        {
            const string senderEmail = "daw.social.app@gmail.com";
            const string senderPassword = "DAW_social_plat(2020)";
            const string smtpServer = "smtp.gmail.com";
            const int smtpPort = 587;

            SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);

            MailMessage email = new MailMessage(senderEmail, toEmail, subject, content);
            email.IsBodyHtml = true;
            email.BodyEncoding = UTF8Encoding.UTF8;

            try
            {
                smtpClient.Send(email);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error occured while trying to send email");
                System.Diagnostics.Debug.WriteLine(e.Message.ToString());
            }
        }

    }
}