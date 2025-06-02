using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Azure.Functions.Worker;
using MailKit;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace AzureFunctionSendEmail
{    
    public class SendEmail
    {
        private readonly ILogger<SendEmail> _logger;
        private readonly IMailService _mailService;

        public SendEmail(ILogger<SendEmail> logger, IMailService mailService)
        {
            _logger = logger;
            _mailService = mailService;
        }

        
        [Function("SendEmail")]        
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req
            //[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req
            //[SendGrid(ApiKey = "SendGridApiKey")] out SendGridMessage message
            )
        {

            _logger.LogInformation($"Timer function executed at: {DateTime.Now}");


            _logger.LogInformation(LogMessage("1.Start function"));
            var mailData = new MailData();
            mailData.EmailSubject = "Test Subject";
            mailData.EmailBody = "Some content";
            mailData.EmailToName = "Linh Ng";
            mailData.EmailToId = "linhdeveloper@gmail.com";

            _logger.LogInformation(LogMessage("2.Before sending email"));

            var result = _mailService.SendMail(mailData);

            _logger.LogInformation(LogMessage("3.After sending email"));

            _logger.LogInformation(LogMessage("4.End function"));

            return new OkObjectResult("Email request submitted(V2).");
        }

        private string LogMessage(string message)
        {
            var prefix = "***********************>>> ";
            var result = String.Format("{0}{1}", prefix, message);

            return result;
        }
    }


    public class MyInfo
    {
        public bool IsPastDue { get; set; }
    }

    public class MailData
    {
        public string EmailToId { get; set; }
        public string EmailToName { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
    }
    public class MailSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public interface IMailService
    {
        bool SendMail(MailData mailData);
    }
    public class MailService() : IMailService
    {
        private readonly MailSettings _mailSettings;        //--Inject mailsettings in Mail service
        private readonly ILogger<MailService> _logger;

        public MailService(IOptions<MailSettings> mailSettingsOptions, ILogger<MailService> logger) : this()
        {
            _mailSettings = mailSettingsOptions.Value;
            _logger = logger;
        }
        private string LogMessage(string message)
        {
            var prefix = "***********************>>> ";
            var result = String.Format("{0}{1}", prefix, message);

            return result;
        }
        public bool SendMail(MailData mailData)
        {
            try
            {
                _logger.LogInformation(LogMessage("2.1 Start SendMail()"));
                using (MimeMessage emailMessage = new MimeMessage())
                {
                    _logger.LogInformation(LogMessage("2.2 Get environment variables"));

                    //string smtpUser = Environment.GetEnvironmentVariable("UserName2");
                    //string smtpPass = Environment.GetEnvironmentVariable("Password2");
                    //string smtpHost = Environment.GetEnvironmentVariable("Server");
                    //int smtpPort = int.Parse(Environment.GetEnvironmentVariable("Port") ?? "587");
                    /*
                       "Server": "smtp.ethereal.email",
                        "Port": 587,
                        "SenderName": "Sender Name",
                        "SenderEmail": "linhdeveloper@gmail.com",
                        "UserName2": "bill.vandervort9@ethereal.email",
                        "Password2": "WVECAuMK4rwFv1yqPj"
                     */
                    string smtpUser = "bill.vandervort9@ethereal.email";
                    string smtpPass = "WVECAuMK4rwFv1yqPj";
                    string smtpHost = "smtp.ethereal.email";
                    int smtpPort = 587;


                    MailboxAddress emailFrom = new MailboxAddress("Sender FN LN", "sender@gmail.com");
                    emailMessage.From.Add(emailFrom);
                    MailboxAddress emailTo = new MailboxAddress("Linh Ng", "linhdeveloper@gmail.com");
                    emailMessage.To.Add(emailTo);

                    emailMessage.Subject = "New Subject";

                    BodyBuilder emailBodyBuilder = new BodyBuilder();
                    emailBodyBuilder.TextBody = mailData.EmailBody;

                    emailMessage.Body = emailBodyBuilder.ToMessageBody();
                    //this is the SmtpClient from the Mailkit.Net.Smtp namespace, not the System.Net.Mail one

                    _logger.LogInformation(LogMessage("2.5 Creat mail client object"));

                    try
                    {
                        using (MailKit.Net.Smtp.SmtpClient mailClient = new MailKit.Net.Smtp.SmtpClient())
                        {
                            _logger.LogInformation(LogMessage("2.6 connecting"));
                            mailClient.Connect(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

                            _logger.LogInformation(LogMessage("2.7 Authenticating"));
                            mailClient.Authenticate(smtpUser, smtpPass);

                            _logger.LogInformation(LogMessage("2.6 Sending email"));
                            mailClient.Send(emailMessage);

                            _logger.LogInformation(LogMessage("2.6 Disconnecting"));
                            mailClient.Disconnect(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(LogMessage("2.6 Exception occurs: " + ex.Message ));
                        throw;
                    }


                    return true;
                    /*
                    MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
                    emailMessage.From.Add(emailFrom);
                    MailboxAddress emailTo = new MailboxAddress(mailData.EmailToName, mailData.EmailToId);
                    emailMessage.To.Add(emailTo);

                    emailMessage.Cc.Add(new MailboxAddress("Cc Receiver", "cc@example.com"));
                    emailMessage.Bcc.Add(new MailboxAddress("Bcc Receiver", "bcc@example.com"));

                    emailMessage.Subject = mailData.EmailSubject;

                    BodyBuilder emailBodyBuilder = new BodyBuilder();
                    emailBodyBuilder.TextBody = mailData.EmailBody;

                    emailMessage.Body = emailBodyBuilder.ToMessageBody();
                    this is the SmtpClient from the Mailkit.Net.Smtp namespace, not the System.Net.Mail one
                    using (MailKit.Net.Smtp.SmtpClient mailClient = new MailKit.Net.Smtp.SmtpClient())
                    {
                        mailClient.Connect(_mailSettings.Server, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                        mailClient.Authenticate(_mailSettings.UserName, _mailSettings.Password);
                        mailClient.Send(emailMessage);
                        mailClient.Disconnect(true);
                    }
                    */
                }

                //return true;
            }
            catch (Exception ex)
            {
                 //Exception Details
                return false;
            }
        }
    }
}
