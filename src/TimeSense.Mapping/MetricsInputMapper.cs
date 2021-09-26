using System.Collections.Generic;
using System.Linq;
using TimeSense.Models;

namespace TimeSense.Mapping
{
    public class MetricsInputMapper
    {
        public IDictionary<int, Metric> Map(IDictionary<string, Metric> controllerInput) =>
            controllerInput.ToDictionary(
                kvp => int.Parse(kvp.Key),
                kvp => kvp.Value);
    }
}