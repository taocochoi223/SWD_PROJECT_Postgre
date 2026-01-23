using Microsoft.EntityFrameworkCore;
using SWD.DAL.Models;
using SWD.DAL.Repositories.Interfaces;

namespace SWD.DAL.Repositories.Implementations
{
    /// <summary>
    /// Repository quản lý System Log
    /// </summary>
    public class LogRepository : ILogRepository
    {
        private readonly IoTFinalDbContext _context;

        public LogRepository(IoTFinalDbContext context)
        {
            _context = context;
        }

        // ================= LOG WRITE =================
        public async Task AddLogAsync(SystemLog log)
        {
            await _context.SystemLogs.AddAsync(log);
        }

        // ================= LOG READ =================
        public async Task<List<SystemLog>> GetRecentLogsAsync(int count)
        {
            return await _context.SystemLogs
                .OrderByDescending(l => l.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        // ================= COMMON =================
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
