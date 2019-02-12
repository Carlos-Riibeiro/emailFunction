using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Http;
using Poc.EmailFunction.HttpTrigger.Models;
using Poc.EmailFunction.HttpTrigger.Attribute;

namespace Poc.EmailFunction.HttpTrigger
{
    public static class EmailFunction
    {
        [FunctionName("SendEmailHttpTriggerFunction")]
        [ExceptionFilter(Name = "SendEmailHttpTriggerFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/email")]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation($"SendEmailHttpTriggerFunction executada: {DateTime.Now}");

            var data = JsonConvert.DeserializeObject<EmailDetail>(await req.Content.ReadAsStringAsync());

            if (data?.Tos == null || data?.Subject == null || data?.From == null || data?.Body == null)
                return new BadRequestObjectResult(HttpStatusCode.BadRequest);

            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_KEY");
            var client = new SendGridClient(apiKey);

            SendGridMessage message = new SendGridMessage();
            message.AddTos(data.Tos);
            message.AddContent("text/html", data.Body);
            message.SetFrom(new EmailAddress(data.From));
            message.SetSubject(data.Subject);

            var response = await client.SendEmailAsync(message);

            log.LogInformation($"SendEmailHttpTriggerFunction finalizada: {DateTime.Now}");

            return response.StatusCode == HttpStatusCode.Accepted
                ? (ActionResult)new OkObjectResult(HttpStatusCode.OK)
                : new BadRequestObjectResult(response.StatusCode);
        }
    }
}
