using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CholoShajiIdentity.Models;
using CholoShajiIdentity.Models.IdentityModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CholoShajiIdentity.DbContexts
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}
