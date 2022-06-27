using BlazorApp1.Server.Models;
using BlazorApp1.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp1.Server.Services
{
    public interface IUsersService
    {
        public Task AddCompanyToWatchList(string userId, string companyId);
        public Task SaveDatabase();
        public Task<List<CompanyDetails>> GetUsersWatchList(string userId);
        public Task DeleteCompanyFromWatchList(string companyId, string userId);
    }
}
