using Amazon.DynamoDBv2;
using Entries.Models;
using Entries.Repository.Abstractions;
using Entries.Serialization;
using Microsoft.AspNetCore.Hosting;

namespace Entries.Repository
{
    public class TemplatesRepository : BaseRepository<TemplateProperty, Template>
    {
        public TemplatesRepository(IHostingEnvironment env, IAmazonDynamoDB dynamoDb, ISerializer serializer) :
            base($"template-table-{env.EnvironmentName}", dynamoDb, serializer)
        {
        }
        
        protected override Template Build(string userId, string id, TemplateProperty input) => new Template
        {
            UserId = userId,
            Id = id,
            Name = input.Name,
            Version = input.Version
        };
    }
}