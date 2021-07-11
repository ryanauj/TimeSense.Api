using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.Core;
using TimeSense.Api.Models;
using TimeSense.Models;
using TimeSense.Repository;
using TimeSense.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            if (httpContext.Request.Headers.ContainsKey(MockCognitoIdentityId))
            {
                return httpContext.Request.Headers[MockCognitoIdentityId];
            }
            
            var lambdaContext = httpContext.Items[AbstractAspNetCoreFunction.LAMBDA_CONTEXT] as ILambdaContext;
            return lambdaContext.Identity.IdentityId;
        }

        // GET api/sensedTimes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SensedTime>>> Get()
        {
            var userId = GetUserId(HttpContext);
            var sensedTimes = await _repository.List(userId);

            return Ok(sensedTimes);
        }
        
        // GET api/sensedTimes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SensedTime>> Get(string id)
        {
            var userId = GetUserId(HttpContext);
            var sensedTime = await _repository.Get(userId, id);
            
            return Ok(sensedTime);
        }

        // POST api/sensedTimes
        [HttpPost]
        public async Task<ActionResult<IdentifierResponse<string>>> Post([FromBody] SensedTimeInput sensedTimeInput)
        {
            var userId = GetUserId(HttpContext);
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
            await _repository.Update(userId, id, sensedTimeInput);
            var response = new IdentifierResponse<string> {Id = id};

            return Ok(response);
        }

        // DELETE api/sensedTimes/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var userId = GetUserId(HttpContext);
            await _repository.Delete(userId, id);

            return Ok();
        }
    }
}
