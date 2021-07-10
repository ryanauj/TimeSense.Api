using System;
using Amazon.DynamoDBv2;
using TimeSense.Models;
using TimeSense.Repository.Abstractions;
using TimeSense.Serialization;
using Microsoft.AspNetCore.Hosting;

namespace TimeSense.Repository
{
    public class SensedTimesRepository : BaseRepository<SensedTimeInput, SensedTime>
    {
        public SensedTimesRepository(IHostingEnvironment env, IAmazonDynamoDB dynamoDb, ISerializer serializer) :
            base($"sensed-time-table-{env.EnvironmentName}", dynamoDb, serializer)
        {
        }

        protected override SensedTime Build(string userId, string id, SensedTimeInput input) =>
            new SensedTime
            {
                UserId = userId,
                Id = id,
                StartTime = input.StartTime,
                StopTime = input.StopTime,
                TargetTime = input.TargetTime
            };
    }
}
