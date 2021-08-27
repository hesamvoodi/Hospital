using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hospital.Pages.Admin.Users
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private UserManager<IdentityUser> _userManager;
        private ApplicationDbContext _db;

        public EditModel(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [Display(Name = "نقش")]
        [BindProperty]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public string Role { get; set; }

        [BindProperty]
        public string Id { get; set; }

        public IActionResult OnGet(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _userManager.FindByIdAsync(id).Result;
            var userRoles = _userManager.GetRolesAsync(user).Result;
            var allRoles = new List<SelectListItem>()
            {
                new SelectListItem("ادمین", SD.AdminEndUser),
                new SelectListItem("دکتر", SD.DoctorEndUser),
                new SelectListItem("کاربر", SD.PatientEndUser)
            };
            Role = userRoles.FirstOrDefault();
            Id = id;
            ViewData["RolesList"] = new SelectList(allRoles, "Value", "Text", Role);
            return Page();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.FindByIdAsync(Id).Result;
                if (user == null)
                {
                    return RedirectToPage("Index");
                }

                var roles = _userManager.GetRolesAsync(user).Result;
                if (roles.Count > 1 || roles.FirstOrDefault() != Role)
                {
                    foreach (var item in roles)
                    {
                        if (item == SD.PatientEndUser)
                        {
                            var userInfo = _db.UserInfos.FirstOrDefault(u => u.UserId == Id);
                            if (userInfo != null)
                            {
                                /*var userSchedules = _db.Schedules.Where(s => s.UserId == userInfo.Id);
                                _db.Schedules.RemoveRange(userSchedules);*/
                                _db.UserInfos.Remove(userInfo);
                            }
                            var doctorRequest = _db.DoctorTickets.FirstOrDefault(d => d.UserId == user.Id);
                            if (doctorRequest != null)
                            {
                                foreach (var file in doctorRequest.FilesUpload.Split(','))
                                {
                                    string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/TicketFiles");
                                    if (!String.IsNullOrEmpty(file) &&
                                        System.IO.File.Exists(deletePath + "/" + file))
                                    {
                                        System.IO.File.Delete(deletePath + "/" + file);
                                    }
                                }
                                _db.DoctorTickets.Remove(doctorRequest);
                            }
                        }
                        else if (item == SD.DoctorEndUser)
                        {
                            var doctor = _db.Doctors.FirstOrDefault(u => u.UserId == Id);
                            /*var schedules = _db.Schedules.Where(s => s.DoctorId == doctor.Id);
                            _db.Schedules.RemoveRange(schedules);*/

                            if (doctor != null)
                            {
                                //delete last image
                                string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/DoctorImage");
                                if (!String.IsNullOrEmpty(doctor.ImageName) &&
                                    System.IO.File.Exists(deletePath + "/" + doctor.ImageName))
                                {
                                    System.IO.File.Delete(deletePath + "/" + doctor.ImageName);
                                }
                                _db.Doctors.Remove(doctor);
                            }
                               

                        }
                        _userManager.RemoveFromRoleAsync(user, item).Wait();
                    }

                    _db.SaveChanges();
                    _userManager.AddToRoleAsync(user, Role).Wait();
                }
                return RedirectToPage("Index");
            }

            return Page();
        }
    }
}
