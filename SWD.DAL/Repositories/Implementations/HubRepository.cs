using Microsoft.EntityFrameworkCore;
using SWD.DAL.Models;
using SWD.DAL.Repositories.Interfaces;

namespace SWD.DAL.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation for Hub/Gateway management
    /// </summary>
    public class HubRepository : IHubRepository
    {
        private readonly IoTFinalDbContext _context;

        public HubRepository(IoTFinalDbContext context)
        {
            _context = context;
        }

        public async Task<Hub?> GetHubByMacAddressAsync(string macAddress)
        {
            return await _context.Hubs
                .Include(h => h.Site)
                .FirstOrDefaultAsync(h => h.MacAddress == macAddress);
        }

        public async Task<Hub?> GetHubByIdAsync(int hubId)
        {
            return await _context.Hubs
                .Include(h => h.Site)
                .Include(h => h.Sensors)
                .FirstOrDefaultAsync(h => h.HubId == hubId);
        }
        public async Task<List<Hub>> GetAllHubsAsync()
        {
            var hubs = await _context.Hubs
                .Include(s => s.Sensors)
                .Include(s => s.Site)
                .ToListAsync();
            return hubs;
        }
        public async Task<List<Hub>> GetHubsBySiteIdAsync(int siteId)
        {
            var hubs = await _context.Hubs
                .Where(h => h.SiteId == siteId)
                .Include(s => s.Sensors)
                .Include(s => s.Site)
                .ToListAsync();
            return hubs;
        }
        public async Task AddHubAsync(Hub hub)
        {
            await _context.Hubs.AddAsync(hub);
        }

        public async Task DeleteHubAsync(int hubId)
        {
            var hub = await _context.Hubs.FindAsync(hubId);
            if (hub != null)
            {
                _context.Hubs.Remove(hub);
            }
        }

        public Task UpdateHubAsync(Hub hub)
        {
            _context.Hubs.Update(hub);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }



    }
}
