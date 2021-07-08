using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeSense.Models;
using TimeSense.Repository.Exceptions;
using TimeSense.Repository.Interfaces;

namespace TimeSense.Repository.Local
{
    public class LocalTimeSenseRepository : IRepository<string, string, SensedTimeInput, SensedTime>
    {
        private static IList<SensedTime> _sensedTimes { get; } = new List<SensedTime>();

        private static void RemoveSensedTime(string userId, string id)
        {
            var sensedTime = _sensedTimes.FirstOrDefault(sensedTime => sensedTime.Id == id);
            if (sensedTime != null)
            {
                _sensedTimes.Remove(sensedTime);
            }
        }
        
        public Task<SensedTime> Get(string userId, string id) => Task.FromResult(_sensedTimes.FirstOrDefault(sensedTime => sensedTime.Id == id));

        public Task<string> Create(string userId, SensedTimeInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var sensedTime = new SensedTime(userId, Guid.NewGuid().ToString(), input);
            _sensedTimes.Add(sensedTime);

            return Task.FromResult(sensedTime.Id);
        }

        public Task Update(SensedTime input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (_sensedTimes == null || _sensedTimes.All(sensedTime => sensedTime.Id != input.Id))
            {
                throw new RepositoryException($"No sensedTime exists with id '{input.Id}'");
            }

            RemoveSensedTime(input.UserId, input.Id);
            _sensedTimes.Add(input);

            return Task.CompletedTask;
        }

        public Task Delete(string userId, string id)
        {
            RemoveSensedTime(userId, id);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<SensedTime>> List(string userId)
        {
            return Task.FromResult(_sensedTimes.AsEnumerable());
        }
    }
}
