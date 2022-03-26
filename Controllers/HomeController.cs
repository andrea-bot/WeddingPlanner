using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private WeddingContext dbContext;
        public HomeController(WeddingContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {

            return View();
        }

        // Create
        [HttpPost("create")]
        public IActionResult Create(WeddingUser user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(o => o.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use");
                    return View("Index");
                }

                PasswordHasher<WeddingUser> hasher = new PasswordHasher<WeddingUser>();
                user.Password = hasher.HashPassword(user, user.Password);

                var newUser = dbContext.Users.Add(user).Entity;
                dbContext.SaveChanges();

                HttpContext.Session.SetInt32("userId", newUser.UserId);

                return RedirectToAction("Index", "Wedding");
            }
            
            return View("Index");
        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser user)
        {
            var test = dbContext.Users.ToList();
            if(ModelState.IsValid)
            {
                WeddingUser toLogin = dbContext.Users.FirstOrDefault(u => u.Email == user.EmailAttempt);
                if(toLogin == null)
                {
                    ModelState.AddModelError("EmailAttempt", "Invalid Email/Password");
                    return View("Index");
                }
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(user, toLogin.Password, user.PasswordAttempt);
                if(result == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError("EmailAttempt", "Invalid Email/Password");
                    return View("Index");
                }
                // Log user into session
                HttpContext.Session.SetInt32("userId", toLogin.UserId);
                return RedirectToAction("Index", "Wedding");
            }
            return View("Index");
        }
        [HttpGet("logout")]
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}