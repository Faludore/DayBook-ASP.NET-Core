using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using DataAccessLibary.Models;
using MimeKit;

namespace WebApiAngularIdentity.Services.Sender
{
    public class EmailSender : IEmailSender
    {
        public static string AdminEmail = "justfortestsfaludore@gmail.com";
        public static string AdminPassword = "TestForJust123";

        public EmailSender()
        {
        }

        public async Task Send(Email email)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Site admin", email.EmailFrom));
            emailMessage.To.Add(new MailboxAddress("", email.EmailTo));
            emailMessage.Subject = "Register on day book";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = email.Message };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync(email.EmailFrom, AdminPassword);
                try
                {
                    await client.SendAsync(emailMessage);
                }
                catch (SmtpFailedRecipientsException ex)
                {
                    for (int i = 0; i < ex.InnerExceptions.Length; i++)
                    {
                        System.Net.Mail.SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                        if (status == System.Net.Mail.SmtpStatusCode.MailboxBusy || status == System.Net.Mail.SmtpStatusCode.MailboxUnavailable)
                        {

                            System.Threading.Thread.Sleep(5000);
                            await client.SendAsync(emailMessage);
                            email.Status = false;
                        }
                        else
                        {
                            email.Status = false;
                        }
                    }
                }
                await client.DisconnectAsync(true);
            }
            email.Status = true;

        }

    }
}
