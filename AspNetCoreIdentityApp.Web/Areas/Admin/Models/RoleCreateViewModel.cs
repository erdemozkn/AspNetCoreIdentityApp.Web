using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Models
{
    public class RoleCreateViewModel
    {
        [Display(Name = "Role Name")]
        [Required(ErrorMessage = "Need to type name of role!")]
        public string? Name { get; set; }
    }
}
