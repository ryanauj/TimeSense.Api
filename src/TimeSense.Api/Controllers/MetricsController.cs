using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimeSense.Api.Extensions;
using TimeSense.Api.Models;
using TimeSense.Mapping;
using TimeSense.Metrics;
using TimeSense.Models;
using TimeSense.Repository;

namespace TimeSense.Api.Controllers
{
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly MetricsRepository _metricsRepository;
        private readonly ILogger<MetricsController> _logger;

        public MetricsController(
            SensedTimesRepository sensedTimesRepository,
            MetricsRepository metricsRepository,
            MetricsInputMapper mapper,
            ILogger<MetricsController> logger)
        {
            _metricsRepository = metricsRepository ?? throw new ArgumentNullException(nameof(metricsRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private BadRequestObjectResult BadRequestErrorResponse(string message) => 
            BadRequest(new ErrorResponse(message));
        
        // GET api/metrics
        [HttpGet]
        public async Task<ActionResult<MetricsEntity>> Get()
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var metrics = await _metricsRepository.Get(userId);
            
            return Ok(metrics);
        }

        // POST api/metrics
        [HttpPost]
        public ActionResult RecalculateMetrics()
        {
            return Ok();
        }

        // PUT api/metrics
        [Obsolete]
        [HttpPut]
        public ActionResult Put([FromBody] IDictionary<string, Metric> controllerInput)
        {
            return Ok();
        }

        // DELETE api/metrics
        [Obsolete]
        [HttpDelete]
        public ActionResult Delete()
        {
            return Ok();
        }
    }
}