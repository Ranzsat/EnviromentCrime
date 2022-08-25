using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EnviromentCrime.Models;

namespace EnviromentCrime.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<IdentityUser> userManager;
        private SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> uManager, SignInManager<IdentityUser> sManager)
        {
            userManager = uManager;
            signInManager = sManager;
        }

        [AllowAnonymous]
        public ViewResult Login(string returnUrl)
        {
            return View(new LoginModel
            {
                ReturnUrl = returnUrl
            });
        }

        /**
         * Login method that is responsible for sending the different
         * roles to the different start pages.
         * If the user inputs the wrong credentials, the system sends a 
         * generic error message back to them.
         * 
         * @return the start page of the different roles
         */
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            //Find the user in the DB
            IdentityUser user = await userManager.FindByNameAsync(loginModel.UserName);
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    //Null the users previous session
                    await signInManager.SignOutAsync();

                    //Compare the users input password with the password in the DB
                    if ((await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false)).Succeeded)
                    {
                        //Goto start page for Coordinator 
                        if(await userManager.IsInRoleAsync(user, "Coordinator"))
                        {
                            return Redirect("/Coordinator/StartCoordinator");
                        }
                        
                        //Goto start page for Manager
                        if(await userManager.IsInRoleAsync(user, "Manager"))
                        {
                            return Redirect("/Manager/StartManager");
                        }

                        //Goto start page for Investigator
                        if (await userManager.IsInRoleAsync(user, "Investigator"))
                        {
                            return Redirect("/Investigator/StartInvestigator");
                        }
                    }
                }
            }

            //If the input credentials were wrong
            ModelState.AddModelError("", "Felaktigt användarnamn eller lösenord");
            return View(loginModel);
        }

        public async Task<RedirectResult> LogOut(string returnUrl = "/")
        {
            await signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
        [AllowAnonymous]
        public ViewResult AccessDenied()
        {
            ViewBag.Title = "Småstads Kommun: Access Denied";
            return View();
        }
    }
}
