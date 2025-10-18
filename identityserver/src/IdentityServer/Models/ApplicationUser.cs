using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? FirstName { get; set; }
        [PersonalData]
        public string? LastName { get; set; }
        [PersonalData]
        public byte[]? Picture { get; set; }
    }
}