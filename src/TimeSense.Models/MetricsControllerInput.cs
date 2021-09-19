using System;
using System.Collections.Generic;

namespace TimeSense.Models
{
    [Serializable]
    public class MetricsControllerInput
    {
        
        public IDictionary<string, decimal> Averages { get; set; }
    }
}