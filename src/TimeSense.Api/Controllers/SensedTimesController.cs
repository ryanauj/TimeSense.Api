using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.Core;
using TimeSense.Api.Models;
using TimeSense.Models;
using TimeSense.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace TimeSense.Api.Controllers
{
    [Route("api/[controller]")]
    public class SensedTimesController : ControllerBase
    {
        private readonly IRepository<string, string, SensedTimeInput, SensedTime> _repository;
        private readonly ILogger<SensedTimesController> _logger;
        private const string MockCognitoIdentityId = "MOCK_COGNITO_IDENTITY_ID";

        public SensedTimesController(
            IRepository<string, string, SensedTimeInput, SensedTime> repository,
            ILogger<SensedTimesController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private static string GetUserId(HttpContext httpContext)
        {
            var lambdaRequest = httpContext.Items[AbstractAspNetCoreFunction.LAMBDA_REQUEST_OBJECT] as APIGatewayProxyRequest;
            
            Log(lambdaRequest, "Lambda Request");
            var lambdaRequestCognitoId = lambdaRequest?.RequestContext?.Identity?.CognitoIdentityId;
            if (!string.IsNullOrWhiteSpace(lambdaRequestCognitoId))
            {
                Console.WriteLine($"Using lambda request cognito id: '{lambdaRequestCognitoId}'");
                return lambdaRequestCognitoId;
            }
            
            if (lambdaRequest?.Headers?.ContainsKey(MockCognitoIdentityId) ?? false)
            {
                var lambdaRequestMockCognitoId = lambdaRequest.Headers[MockCognitoIdentityId];
                Console.WriteLine($"Using lambda request mock cognito id: '{lambdaRequestMockCognitoId}'");
                return lambdaRequestMockCognitoId;
            }
            
            Log(httpContext.Request, "Context Request");
            if (httpContext?.Request?.Headers?.ContainsKey(MockCognitoIdentityId) ?? false)
            {
                var mockCognitoId = httpContext.Request.Headers[MockCognitoIdentityId];
                Console.WriteLine($"Using headers mock cognito id: '{mockCognitoId}'");
                return mockCognitoId;
            }

            throw new AuthenticationException("UserId not found in request!");
        }

        private static void Log<T>(T objectToLog, string message=null)
        {
            var builder = new StringBuilder(Environment.NewLine);
            if (!string.IsNullOrWhiteSpace(message))
            {
                builder.AppendLine(message);
            }

            builder.AppendLine(JsonConvert.SerializeObject(objectToLog));
            Console.WriteLine(builder.ToString());
        }

        private static void LogLambdaContext(ILambdaContext lambdaContext, string message=null)
        {
            var builder = new StringBuilder(Environment.NewLine);
            if (!string.IsNullOrWhiteSpace(message))
            {
                builder.AppendLine(message);
            }

            builder.AppendLine(JsonConvert.SerializeObject(lambdaContext));
            
            Console.WriteLine(builder.ToString());
        }

        private BadRequestObjectResult BadRequestErrorResponse(string message) => 
            BadRequest(new ErrorResponse("No user id passed in."));

        // GET api/sensedTimes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SensedTime>>> Get()
        {
            var userId = GetUserId(HttpContext);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTimes = await _repository.List(userId);

            return Ok(sensedTimes);
        }
        
        // GET api/sensedTimes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SensedTime>> Get(string id)
        {
            var userId = GetUserId(HttpContext);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTime = await _repository.Get(userId, id);
            
            return Ok(sensedTime);
        }

        // POST api/sensedTimes
        [HttpPost]
        public async Task<ActionResult<IdentifierResponse<string>>> Post([FromBody] SensedTimeInput sensedTimeInput)
        {
            var userId = GetUserId(HttpContext);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var id = await _repository.Create(userId, sensedTimeInput);
            var response = new IdentifierResponse<string> {Id = id};
            
            return Ok(response);
        }

        // PUT api/sensedTimes
        [HttpPut("{id}")]
        public async Task<ActionResult<IdentifierResponse<string>>> Put(
            string id,
            [FromBody] SensedTimeInput sensedTimeInput
        )
        {
            var userId = GetUserId(HttpContext);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            await _repository.Update(userId, id, sensedTimeInput);
            var response = new IdentifierResponse<string> {Id = id};

            return Ok(response);
        }

        // DELETE api/sensedTimes/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var userId = GetUserId(HttpContext);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            await _repository.Delete(userId, id);

            return Ok();
        }
    }
}
