using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [EmailAddress(ErrorMessage = "Email Formatına Uygun Giriş Yapınız...")]
        [Required(ErrorMessage = "Email Adresini Giriniz...")]
        [Display(Name = "Email:")]
        public string Email { get; set; }
    }
}
