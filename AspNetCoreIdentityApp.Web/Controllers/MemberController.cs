using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.Security.Claims;
using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Service.Services;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;
        private readonly IMemberService _memberService;
        private string userName => User.Identity!.Name!;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
        }

        public async Task<IActionResult> Index()
        {

            return View(await _memberService.GetUserViewModelByUserNameAsync(userName));
        }




        [HttpGet]
        public async Task<IActionResult> PasswordChange()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel passwordChangeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!await _memberService.CheckPasswordAsync(userName, passwordChangeViewModel.OldPassword!))
            {
                ModelState.AddModelError(string.Empty, "Eski şifreniz yanlış.");
                return View();
            }

            var (isSuccess,errors) = await _memberService.ChangePasswordAsync(userName, passwordChangeViewModel.OldPassword!, passwordChangeViewModel.NewPassword!);

            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors!.Select(x => x.Description).ToList());
                return View();
            }


            TempData["SuccessMessage"] = "Üyelik işkemi başarıyla gerçekleşmiştir.";


            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList = _memberService.GetGenderSelectList();

            return View(await _memberService.GetUserEditViewModelAsync(userName));
        }
        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel userEditViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var (isSuccess,errors) = await _memberService.EditUserAsync(userEditViewModel, userName);

            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors.Select(x => x.Description).ToList());
                return View();
            }

            TempData["SuccessMessage"] = "Üye bilgileri başarıyla değiştirildi.";


            return View(await _memberService.GetUserEditViewModelAsync(userName));
        }



        public async Task LogOut()
        {
            //Program.cs there is a logoutpath in 25.row that way is more efficient.
            await _memberService.LogoutAsync();

            //return RedirectToAction("Index", "Home
        }

        public IActionResult AccessDenied(string returnURL)
        {
            string message = "Bu sayfayı görmeye yetkiniz yok.";
            ViewBag.deniedmessage = message;
            return View();
        }

        public IActionResult Claims()
        {
            var list = User.Claims.Select(x => new ClaimViewModel()
            {
                Issuer = x.Issuer,
                Type = x.Type,
                Value = x.Value
            }).ToList();
            return View(list);
        }

        [Authorize(Policy = "AntalyaPolicy")]
        public IActionResult AntalyaPage()
        {
            return View();
        }

        [Authorize(Policy = "ExchangePolicy")]
        public IActionResult ExchangePage()
        {
            return View();
        }

        [Authorize(Policy = "ViolencePolicy")]
        public IActionResult ViolencePage()
        {
            return View();
        }

    }
}
