using SWD.BLL.Interfaces;
using SWD.DAL.Models;
using SWD.DAL.Repositories.Interfaces;

namespace SWD.BLL.Services
{
    public class SiteService : ISiteService
    {
        private readonly ISiteRepository _siteRepo;

        public SiteService(ISiteRepository siteRepo)
        {
            _siteRepo = siteRepo;
        }

        public async Task<List<Site>> GetAllSitesAsync()
        {
            return await _siteRepo.GetAllSitesAsync();
        }

        public async Task<Site?> GetSiteByIdAsync(int siteId)
        {
            return await _siteRepo.GetSiteByIdAsync(siteId);
        }

        public async Task CreateSiteAsync(Site site)
        {
            await _siteRepo.AddSiteAsync(site);
            await _siteRepo.SaveChangesAsync();
        }

        public async Task UpdateSiteAsync(Site site)
        {
            await _siteRepo.UpdateSiteAsync(site);
            await _siteRepo.SaveChangesAsync();
        }

        public async Task DeleteSiteAsync(int siteId)
        {
            await _siteRepo.DeleteSiteAsync(siteId);
            await _siteRepo.SaveChangesAsync();
        }

        public async Task<List<Site>> GetSiteHierarchyAsync()
        {
            return await _siteRepo.GetSiteHierarchyAsync();
        }

        public async Task<Site?> GetSiteHierarchyByIdAsync(int siteId)
        {
            return await _siteRepo.GetSiteHierarchyByIdAsync(siteId);
        }
    }
}
