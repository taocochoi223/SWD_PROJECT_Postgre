using SWD.DAL.Models;

namespace SWD.DAL.Repositories.Interfaces
{
    /// <summary>
    /// Repository quản lý System Log (ghi nhận & truy vấn log)
    /// </summary>
    public interface ILogRepository
    {
        // ================= LOGGING =================
        Task AddLogAsync(SystemLog log);

        /// <summary>
        /// Lấy danh sách log gần nhất (phục vụ debug / audit)
        /// </summary>
        Task<List<SystemLog>> GetRecentLogsAsync(int count);

        // ================= COMMON =================
        Task SaveChangesAsync();
    }
}
