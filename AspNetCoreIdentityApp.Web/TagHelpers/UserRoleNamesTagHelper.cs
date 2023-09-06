using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace AspNetCoreIdentityApp.Web.TagHelpers
{
    public class UserRoleNamesTagHelper : TagHelper
    {
        public string UserID { get; set; }
        private readonly UserManager<AppUser> _userManager;

        public UserRoleNamesTagHelper(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var user = await _userManager.FindByIdAsync(UserID);

            var userRoles = await _userManager.GetRolesAsync(user);

            var stringBuilder = new StringBuilder();

            userRoles.ToList().ForEach(x =>
            {
                stringBuilder.Append(@$"<span class=""badge bg-info"">{x.ToLower()}</span>");
            });

            output.Content.SetHtmlContent(stringBuilder.ToString());
        }
    }
}
