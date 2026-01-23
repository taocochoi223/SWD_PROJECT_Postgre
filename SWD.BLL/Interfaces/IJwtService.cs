using SWD.DAL.Models;

namespace SWD.BLL.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        int? ValidateToken(string token);
    }
}
