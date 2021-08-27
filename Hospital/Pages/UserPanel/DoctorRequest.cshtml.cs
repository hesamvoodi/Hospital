using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Models;
using Hospital.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hospital.Pages.UserPanel
{
    [Authorize(Roles = SD.PatientEndUser)]
    public class DoctorRequestModel : PageModel
    {
        private ApplicationDbContext _db;

        public DoctorRequestModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public DoctorTicket DoctorTicket { get; set; }

        [BindProperty]
        public List<IFormFile> FilesUp { get; set; }

        public bool CanAccess { get; set; }

        [TempData]
        public bool IsSuccess { get; set; }
        
        public void OnGet()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CanAccess = !_db.DoctorTickets.Any(d => d.UserId == userId);
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                if (FilesUp != null)
                {
                    if (FilesUp.Count <= 3)
                    {
                        DoctorTicket.FilesUpload = "";
                        foreach (var file in FilesUp)
                        {
                            if (file.Length > 4194304)
                            {
                                ModelState.AddModelError("", "هر فایل باید حداکثر 4 مگابایت باشد.");
                                IsSuccess = false;
                                CanAccess = true;
                                return Page();
                            }
                        }
                        foreach (var file in FilesUp)
                        {
                            if (file.Length > 0 && file.Length < 4194304)
                            {
                                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                                DoctorTicket.FilesUpload += fileName + ",";
                                string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/TicketFiles", fileName);
                                using (var stream = new FileStream(savePath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "تعداد فایل ها حداکثر باید 3 عدد باشد.");
                        IsSuccess = false;
                        CanAccess = true;
                        return Page();
                    }
                }

                DoctorTicket.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                DoctorTicket.IsAccepted = null;
                _db.DoctorTickets.Add(DoctorTicket);
                _db.SaveChanges();
                IsSuccess = true;
                return RedirectToPage();
            }

            IsSuccess = false;
            CanAccess = true;
            return Page();
        }
    }
}
