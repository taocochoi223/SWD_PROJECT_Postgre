using SWD.DAL.Models;

namespace SWD.BLL.Interfaces
{
    /// <summary>
    /// Service for Site management operations
    /// </summary>
    public interface ISiteService
    {
        Task<List<Site>> GetAllSitesAsync();
        Task<Site?> GetSiteByIdAsync(int siteId);
        Task CreateSiteAsync(Site site);
        Task UpdateSiteAsync(Site site);
        Task DeleteSiteAsync(int siteId);
        Task<List<Site>> GetSiteHierarchyAsync();
        Task<Site?> GetSiteHierarchyByIdAsync(int siteId);
    }
}
