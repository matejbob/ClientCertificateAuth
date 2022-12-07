using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ClientCertificateAuth
{
	public class Functions
	{
		private readonly ILogger _logger;

		public Functions(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<Functions>();
		}

		[AnonymousAccess]
		[Function("PublicFunction")]
		public HttpResponseData RunPublicFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
		{
			_logger.LogInformation($"{nameof(RunPublicFunction)} invoked");

			var response = req.CreateResponse(HttpStatusCode.OK);

			response.WriteString(System.Text.Json.JsonSerializer.Serialize(new { Msg = "Public Azure Function called" }));

			return response;
		}

		[Function("SecuredFunction")]
		public HttpResponseData RunSecuredFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
		{
			_logger.LogInformation($"{nameof(RunSecuredFunction)} invoked");

			var response = req.CreateResponse(HttpStatusCode.OK);

			response.WriteString(System.Text.Json.JsonSerializer.Serialize(new { Msg = "Secured Azure Function called" }));

			return response;
		}
	}
}