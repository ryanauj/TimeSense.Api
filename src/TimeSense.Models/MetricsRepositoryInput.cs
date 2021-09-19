using System.Collections.Generic;

namespace TimeSense.Models
{
    public class MetricsRepositoryInput
    {
        public IDictionary<int, decimal> Averages { get; set; }
    }
}