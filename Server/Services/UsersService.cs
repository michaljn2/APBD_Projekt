using BlazorApp1.Server.Data;
using BlazorApp1.Server.Models;
using BlazorApp1.Shared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp1.Server.Services
{
    public class UsersService : IUsersService
    {
        private readonly ApplicationDbContext _context;
        public UsersService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddCompanyToWatchList(string userId, string companyId)
        {
            await _context.UsersCompanies.AddAsync(new UsersCompanies
            {
                IdCompany = companyId,
                IdUser = userId
            });
        }

        public async Task DeleteCompanyFromWatchList(string companyId, string userId)
        {
            var entry = await _context.UsersCompanies.FirstOrDefaultAsync(e => e.IdCompany == companyId && e.IdUser == userId);
            _context.Entry(entry).State = EntityState.Deleted;
            
        }

        public async Task<List<CompanyDetails>> GetUsersWatchList(string userId)
        {
            return await _context.UsersCompanies.Where(e => e.IdUser == userId).Include(e => e.Company).Select(e => new CompanyDetails
            {
                IdCompany = e.Company.IdCompany,
                Name = e.Company.Name,
                Logo = e.Company.Logo,
                City = e.Company.City,
                Description = e.Company.Description
            }).ToListAsync();
        }
        public async Task SaveDatabase()
        {
           await _context.SaveChangesAsync();
        }
    }
}
