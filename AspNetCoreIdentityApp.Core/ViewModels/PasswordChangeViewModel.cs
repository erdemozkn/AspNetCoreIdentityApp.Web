using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class PasswordChangeViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Previous Password")]
        [Required(ErrorMessage = "Previous Message Required")]
        [MinLength(6,ErrorMessage = "Şifreniz en az 6 karakter olabilir.")]
        public string? OldPassword { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [Required(ErrorMessage = "Old Message Required")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olabilir.")]
        public string? NewPassword { get; set; }

        [Compare(nameof(NewPassword),ErrorMessage = "Şifreler uyuşmuyor.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Required(ErrorMessage = "Old Message Required")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olabilir.")]
        public string? ConfirmPassword { get; set; }
    }
}
