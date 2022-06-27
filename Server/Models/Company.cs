using System.Collections.Generic;

namespace BlazorApp1.Server.Models
{
    public class Company
    {
        public string IdCompany { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Logo { get; set; }
        public string Description { get; set; }
        public virtual IEnumerable<UsersCompanies> UsersCompanies { get; set; }
        public virtual IEnumerable<CompanyDailyTrades> CompanyDailyTrades { get; set; }
    }
}
