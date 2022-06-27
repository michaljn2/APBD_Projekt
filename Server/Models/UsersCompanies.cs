using BlazorApp1.Server.Models;

namespace BlazorApp1.Server.Models
{
    public class UsersCompanies
    {
        public string IdUser { get; set; }
        public string IdCompany { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Company Company { get; set; }
    }
}
