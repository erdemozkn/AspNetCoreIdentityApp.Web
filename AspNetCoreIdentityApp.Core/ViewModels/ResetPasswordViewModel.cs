using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Messaj kısmı boş bırakılmamalıdır.")]
        public string Password { get; set; }




        [Compare(nameof(Password),ErrorMessage = "Şifreler aynı değil.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Confirm Mesaj kısmı boş bırakılmamalıdır.")]
        public string ConfirmPassword { get; set; }
    }
}
