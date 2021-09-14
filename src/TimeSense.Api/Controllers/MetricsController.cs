using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimeSense.Api.Extensions;
using TimeSense.Api.Models;
using TimeSense.Models;
using TimeSense.Repository;

namespace TimeSense.Api.Controllers
{
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly MetricsRepository _repository;
        private readonly ILogger<MetricsController> _logger;

        public MetricsController(
            MetricsRepository repository,
            ILogger<MetricsController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private BadRequestObjectResult BadRequestErrorResponse(string message) => 
            BadRequest(new ErrorResponse(message));
        
        // GET api/sensedTimes/{id}
        [HttpGet]
        public async Task<ActionResult<SensedTime>> Get()
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTime = await _repository.Get(userId);
            
            return Ok(sensedTime);
        }

        // POST api/sensedTimes
        [HttpPost]
        public async Task<ActionResult<SensedTime>> Post([FromBody] IDictionary<string, dynamic> metrics)
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTime = await _repository.Create(userId, metrics);
            
            return Ok(sensedTime);
        }

        // PUT api/sensedTimes
        [HttpPut]
        public async Task<ActionResult<SensedTime>> Put([FromBody] IDictionary<string, dynamic> metrics)
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTime = await _repository.Update(userId, metrics);

            return Ok(sensedTime);
        }

        // DELETE api/sensedTimes/{id}
        [HttpDelete]
        public async Task<ActionResult> Delete()
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            await _repository.Delete(userId);

            return Ok();
        }
    }
}