using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSense.Models;

namespace TimeSense.Repository.Interfaces
{
    public interface ISensedTimesRepository : IRepository<string, string, SensedTimeInput, SensedTime>
    {
        Task<IEnumerable<SensedTime>> GetLatestSensedTimes(string userId, int numToRetrieve);
    }
}