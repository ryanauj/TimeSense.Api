using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSense.Api.Models;
using TimeSense.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimeSense.Api.Extensions;
using TimeSense.Repository;

namespace TimeSense.Api.Controllers
{
    [Route("api/[controller]")]
    public class SensedTimesController : ControllerBase
    {
        private readonly SensedTimesRepository _repository;
        private readonly ILogger<SensedTimesController> _logger;

        public SensedTimesController(SensedTimesRepository repository, ILogger<SensedTimesController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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
            
            var sensedTimes = await _repository.List(userId);

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
            
            var sensedTimes = _repository.GetLatestSensedTimes(userId, latestToTake);

            return Ok(sensedTimes);
        }

        // GET api/sensedTimes/target
        [HttpGet("target")]
        public ActionResult<IDictionary<decimal, SensedTime>> GetSensedTimesByTargetTime()
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTimesByTargetTime = _repository.GetLatestSensedTimesByTargetTime(userId);

            return Ok(sensedTimesByTargetTime);
        }

        // GET api/sensedTimes/target/latest/{latestToTake}
        [HttpGet("target/latest/{latestToTake}")]
        public ActionResult<IDictionary<decimal, SensedTime>> GetSensedTimesByTargetTimeWithLatest(int latestToTake)
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTimesByTargetTime = _repository.GetLatestSensedTimesByTargetTime(userId, latestToTake);

            return Ok(sensedTimesByTargetTime);
        }

        // GET api/sensedTimes/target/{targetTime}
        [HttpGet("target/{targetTime}")]
        public ActionResult<IDictionary<decimal, SensedTime>> GetSensedTimesForTargetTime(decimal targetTime)
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTimesByTargetTime = _repository.GetLatestSensedTimesForTargetTime(userId, targetTime, null);

            return Ok(sensedTimesByTargetTime);
        }

        // GET api/sensedTimes/target/{targetTime}/latest/{latestToTake}
        [HttpGet("target/{targetTime}/latest/{latestToTake}")]
        public ActionResult<IDictionary<decimal, SensedTime>> GetSensedTimesForTargetTimeWithLatest(decimal targetTime, int latestToTake)
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            var sensedTimesByTargetTime = _repository.GetLatestSensedTimesForTargetTime(userId, targetTime, latestToTake);

            return Ok(sensedTimesByTargetTime);
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
            
            var sensedTime = await _repository.Get(userId, id);
            
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
            
            var sensedTime = await _repository.Create(userId, sensedTimeInput);
            
            return Ok(sensedTime);
        }

        // PUT api/sensedTimes
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(
            string id,
            [FromBody] SensedTimeInput sensedTimeInput
        )
        {
            var userId = HttpContext.GetUserId(_logger);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequestErrorResponse("No user id passed in.");
            }
            
            await _repository.Update(userId, id, sensedTimeInput);
            // TODO: Update metrics on update

            return Ok();
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
            
            await _repository.Delete(userId, id);
            // TODO: Update metrics on remove

            return Ok();
        }
    }
}
