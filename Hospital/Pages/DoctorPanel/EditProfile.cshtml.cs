using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Models;
using Hospital.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hospital.Pages.DoctorPanel
{
    [Authorize(Roles = SD.DoctorEndUser)]
    public class EditProfileModel : PageModel
    {
        private ApplicationDbContext _db;

        public EditProfileModel(ApplicationDbContext db)
        {
            _db = db;
        }


        [BindProperty]
        public Models.Doctor Doctor { get; set; }

        [BindProperty]
        public string Base64Id { get; set; }

        public IActionResult OnGet()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Doctor = _db.Doctors.FirstOrDefault(d => d.UserId == userId);
            if (Doctor == null)
            {
                return NotFound();
            }

            ViewData["GroupId"] = new SelectList(_db.DoctorGroups, "Id", "Name", Doctor.GroupId);
            Base64Id = Convert.ToBase64String(BitConverter.GetBytes(Doctor.Id)); ;
            return Page();
        }

        public IActionResult OnPost(IFormFile imgUp)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (imgUp != null && imgUp.FileName.IsImage())
                {
                    if (imgUp.Length > 0 && imgUp.Length < 2097152)
                    {
                        if (Doctor.ImageName != null)
                        {
                            string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/DoctorImage", Doctor.ImageName);
                            //delete last image
                            if (!String.IsNullOrEmpty(Doctor.ImageName) &&
                                System.IO.File.Exists(deletePath))
                            {
                                System.IO.File.Delete(deletePath);
                            }
                        }
                        //save new image
                        Doctor.ImageName = Guid.NewGuid() + Path.GetExtension(imgUp.FileName);
                        string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/DoctorImage", Doctor.ImageName);
                        using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            imgUp.CopyTo(stream);
                        }
                    }
                    else if (imgUp.Length > 2097152)
                    {
                        ModelState.AddModelError("خطا در آپلود فایل", "حجم فایل باید کمتر از 2 مگابایت باشد.");
                        ViewData["GroupId"] = new SelectList(_db.DoctorGroups, "Id", "Name", Doctor.GroupId);
                        return Page();
                    }
                }
                
                Doctor.Id = BitConverter.ToInt32(Convert.FromBase64String(Base64Id), 0);
                Doctor.UserId = userId;
                _db.Update(Doctor);
                _db.SaveChanges();
                ViewData["GroupId"] = new SelectList(_db.DoctorGroups, "Id", "Name", Doctor.GroupId);
                return Page();
            }
            ViewData["GroupId"] = new SelectList(_db.DoctorGroups, "Id", "Name", Doctor.GroupId);
            return Page();
        }
    }
}
