using AspNetCoreIdentityApp.Core.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class UserEditViewModel
    {
        [Display(Name = "UserName")]
        [Required(ErrorMessage = "UserName cannot be null.")]
        public string? UserName { get; set; }

        [EmailAddress(ErrorMessage = "Email formatı yanlıştır.")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email cannot be null.")]
        public string? Email { get; set; }

        [Display(Name = "Phone")]
        [Required(ErrorMessage = "Phone cannot be null.")]
        public string? Phone { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birthday")]
        [Required(ErrorMessage = "Birthday cannot be null.")]
        public DateTime? BirthDate { get; set; }


        [Display(Name = "City")]
        [Required(ErrorMessage = "City cannot be null.")]
        public string? City { get; set; }

        [Display(Name = "Picture")]
        public IFormFile? Picture { get; set; }


        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }
    }
}
