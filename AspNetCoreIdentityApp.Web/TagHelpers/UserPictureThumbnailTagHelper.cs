using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCoreIdentityApp.Web.TagHelpers
{
    public class UserPictureThumbnailTagHelper : TagHelper
    {
        public string? PictureUrlTag { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";

            if (string.IsNullOrWhiteSpace(PictureUrlTag))
            {
                output.Attributes.SetAttribute("src", $"/userpictures/default.jpg");
            }
            else
            {
                output.Attributes.SetAttribute("src", $"/userpictures/{PictureUrlTag}");
            }
        }
    }
}
