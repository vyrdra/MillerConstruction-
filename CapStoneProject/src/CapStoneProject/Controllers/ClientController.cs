﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CapStoneProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CapStoneProject.Models.ViewModels;
using CapStoneProject.Repositories;
using CapStoneProject.Repositories.Interfaces;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CapStoneProject.Controllers
{
    public class ClientController : Controller
    {
        private UserManager<UserIdentity> userManager;
        private SignInManager<UserIdentity> signInManager;
        private RoleManager<IdentityRole> roleManager;
        private IClientRepo clientRepo;

        public ClientController(RoleManager<IdentityRole> roleMgr, UserManager<UserIdentity> usrMgr, SignInManager<UserIdentity> sim, IClientRepo clRepo)
        {
            userManager = usrMgr;
            signInManager = sim;
            roleManager = roleMgr;
            clientRepo = clRepo;
        }

        public IActionResult Register()
        {
            return View(new VMRegister());
        }

        [HttpPost]
        public async Task<IActionResult> Register(VMRegister vm)
        {
            if (ModelState.IsValid)
            {
                UserIdentity user = new UserIdentity();
                string name = HttpContext.User.Identity.Name;
                user = await userManager.FindByNameAsync(name);

                Client client = new Client
                {
                    FirstName = vm.FirstName,
                    LastName = vm.LastName,
                    CompanyName = vm.CompanyName,
                    Street = vm.Street,
                    City = vm.City,
                    State = vm.State,
                    Zipcode = vm.Zipcode,
                    PhoneNumber = vm.PhoneNumber,
                    UserID = user.Id,
                    Email = user.Email                   
                };

                clientRepo.Create(client);
                return RedirectToAction("Login", "Account");
            }
            else
            { 
            return View(vm);
            }
        }        
    }
}
