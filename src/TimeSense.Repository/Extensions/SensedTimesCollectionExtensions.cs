using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TimeSense.Models;

namespace TimeSense.Repository.Extensions
{
    public static class SensedTimesCollectionExtensions
    {
        public static async Task<IEnumerable<decimal>> GetUserDistinctTargetTimes(
            this IMongoCollection<SensedTime> sensedTimesCollection, 
            string userId
        )
        {
            var distinctTargetTimesForUserCursor = await sensedTimesCollection.DistinctAsync(
                st => st.TargetTime, st => st.UserId == userId);
            return distinctTargetTimesForUserCursor.ToEnumerable();
        }
    }
}