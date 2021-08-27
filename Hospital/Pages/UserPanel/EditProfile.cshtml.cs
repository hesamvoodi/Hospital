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

namespace Hospital.Pages.UserPanel
{
    [Authorize(Roles = SD.PatientEndUser)]
    public class EditProfileModel : PageModel
    {
        private ApplicationDbContext _db;

        public EditProfileModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public UserInfo UserInfo { get; set; }

        [TempData]
        public bool? EditProfileStatus { get; set; }

        [BindProperty]
        public string ReturnUrl { get; set; }
        public void OnGet()
        {
            string id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserInfo = _db.UserInfos.FirstOrDefault(u => u.UserId == id);
            HttpContext.Session.SetInt32("UserInfoId", UserInfo.Id);
        }

        public IActionResult OnPost(IFormFile imgUp)
        {
            if (ModelState.IsValid)
            {
                int? id = HttpContext.Session.GetInt32("UserInfoId");
                if (id != null)
                {
                    UserInfo.Id = id.Value;
                }
                else
                {
                    EditProfileStatus = false;
                    return RedirectToPage();
                }

                UserInfo.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _db.Update(UserInfo);
                if (imgUp != null)
                {
                    if (imgUp.Length > 0 && imgUp.Length < 10485760)
                    {
                        string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserFile");
                        //delete last image
                        if (!String.IsNullOrEmpty(UserInfo.FileName) &&
                            System.IO.File.Exists(deletePath + "/" + UserInfo.FileName))
                        {
                            System.IO.File.Delete(deletePath + "/" + UserInfo.FileName);
                        }
                        //save new image
                        UserInfo.FileName = Guid.NewGuid() + Path.GetExtension(imgUp.FileName);
                        string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserFile", UserInfo.FileName);
                        using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            imgUp.CopyTo(stream);
                        }
                    }
                    else if (imgUp.Length > 10485760)
                    {
                        ModelState.AddModelError("خطا در آپلود فایل", "حجم فایل باید کمتر از 10 مگابایت باشد.");
                        return Page();
                    }
                }
                _db.SaveChanges();
                EditProfileStatus = true;
                if (!String.IsNullOrEmpty(ReturnUrl))
                {
                    EditProfileStatus = null;
                    ReturnUrl ??= Url.Content("~/");
                    return LocalRedirect($"/FinalApproval?id={ReturnUrl}");
                }
                else
                {
                    return RedirectToPage();
                }
            }

            EditProfileStatus = false;
            return RedirectToPage();
        }
    }
}
