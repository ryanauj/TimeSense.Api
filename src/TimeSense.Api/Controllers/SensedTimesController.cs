using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeSense.Api.Models;
using TimeSense.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimeSense.Api.Extensions;
using TimeSense.Repository;
using TimeSense.Repository.Extensions;

namespace TimeSense.Api.Controllers
{
    [Route("api/[controller]")]
    public class SensedTimesController : ControllerBase
    {
        private readonly SensedTimesRepository _sensedTimes;
        private readonly MetricsRepository _metrics;
        private readonly ILogger<SensedTimesController> _logger;

        public SensedTimesController(
            SensedTimesRepository sensedTimesRepository,
            MetricsRepository metricsRepository,
            ILogger<SensedTimesController> logger)
        {
            _sensedTimes = sensedTimesRepository ?? throw new ArgumentNullException(nameof(sensedTimesRepository));
            _metrics = metricsRepository ?? throw new ArgumentNullException(nameof(metricsRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private BadRequestObjectResult BadRequestErrorResponse(string message) => 
            BadRequest(new ErrorResponse(message));

        // GET api/sensedTimes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SensedTime>>> Get()
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTimes = await _sensedTimes.List(userId);

            return Ok(sensedTimes);
        }

        // GET api/sensedTimes/latest/{latestToTake}
        [HttpGet("latest/{latestToTake}")]
        public ActionResult<IEnumerable<SensedTime>> GetLatest(int latestToTake)
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTimes = _sensedTimes.GetLatestSensedTimes(userId, latestToTake);

            return Ok(sensedTimes);
        }

        // GET api/sensedTimes/target
        [HttpGet("target")]
        public ActionResult<IDictionary<decimal, SensedTimesAndMetrics>> GetSensedTimesByTargetTime()
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTimesByTargetTime = _sensedTimes.GetLatestSensedTimesByTargetTime(userId);
            var sensedTimesAndMetricsByTargetTime = sensedTimesByTargetTime
                .Select(kvp =>
                {
                    var (targetTime, sensedTimes) = kvp;
                    var sensedTimesAndMetrics = new SensedTimesAndMetrics
                    {
                        Metrics = sensedTimes.CalculateMetricsForTargetTime(),
                        SensedTimes = sensedTimes
                    };
                    return new KeyValuePair<decimal, SensedTimesAndMetrics>(targetTime, sensedTimesAndMetrics);
                })
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return Ok(sensedTimesAndMetricsByTargetTime);
        }

        // GET api/sensedTimes/target/latest/{latestToTake}
        [HttpGet("target/latest/{latestToTake}")]
        public ActionResult<IDictionary<decimal, SensedTimesAndMetrics>> GetSensedTimesByTargetTimeWithLatest(int latestToTake)
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTimesByTargetTime = _sensedTimes.GetLatestSensedTimesByTargetTime(userId);
            var sensedTimesAndMetricsByTargetTime = sensedTimesByTargetTime
                .Select(kvp =>
                {
                    var (targetTime, sensedTimes) = kvp;
                    var sensedTimesAndMetrics = new SensedTimesAndMetrics
                    {
                        Metrics = sensedTimes.CalculateMetricsForTargetTime(),
                        SensedTimes = sensedTimes.Take(latestToTake)
                    };
                    return new KeyValuePair<decimal, SensedTimesAndMetrics>(targetTime, sensedTimesAndMetrics);
                })
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return Ok(sensedTimesAndMetricsByTargetTime);
        }

        // GET api/sensedTimes/target/{targetTime}
        [HttpGet("target/{targetTime}")]
        public ActionResult<SensedTimesAndMetrics> GetSensedTimesForTargetTime(decimal targetTime)
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTimesByTargetTime = _sensedTimes.GetLatestSensedTimesForTargetTime(userId, targetTime).ToList();
            var sensedTimesAndMetricsForTargetTime = new SensedTimesAndMetrics
            {
                Metrics = sensedTimesByTargetTime.CalculateMetricsForTargetTime(),
                SensedTimes = sensedTimesByTargetTime
            };

            return Ok(sensedTimesAndMetricsForTargetTime);
        }

        // GET api/sensedTimes/target/{targetTime}/latest/{latestToTake}
        [HttpGet("target/{targetTime}/latest/{latestToTake}")]
        public ActionResult<SensedTimesAndMetrics> GetSensedTimesForTargetTimeWithLatest(decimal targetTime, int latestToTake)
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTimesByTargetTime = _sensedTimes.GetLatestSensedTimesForTargetTime(userId, targetTime, latestToTake).ToList();
            var sensedTimesAndMetricsForTargetTime = new SensedTimesAndMetrics
            {
                Metrics = sensedTimesByTargetTime.CalculateMetricsForTargetTime(),
                SensedTimes = sensedTimesByTargetTime.Take(latestToTake)
            };

            return Ok(sensedTimesAndMetricsForTargetTime);
        }
        
        // GET api/sensedTimes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SensedTime>> Get(string id)
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTime = await _sensedTimes.Get(userId, id);
            
            return Ok(sensedTime);
        }

        // POST api/sensedTimes
        [HttpPost]
        public async Task<ActionResult<SensedTime>> Post([FromBody] SensedTimeInput sensedTimeInput)
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTime = await _sensedTimes.Create(userId, sensedTimeInput);
            await _metrics.Update(userId, sensedTime.TargetTime);

            return Ok(sensedTime);
        }

        // DELETE api/sensedTimes/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTime = await _sensedTimes.Get(userId, id);
            
            await _sensedTimes.Delete(userId, id);
            await _metrics.Update(userId, sensedTime.TargetTime);

            return Ok();
        }
    }
}
