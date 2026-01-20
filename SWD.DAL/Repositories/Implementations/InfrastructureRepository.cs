using Microsoft.EntityFrameworkCore;
using SWD.DAL.Models;
using SWD.DAL.Repositories.Interfaces;

//ALOALO chú ýnè Sơn GÀ: đoạn này dùng để thao tác với cơ sở dữ liệu liên quan đến hạ tầng (Infrastructure) như Hub, Site và User.
//Cái này dùng để quản lý Cửa hàng (Site), Hub và User đăng nhập.

namespace SWD.DAL.Repositories.Implementations
{
    public class InfrastructureRepository : IInfrastructureRepository
    {
        private readonly IoTFinalDbContext _context;

        public InfrastructureRepository(IoTFinalDbContext context)
        {
            _context = context;
        }

        // --- HUB MANAGEMENT ---
        public async Task<Hub?> GetHubByMacAddressAsync(string macAddress)
        {
            return await _context.Hubs.FirstOrDefaultAsync(h => h.MacAddress == macAddress);
        }

        public async Task UpdateHubStatusAsync(Hub hub)
        {
            _context.Hubs.Update(hub);
            await Task.CompletedTask;
        }

        // --- SITE MANAGEMENT ---
        public async Task<List<Site>> GetAllSitesAsync()
        {
            return await _context.Sites.Include(s => s.Org).ToListAsync();
        }

        public async Task<Site?> GetSiteByIdAsync(int id)
        {
            var item = await _context.Sites
                .Include(s => s.Hubs)
                .ThenInclude(h => h.Sensors)
                .FirstOrDefaultAsync(s => s.SiteId == id);
            return item;
        }

        // --- USER AUTH ---
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            // Tìm user theo email để Login
            return await _context.Users
                .Include(u => u.Site) // Lấy kèm Site để biết nó là Staff cửa hàng nào
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}