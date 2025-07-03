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
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace AzureFunctionSendEmail
{
    public class CheckConnection
    {
        private readonly ILogger<SendEmail> _logger;
        private readonly IMailService _mailService;

        public CheckConnection(ILogger<SendEmail> logger, IMailService mailService)
        {
            _logger = logger;
            _mailService = mailService;
        }

        [Function("CheckConnection")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {

            _logger.LogInformation($"Timer function executed at: {DateTime.Now}");

            #region DATABASE CONNECTION

            // Get connection string from Azure App Settings
            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");

            if (string.IsNullOrEmpty(connectionString))
            {
                return new BadRequestObjectResult("SQL connection string is not configured.");
            }

            string queryResult;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var command = new SqlCommand("SELECT TOP 1 Name FROM Users", conn);
                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        queryResult = reader.GetString(0);
                    }
                    else
                    {
                        queryResult = "No data found.";
                    }
                }

                return new OkObjectResult($"Top user: {queryResult}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Database error: {ex.Message}");
                return new StatusCodeResult(500);
            }

            #endregion

        }     
       
    }
 
}
