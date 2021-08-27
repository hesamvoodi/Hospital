using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Models;
using Hospital.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hospital.Pages
{
    public class CompleteUserDataModel : PageModel
    {
        private ApplicationDbContext _db;
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;

        public CompleteUserDataModel(ApplicationDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public UserInfo UserInfo { get; set; }

        public IActionResult OnGet()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _db.Users.Find(userId);
            bool isInfoExist = _db.UserInfos.Any(u => u.UserId == userId);
            if (!User.Identity.IsAuthenticated ||
                !_userManager.IsInRoleAsync(user, SD.PatientEndUser).Result ||
                isInfoExist)
            {
                return RedirectToPage("Index");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                UserInfo.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _db.UserInfos.Add(UserInfo);
                _db.SaveChanges();
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
