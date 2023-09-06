using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "role-action, admin")]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(x => new RoleViewModel()
            {
                ID = x.Id,
                Name = x.Name,
            }).ToListAsync();

            return View(roles);
        }




        [Authorize(Roles = "role-action")]
        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }
        [Authorize(Roles = "role-action")]
        [HttpPost]
        public async Task<IActionResult> AddRole(RoleCreateViewModel roleCreateViewModel)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = roleCreateViewModel.Name });
            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
                return View(result);    
            }
            TempData["SuccessMessage"] = "Rol oluşturulmuştur";
            return RedirectToAction(nameof(RolesController.Index));
        }





        [Authorize(Roles = "role-action")]
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);
            var list = new RoleUpdateViewModel()
            {
                ID = roleToUpdate.Id,
                Name = roleToUpdate.Name
            };
            return View(list);
        }
        [Authorize(Roles = "role-action")]
        [HttpPost]
        public async Task<IActionResult> EditRole(RoleUpdateViewModel roleUpdateViewModel)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(roleUpdateViewModel.ID);
            roleToUpdate.Name = roleUpdateViewModel.Name;
            await _roleManager.UpdateAsync(roleToUpdate);
            TempData["SuccessMessage"] = "Rol bilgisi güncellenmiştir";
            return RedirectToAction(nameof(RolesController.Index));
        }


        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);
            await _roleManager.DeleteAsync(roleToDelete);
            TempData["SuccessMessage"] = "Rol silinmiştir";
            return RedirectToAction(nameof(RolesController.Index));
        }






        [Authorize(Roles = "role-action")]
        [HttpGet]
        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            //send id to post to find user
            ViewBag.Id = id;
            //choosen user
            var currenuser = await _userManager.FindByIdAsync(id);
            //all of the roles
            var roles = await _roleManager.Roles.ToListAsync();
            //user's roles
            var userRoles = await _userManager.GetRolesAsync(currenuser);
            //Template List
            var roleViewModelList = new List<AssignRoleToUserViewModel>();


            foreach (var item in roles)
            {
                //Template
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel()
                {
                    ID = item.Id,
                    Name = item.Name,
                };

                if (userRoles.Contains(item.Name))
                {
                    assignRoleToUserViewModel.Status = true;
                }

                roleViewModelList.Add(assignRoleToUserViewModel);

            }


            return View(roleViewModelList);
        }
        [Authorize(Roles = "role-action")]
        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(List<AssignRoleToUserViewModel> requestList,string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            foreach (var item in requestList)
            {
                if(item.Status)
                {
                   await _userManager.AddToRoleAsync(user,item.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user,item.Name);
                }




            }
            return RedirectToAction(nameof(HomeController.UserList),"Home");
        }
    }
}
