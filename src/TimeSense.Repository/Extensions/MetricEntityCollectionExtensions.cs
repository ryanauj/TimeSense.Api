using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TimeSense.Models;

namespace TimeSense.Repository.Extensions
{
    public static class MetricEntityCollectionExtensions
    {
        public static async Task<IEnumerable<decimal>> GetUserDistinctTargetTimes(
            this IMongoCollection<MetricEntity> metricEntityCollection, 
            string userId
        )
        {
            var distinctTargetTimesForUserCursor = await metricEntityCollection.DistinctAsync(
                m => m.TargetTime, m => m.Id.UserId == userId);
            return distinctTargetTimesForUserCursor.ToEnumerable();
        }
    }
}