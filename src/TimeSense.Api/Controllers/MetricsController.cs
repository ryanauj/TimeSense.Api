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
        private readonly SensedTimesRepository _sensedTimesRepository;
        private readonly MetricsRepository _metricsRepository;
        private readonly MetricsProcessor _metricsProcessor;
        private readonly MetricsInputMapper _mapper;
        private readonly ILogger<MetricsController> _logger;

        public MetricsController(
            SensedTimesRepository sensedTimesRepository,
            MetricsRepository metricsRepository,
            MetricsProcessor metricsProcessor,
            MetricsInputMapper mapper,
            ILogger<MetricsController> logger)
        {
            _sensedTimesRepository = sensedTimesRepository ??
                                     throw new ArgumentNullException(nameof(sensedTimesRepository));
            _metricsRepository = metricsRepository ?? throw new ArgumentNullException(nameof(metricsRepository));
            _metricsProcessor = metricsProcessor ?? throw new ArgumentNullException(nameof(metricsProcessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
        public async Task<ActionResult<MetricsEntity>> RecalculateMetrics()
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            

            var allSensedTimes = await _sensedTimesRepository.List(userId);
            var sensedTimesByTargetValue = new Dictionary<int, IList<SensedTime>>();
            foreach (var sensedTime in allSensedTimes)
            {
                var targetTime = decimal.ToInt32(sensedTime.TargetTime);
                if (!sensedTimesByTargetValue.ContainsKey(targetTime))
                {
                    sensedTimesByTargetValue[targetTime] = new List<SensedTime>();
                }
                
                sensedTimesByTargetValue[targetTime].Add(sensedTime);
            }

            await _metricsRepository.Delete(userId);
            await _metricsProcessor.AddMetrics(userId, sensedTimesByTargetValue);
            
            var metrics = await _metricsRepository.Get(userId);
            
            return Ok(metrics);
        }

        // PUT api/metrics
        [HttpPut]
        public async Task<ActionResult<MetricsEntity>> Put([FromBody] IDictionary<string, Metric> controllerInput)
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var repositoryInput = _mapper.Map(controllerInput);
            
            var metrics = await _metricsRepository.Update(userId, repositoryInput);

            return Ok(metrics);
        }

        // DELETE api/metrics
        [HttpDelete]
        public async Task<ActionResult> Delete()
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            await _metricsRepository.Delete(userId);

            return Ok();
        }
    }
}