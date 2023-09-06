using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Service.Services;
using NuGet.Common;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;


        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }





        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel signUpViewModel)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }
            var identityResult = await _userManager.CreateAsync(new()
            {
                UserName = signUpViewModel.UserName,
                Email = signUpViewModel.Email,
                PhoneNumber = signUpViewModel.Phone
            }, signUpViewModel.PasswordConfirm);


            if (!identityResult.Succeeded)
            {
                ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
                return View();
            }


            //borsa sitesi gibi bir yerdek ihesapları 10 günlüğüne bedava kullandıırm sora para istiyoruz. İşte o zaman 10 gün sınırı koyduk.
            var exchangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());
            var user = await _userManager.FindByNameAsync(signUpViewModel.UserName);
            var claimResult = await _userManager.AddClaimAsync(user!, exchangeExpireClaim);

            if (!claimResult.Succeeded)
            {
                ModelState.AddModelErrorList(claimResult.Errors.Select(x => x.Description).ToList());
                return View();
            }

            TempData["SuccessMessage"] = "Üyelik kayıt işlemi başarıyla gerçekleşmiştir.";
            return RedirectToAction(nameof(HomeController.SignUp));

        }





        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel signInViewModel, string? returnurl = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            returnurl = returnurl ?? Url.Action("Index", "Home");

            var hasUser = await _userManager.FindByEmailAsync(signInViewModel.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya şifre yanlış");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(hasUser, signInViewModel.Password, signInViewModel.RememberMe, true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>()
                {
                    "3 dakika boyunca giriş yapamazsınız"
                });
                return View();
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(new List<string>()
                {
                "Email veya şifreniz yanlış"
                });
                return View();
            }

            if (hasUser?.BirthDate != null)
            {
                await _signInManager.SignInWithClaimsAsync(hasUser, signInViewModel.RememberMe, new[]
                {
                    new Claim("birthday",hasUser.BirthDate.ToString())
                });
            }

            return Redirect(returnurl!);

        }





        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel forgetPasswordViewModel)
        {
            var HasUser = await _userManager.FindByEmailAsync(forgetPasswordViewModel.Email);
            if (HasUser == null)
            {
                ModelState.AddModelError(String.Empty, "Bu email adresine sahip kullanıcı bulunmamaktadır.");
                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(HasUser);

            var passwordResetLink = Url.Action("ResetPassword", "Home", new
            {
                UserID = HasUser.Id,
                Token = passwordResetToken
            }, HttpContext.Request.Scheme);
            //link
            //https://localhost:7008\UserID=313131&Token=klasgdkausbhdlkasdşlaksjd

            //email

            await _emailService.SendResetPasswordEmail(passwordResetLink, HasUser.Email);



            TempData["SuccessMessage"] = "Şifre sıfırlama e-posta adresinize gönderilmiştir.";

            return RedirectToAction(nameof(ForgetPassword));
        }




        [HttpGet]
        public IActionResult ResetPassword(string userID, string Token)
        {
            TempData["userID"] = userID;
            TempData["Token"] = Token;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {

            var userID = TempData["userID"];
            var Token = TempData["Token"];


            if (userID == null || Token == null)
            {
                throw new Exception("bir hata meydana geldi.");
            }

            var hasUser = await _userManager.FindByIdAsync(userID.ToString());

            if (hasUser == null)
            {
                ModelState.AddModelError(String.Empty, "Kullanıcı bulunamamıştır");
            }

            var result = await _userManager.ResetPasswordAsync(hasUser, Token.ToString(), resetPasswordViewModel.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla yenilenmiştir";
            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
                return View();
            }


            return RedirectToAction(nameof(SignIn));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}