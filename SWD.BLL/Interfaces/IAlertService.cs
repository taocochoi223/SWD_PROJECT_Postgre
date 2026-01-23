using SWD.DAL.Models;

namespace SWD.BLL.Interfaces
{
    public interface IAlertService
    {
        /// <summary>
        /// Kiểm tra Reading mới có vi phạm luật nào không.
        /// Nếu có -> Tạo AlertHistory + Notification.
        /// </summary>
        Task CheckAndTriggerAlertAsync(Reading reading);

        Task<AlertHistory?> GetAlertByIdAsync(int historyId);
        Task<List<AlertHistory>> GetAlertHistoryAsync(int? sensorId);
        Task<List<AlertHistory>> GetAlertsWithFiltersAsync(string? status, string? search);
        Task ResolveAlertAsync(int historyId);
        Task DeleteAlertAsync(int historyId);

        // Quản lý Rule
        Task<List<AlertRule>> GetAllRulesAsync();
        Task CreateRuleAsync(AlertRule rule);
    }
}
