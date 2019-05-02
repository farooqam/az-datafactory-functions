using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HelloWorldFunctionApp
{
	public static class HelloWorldFunction
    {
        [FunctionName("FarooqHelloWorld")]
        public static async Task<JObject> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

	        RequestTelemetry telemetry = new RequestTelemetry();
			TelemetryConfiguration.Active.TelemetryInitializers.OfType<OperationCorrelationTelemetryInitializer>().Single().Initialize(telemetry);

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? JObject.FromObject(new {message = $"Hello {name}!", operationId = telemetry.Context.Operation.Id})
                : JObject.FromObject(new {error = "Specify a name.", operationId = telemetry.Context.Operation.Id});
        }
    }
}
