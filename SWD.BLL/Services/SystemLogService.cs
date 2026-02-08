using SWD.BLL.Interfaces;
using SWD.DAL.Models;
using SWD.DAL.Repositories.Interfaces;

namespace SWD.BLL.Services
{
    public class SystemLogService : ISystemLogService
    {
        private readonly ILogRepository _logRepo;

        public SystemLogService(ILogRepository logRepo)
        {
            _logRepo = logRepo;
        }

        public async Task LogOptionAsync(string source, string payload, string errorMsg = null)
        {
            var log = new SystemLog
            {
                Source = source,
                RawPayload = payload,
                ErrorMessage = errorMsg,
                CreatedAt = DateTime.Now
            };
            await _logRepo.AddLogAsync(log);
            await _logRepo.SaveChangesAsync();
        }

        public async Task<List<SystemLog>> GetRecentLogsAsync(int count = 50)
        {
            return await _logRepo.GetRecentLogsAsync(count);
        }
    }
}
