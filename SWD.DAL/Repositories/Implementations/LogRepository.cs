using SWD.DAL.Models;
using SWD.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.DAL.Repositories.Implementations
{
    public class LogRepository : ILogRepository
    {
        private readonly IoTFinalDbContext _context;

        public LogRepository(IoTFinalDbContext context)
        {
            _context = context;
        }

        public async Task AddLogAsync(SystemLog log)
        {
            await _context.SystemLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
