using SWD.DAL.Models;

namespace SWD.BLL.Interfaces
{
    public interface ISystemLogService
    {
        Task LogOptionAsync(string source, string payload, string errorMsg = null);
        Task<List<SystemLog>> GetRecentLogsAsync(int count = 50);
    }
}
