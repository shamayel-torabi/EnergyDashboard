using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Duende.IdentityServer.Services;
using IdentityServer.Models;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Extensions;

namespace IdentityServer.Services
{
    public class IdentityClaimsProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityClaimsProfileService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject;
            if (subject == null) throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(subjectId);
            if (user == null)
                throw new ArgumentException("Invalid subject identifier");

            var claims = await GetClaimsFromUser(user);

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null && user.EmailConfirmed;
        }

        private async Task<List<Claim>> GetClaimsFromUser(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName)
            };

            if (_userManager.SupportsUserEmail)
            {
                claims.AddRange(new[]
                {
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
            });
            }

            if (_userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                claims.AddRange(new[]
                {
                new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
                new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
            });
            }

            if (_userManager.SupportsUserClaim)
            {
                claims.AddRange(await _userManager.GetClaimsAsync(user));
            }

            if (_userManager.SupportsUserRole)
            {
                var roles = await _userManager.GetRolesAsync(user);
                claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));
            }

            if (user.FirstName != null && user.LastName != null)
            {
                claims.Add(new Claim(JwtClaimTypes.Name, user.FirstName + " " + user.LastName));
                claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
                claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
            }
            else
                claims.Add(new Claim(JwtClaimTypes.Name, user.UserName));

            if (user.Picture != null)
            {
                string picture = Convert.ToBase64String(user.Picture);
                claims.Add(new Claim(JwtClaimTypes.Picture, picture));
            }

            return claims;
        }
    }
}
