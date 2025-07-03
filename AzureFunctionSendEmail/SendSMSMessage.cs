using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Communication.Sms;

namespace AzureFunctionSendEmail
{
    public class SendSMSMessage
    {
        private readonly ILogger<SendEmail> _logger;
        private readonly IMailService _mailService;

        public SendSMSMessage(ILogger<SendEmail> logger, IMailService mailService)
        {
            _logger = logger;
            _mailService = mailService;
        }

        [Function("SendSMSMessage")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {

            _logger.LogInformation($"Timer function executed at: {DateTime.Now}");                     
            
            try
            {                
                string connectionString = "YOUR_ACS_CONNECTION_STRING";
                SmsClient smsClient = new SmsClient(connectionString);

                SmsSendResult result = smsClient.Send(
                    from: "+1YOUR_ACS_PHONE_NUMBER",   // Azure registered phone
                    to: "+1TARGET_PHONE_NUMBER",       // Recipient
                    message: "Hello from Azure SMS via .NET!"
                );


                _logger.LogInformation($"SMS sent at: {DateTime.Now}");
                
                return new OkObjectResult("Send SMS success!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Send SMS message error: {ex.Message}");
                return new StatusCodeResult(500);
            }

            _logger.LogInformation($"Timer function ended at: {DateTime.Now}");
        }

    }
}
