using BlazorApp1.Server.Models;
using BlazorApp1.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp1.Server.Services
{
    public interface ICompanyService
    {
        public Task<List<CompanyName>> GetCompaniesByName(string ticker);

        public Task<bool> DoesUserExist (string userId);

        public Task<bool> DoesCompanyExist(string companyId);
        public Task<Company> GetCompany(string companyId);
        public Task<ApplicationUser> GetApplicationUser(string userId);
        public Task SaveDatabase();

        public Task<CompanyDetails> GetCompanyDetails(string ticker);
        public Task AddCompany(CompanyDetails company);
        public Task<bool> DoesCompanyExistInDb(string ticker);
        public Task<CompanyDetails> GetCompanyDetailsFromDb(string ticker);
        public Task<StockChartsGet> GetDailyStockCharts(string ticker);

        public Task<StockChartsGet> GetStockCharts(string ticker, string start, string end);
       
    }
}
