using System.Linq;
using TimeSense.Models;

namespace TimeSense.Mapping
{
    public class MetricsInputMapper
    {
        public MetricsRepositoryInput Map(MetricsControllerInput controllerInput)
        {
            var averages = controllerInput.Averages.ToDictionary(
                kvp => int.Parse(kvp.Key),
                kvp => kvp.Value);
            
            return new MetricsRepositoryInput
            {
                Averages = averages
            };
        }
    }
}