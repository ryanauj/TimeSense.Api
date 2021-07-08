using System;
using Amazon.DynamoDBv2;
using TimeSense.Models;
using TimeSense.Repository.Abstractions;
using TimeSense.Serialization;
using Microsoft.AspNetCore.Hosting;

namespace TimeSense.Repository
{
    public class TimeSenseRepository : BaseRepository<SensedTimeInput, SensedTime>
    {
        public TimeSenseRepository(IHostingEnvironment env, IAmazonDynamoDB dynamoDb, ISerializer serializer) :
            base($"sensedTime-table-{env.EnvironmentName}", dynamoDb, serializer)
        {
        }
        
        protected override SensedTime Build(string userId, string id, SensedTimeInput input) => new SensedTime(userId, id, input);
    }
}
