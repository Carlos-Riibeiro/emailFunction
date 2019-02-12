using Microsoft.ApplicationInsights;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Poc.EmailFunction.HttpTrigger.Attribute
{
    public class ExceptionFilterAttribute : FunctionExceptionFilterAttribute
    {
        public string Name { get; set; }

        public override Task OnExceptionAsync(FunctionExceptionContext exceptionContext, CancellationToken cancellationToken)
        {
            var telemetry = new TelemetryClient
            {
                InstrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY")
            };

            var time = DateTime.Now;
            var sw = Stopwatch.StartNew();
            telemetry.Context.Operation.Id = Guid.NewGuid().ToString();

            exceptionContext.Logger.LogError(
                $"Function Exception: '{exceptionContext.FunctionName}" +
                $":{exceptionContext.FunctionInstanceId}" +
                $"Inner Exception: {exceptionContext.Exception.InnerException}" +
                $"Message Exception: {exceptionContext.Exception.Message}");

            telemetry.TrackRequest(this.Name, time, sw.Elapsed, HttpStatusCode.InternalServerError.ToString(), false);

            exceptionContext.Logger.LogError($"Encerrada {this.Name}: {DateTime.Now}");
            telemetry.TrackTrace($"Encerrada {this.Name}: {DateTime.Now}");

            return Task.CompletedTask;
        }
    }
}
