using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.ClaimProviders
{
    public class UserClaimProvider : IClaimsTransformation
    {
        private readonly UserManager<AppUser> _userManager;

        public UserClaimProvider(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identityuser = principal.Identity as ClaimsIdentity;

            var user = await _userManager.FindByNameAsync(identityuser.Name);

            
            if(string.IsNullOrEmpty(user.City))
            {
                return principal;
            }

            if(principal.HasClaim(x => x.Type != "city"))
            {
                Claim cityClaim = new Claim("city", user.City);
                identityuser.AddClaim(cityClaim);
            }
            return principal;

        }
    }
}
