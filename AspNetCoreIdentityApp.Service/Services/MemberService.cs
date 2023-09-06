using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Service.Services
{
    public class MemberService : IMemberService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFileProvider _fileProvider;
        public MemberService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IFileProvider fileProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileProvider = fileProvider;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        async Task<UserViewModel> GetUserViewModelByUserNameAsync(string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);
            return new UserViewModel
            {
                Email = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                UserName = currentUser.UserName,
                PictureUrl = currentUser.Picture
            };
        }

        Task<UserViewModel> IMemberService.GetUserViewModelByUserNameAsync(string userName)
        {
            throw new NotImplementedException();
        }

        async Task<bool> IMemberService.CheckPasswordAsync(string userName, string oldPassword)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);

            return await _userManager.CheckPasswordAsync(currentUser, oldPassword);
        }


        async Task<(bool, IEnumerable<IdentityError>?)> IMemberService.ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);

            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser!, oldPassword, newPassword);

            if (!resultChangePassword.Succeeded)
            {
                return (false, resultChangePassword.Errors);
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser, newPassword, true, true);

            return (true, null);
        }


        async Task<UserEditViewModel> IMemberService.GetUserEditViewModelAsync(string userName)
        {
            var currentuser = (await _userManager.FindByNameAsync(userName))!;
            return new UserEditViewModel()
            {
                UserName = currentuser.UserName!,
                Email = currentuser.Email!,
                BirthDate = currentuser.BirthDate,
                City = currentuser.City,
                Gender = currentuser.Gender,
                Phone = currentuser.PhoneNumber!
            };
        }

        SelectList IMemberService.GetGenderSelectList()
        {
            return new SelectList(Enum.GetNames(typeof(Gender)));
        }


        async Task<(bool, IEnumerable<IdentityError>?)> IMemberService.EditUserAsync(UserEditViewModel request, string userName)
        {
            var currentuser = (await _userManager.FindByNameAsync(userName))!;

            currentuser.UserName = request.UserName;
            currentuser.Email = request.Email;
            currentuser.BirthDate = (DateTime)request.BirthDate;
            currentuser.City = request.City;
            currentuser.Gender = request.Gender;

            if (request.Picture != null && request.Picture.Length > 0)
            {

                //those are for wwwroot photo folder
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");

                var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";

                var newPicturePath = Path.Combine(wwwrootFolder.First(x => x.Name == "userpictures").PhysicalPath!, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create);

                await request.Picture.CopyToAsync(stream);
                //this one dor database
                currentuser.Picture = randomFileName;
            }

            var updateToUserResult = await _userManager.UpdateAsync(currentuser);
            if (!updateToUserResult.Succeeded)
            {
                return (false, updateToUserResult.Errors);
            }

            await _userManager.UpdateSecurityStampAsync(currentuser);
            await _signInManager.SignOutAsync();

            if (request.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(currentuser, true, new[]
                {
                    new Claim("birthday",currentuser.BirthDate.ToString())
                });
            }
            else
            {
                await _signInManager.SignInAsync(currentuser, true);
            }

            return (true, null);


        }
    }
}
