
using Microsoft.Extensions.Logging;
using WebApiAngularIdentity.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataAccessLibary.Models;

using System.Collections.Concurrent;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net.Mail;
using WebApiAngularIdentity.Services.TaskQueue;
using DataAccessLibary.DataAccess;

namespace WebApiAngularIdentity.Workers
{
    public class TaskProcessor
    {
        private readonly ILogger<TaskProcessor> logger;
        private readonly Settings settings;
        private AuthenticationContext _authenticationContext;

        public static string AdminEmail = "justfortestsfaludore@gmail.com";
        public static string AdminPassword = "TestForJust123";

        public TaskProcessor(ILogger<TaskProcessor> logger, Settings settings, AuthenticationContext authenticationContext)
        {
            this.logger = logger;
            this.settings = settings;
            _authenticationContext = authenticationContext;
        }

        public async Task RunAsync(Mail number, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            Func<Mail, Mail> sender = null;

            sender = (model) =>
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Site admin", model.EmailFrom));
                emailMessage.To.Add(new MailboxAddress("", model.EmailTo));
                emailMessage.Subject = "Register on day book";
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = model.Message };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate(model.EmailFrom, AdminPassword);
                    try
                    {
                        client.Send(emailMessage);
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        for (int i = 0; i < ex.InnerExceptions.Length; i++)
                        {
                            System.Net.Mail.SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                            if (status == System.Net.Mail.SmtpStatusCode.MailboxBusy || status == System.Net.Mail.SmtpStatusCode.MailboxUnavailable)
                            {

                                System.Threading.Thread.Sleep(5000);
                                client.Send(emailMessage);
                                model.Status = false;                       
                                AddMailToDB(model);
                            }
                            else
                            {
                                model.Status = false;
                                AddMailToDB(model);
                            }
                        }
                    }
                    client.Disconnect(true);
                }
                model.Status = true;
                AddMailToDB(model);
                return model;
            };

            var result = await Task.Run(async () =>
            {
                await Task.Delay(1000);
                return  sender(number);
            }, token);

          

            logger.LogInformation($"Task finished. Result: {string.Join(" ", result)}");
        }
        void AddMailToDB(Mail mail)
        {
            using (_authenticationContext)
            {
                _authenticationContext.Mails.Add(mail);
                _authenticationContext.SaveChanges();
            }
        }
    }
}
