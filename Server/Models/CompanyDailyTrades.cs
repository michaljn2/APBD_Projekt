using System;

namespace BlazorApp1.Server.Models
{
    public class CompanyDailyTrades
    {
        public string IdCompany { get; set; }
        public DateTime DateTime { get; set; }
        public string Json { get; set; }
        public virtual Company Company { get; set; }
    }
}
