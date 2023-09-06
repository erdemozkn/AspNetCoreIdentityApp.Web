using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "Kullanıcı Ad boş bırakılamaz.")]
        [Display(Name = "Kullanıcı Adı:")]
        public string UserName { get; set; }



        [EmailAddress(ErrorMessage = "Email formatı yanlış")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz.")]
        [Display(Name = "Email:")]
        public string Email { get; set; }



        [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
        [Display(Name = "Telefon:")]
        public string Phone { get; set; }


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
        [Display(Name = "Şifre:")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olabilir.")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage = "Şifre ile aynı değil")]
        [Required(ErrorMessage = "Confirm şifre alanı boş bırakılamaz")]
        [Display(Name = "Confirm Şifre:")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olabilir.")]
        public string PasswordConfirm { get; set; }
    }
}
