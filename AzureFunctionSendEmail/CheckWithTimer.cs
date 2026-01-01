////using Microsoft.AspNetCore.Http;
////using Microsoft.AspNetCore.Mvc;
////using AzureFunctionSendEmail;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Extensions.Logging;

//using System;

using System;
using AzureFunctionSendEmail;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
//using Microsoft.Azure.Functions.Worker;


public class TimerFunction
{
    private readonly ILogger<SendEmail> _logger;
    public TimerFunction(ILogger<SendEmail> logger, IMailService mailService)
    {
        _logger = logger;        
    }

    [FunctionName("TimerFunction")]
    //[Function("TimerFunction")]
    public static void Run(
        [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
        ILogger log)
    {
        log.LogInformation($"Timer triggered at: {DateTime.Now}");
    }
}