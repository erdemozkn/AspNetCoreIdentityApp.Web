using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Models
{
    public class RoleUpdateViewModel
    {
        public string ID { get; set; }
        [Display(Name = "Role Name")]
        [Required(ErrorMessage = "Need to type name of role!")]
        public string Name { get; set; }
    }
}
