using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.BLL.Interfaces
{
    public interface IAnalyticsService
    {
        Task<Object> GetSummaryAsync(int sensorId, DateTime? from, DateTime? to);
        Task<Object> GetTrendsAsync(int sensorId, string interval, DateTime? from, DateTime? to);
        Task<Object> CompareSensorsAsync(string sensorIds, DateTime? from, DateTime? to);
        Task<Object> DetectAnomaliesAsync(int sensorId, DateTime? from, DateTime? to);
    }
}
