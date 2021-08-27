using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Pages
{
    public class CompleteDoctorDataModel : PageModel
    {
        private ApplicationDbContext _db;
        private UserManager<IdentityUser> _userManager;

        public CompleteDoctorDataModel(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult OnGet()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _db.Users.Find(userId);
            bool isInfoExist = _db.Doctors.Any(u => u.UserId == userId);
            if (!User.Identity.IsAuthenticated ||
                !_userManager.IsInRoleAsync(user, SD.DoctorEndUser).Result ||
                isInfoExist)
            {
                return RedirectToPage("Index");
            }
            ViewData["GroupId"] = new SelectList(_db.DoctorGroups, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Models.Doctor Doctor { get; set; }

        public IActionResult OnPost(IFormFile imgUp)
        {
            if (ModelState.IsValid)
            {
                if (imgUp != null)
                {
                    if (imgUp.Length > 0 && imgUp.Length < 2097152)
                    {
                        //save new image
                        var randomFileName = Path.GetRandomFileName();
                        randomFileName = randomFileName.Substring(0, randomFileName.IndexOf('.'))
                                         + Path.GetExtension(imgUp.FileName);
                        var filePath = Path.Combine("wwwroot/DoctorImage", randomFileName);
                        Doctor.ImageName = randomFileName;
                        using var stream = System.IO.File.Create(filePath);
                        imgUp.CopyToAsync(stream).Wait();
                    }
                    else if (imgUp.Length > 2097152)
                    {
                        ModelState.AddModelError("خطا در آپلود فایل", "حجم فایل باید کمتر از 2 مگابایت باشد.");
                        ViewData["GroupId"] = new SelectList(_db.DoctorGroups, "Id", "Name");
                        return Page();
                    }
                }

                Doctor.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _db.Doctors.Add(Doctor);
                _db.SaveChanges();
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
