﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp1.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual IEnumerable<UsersCompanies> UsersCompanies { get; set; }

    }
}
