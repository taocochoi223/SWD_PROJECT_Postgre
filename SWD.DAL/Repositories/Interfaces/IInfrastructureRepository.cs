using SWD.DAL.Models;

//ALOALO chú ý nè Quí PHÈN: Đây là interface cho repository quản lý hạ tầng (Infrastructure) trong hệ thống SWD.
//Cái này dùng để quản lý Cửa hàng (Site), Hub và User đăng nhập.
namespace SWD.DAL.Repositories.Interfaces
{
    public interface IInfrastructureRepository
    {
        // Quản lý Hub (Gateway)
        Task<Hub?> GetHubByMacAddressAsync(string macAddress); // Để xác thực thiết bị
        Task UpdateHubStatusAsync(Hub hub); // Cập nhật Online/LastHandshake

        // Quản lý Site (Cửa hàng)
        Task<List<Site>> GetAllSitesAsync();
        Task<Site?> GetSiteByIdAsync(int id);

        // Quản lý User (Login)
        Task<User?> GetUserByEmailAsync(string email);

        Task SaveChangesAsync();
    }
}