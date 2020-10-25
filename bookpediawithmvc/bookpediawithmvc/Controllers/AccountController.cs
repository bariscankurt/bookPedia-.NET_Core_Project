using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bookpediawithmvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace bookpediawithmvc.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager)
        {
            userManager = _userManager;
            signInManager = _signInManager;
        }

        
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    var result = await signInManager.PasswordSignInAsync(user,model.Password,false,false);
                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/");
                    }

                }
                else
                {
                    ModelState.AddModelError("Email", "Invalid Email or Password");
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(RegisterModel model)
        {
            int duplicateControl = 0;
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                foreach (var item in userManager.Users)
                {
                    if (model.UserName == item.UserName)
                    {
                        duplicateControl++;
                        ModelState.AddModelError("", "Username cannot be duplicated");
                    }


                }
                if (duplicateControl > 0)
                {
                    return View(model);
                }
                user.UserName = model.UserName;
                user.Email = model.Email;
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }

                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
