using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class AppRole : IdentityRole
    {
        public AppRole()
        {
        }
        public AppRole(string roleName) : base(roleName)
        {
        }

        public AppRole(string roleName, string displayName)
        {
            this.Name = roleName;
            this.DisplayName = displayName;
        }

        //public string Id { get; set; }
        public string DisplayName { get; set; }

    }
}
