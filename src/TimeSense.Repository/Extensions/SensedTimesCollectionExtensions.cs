using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TimeSense.Models;

namespace TimeSense.Repository.Extensions
{
    public static class SensedTimesCollectionExtensions
    {
        public static async Task<IEnumerable<string>> GetDistinctUserIds(
            this IMongoCollection<SensedTime> sensedTimesCollection
        )
        {
            // var allSensedTimes = sensedTimesCollection.Find(s => true);
            // var sensedTimes = allSensedTimes.ToEnumerable();
            // var users = new HashSet<string>();
            // foreach (var sensedTime in sensedTimes)
            // {
            //     users.Add(sensedTime.UserId);
            // }
            //
            // return users;
            var distinctTargetTimesForUserCursor = await sensedTimesCollection.DistinctAsync(
                st => st.UserId, st => true);
            return distinctTargetTimesForUserCursor.ToEnumerable();
        }
        
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