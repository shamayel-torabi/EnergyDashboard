using Duende.IdentityServer.EntityFramework.Options;
using IdentityServer.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IdentityServer.Data
{
    public class IdentityDbContext : ApiAuthorizationDbContext<ApplicationUser>, IDataProtectionKeyContext
    {
        public IdentityDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {

        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}