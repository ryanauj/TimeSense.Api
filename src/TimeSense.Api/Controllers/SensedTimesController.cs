using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer;
using TimeSense.Api.Models;
using TimeSense.Models;
using TimeSense.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TimeSense.Api.Controllers
{
    [Route("api/[controller]")]
    public class SensedTimesController : ControllerBase
    {
        private readonly ISensedTimesRepository _repository;
        private readonly ILogger<SensedTimesController> _logger;
        private const string MockCognitoIdentityId = "MOCK_COGNITO_IDENTITY_ID";

        public SensedTimesController(
            ISensedTimesRepository repository,
            ILogger<SensedTimesController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private static string GetUserId(HttpContext httpContext)
        {
            var lambdaRequest = httpContext.Items[AbstractAspNetCoreFunction.LAMBDA_REQUEST_OBJECT] as APIGatewayProxyRequest;
            
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
            
            if (httpContext?.Request?.Headers?.ContainsKey(MockCognitoIdentityId) ?? false)
            {
                var mockCognitoId = httpContext.Request.Headers[MockCognitoIdentityId];
                Console.WriteLine($"Using headers mock cognito id: '{mockCognitoId}'");
                return mockCognitoId;
            }

            throw new AuthenticationException("UserId not found in request!");
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
            
            var sensedTimes = await _repository.ListWithOrder(userId, true, time => time.CreatedAt);

            return Ok(sensedTimes);
        }

        // GET api/sensedTimes/latest/{latestToTake}
        [HttpGet("latest/{latestToTake}")]
        public async Task<ActionResult<IEnumerable<SensedTime>>> GetLatest(int latestToTake)
        {
            var userId = GetUserId(HttpContext);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTimes = await _repository.GetLatestSensedTimes(userId, latestToTake);

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
        public async Task<ActionResult<SensedTime>> Post([FromBody] SensedTimeInput sensedTimeInput)
        {
            var userId = GetUserId(HttpContext);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTime = await _repository.Create(userId, sensedTimeInput);
            
            return Ok(sensedTime);
        }

        // PUT api/sensedTimes
        [HttpPut("{id}")]
        public async Task<ActionResult<SensedTime>> Put(
            string id,
            [FromBody] SensedTimeInput sensedTimeInput
        )
        {
            var userId = GetUserId(HttpContext);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTime = await _repository.Update(userId, id, sensedTimeInput);

            return Ok(sensedTime);
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
