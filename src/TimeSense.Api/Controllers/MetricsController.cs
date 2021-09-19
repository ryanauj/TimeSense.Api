using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimeSense.Api.Extensions;
using TimeSense.Api.Models;
using TimeSense.Mapping;
using TimeSense.Models;
using TimeSense.Repository;

namespace TimeSense.Api.Controllers
{
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly MetricsRepository _repository;
        private readonly MetricsInputMapper _mapper;
        private readonly ILogger<MetricsController> _logger;

        public MetricsController(
            MetricsRepository repository,
            MetricsInputMapper mapper,
            ILogger<MetricsController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private BadRequestObjectResult BadRequestErrorResponse(string message) => 
            BadRequest(new ErrorResponse(message));
        
        // GET api/metrics
        [HttpGet]
        public async Task<ActionResult<Metrics>> Get()
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var metrics = await _repository.Get(userId);
            
            return Ok(metrics);
        }

        // POST api/metrics
        [HttpPost]
        public async Task<ActionResult<Metrics>> Post([FromBody] MetricsControllerInput controllerInput)
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }

            var repositoryInput = _mapper.Map(controllerInput);
            
            var metrics = await _repository.Create(userId, userId, repositoryInput);
            
            return Ok(metrics);
        }

        // PUT api/metrics
        [HttpPut]
        public async Task<ActionResult<Metrics>> Put([FromBody] MetricsControllerInput controllerInput)
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var repositoryInput = _mapper.Map(controllerInput);
            
            var metrics = await _repository.Update(userId, repositoryInput);

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
            
            await _repository.Delete(userId);

            return Ok();
        }
    }
}