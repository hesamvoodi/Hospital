using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Models;
using Hospital.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hospital.Pages.DoctorPanel.ManageTime
{
    [Authorize(Roles = SD.DoctorEndUser)]
    public class UserDetailsModel : PageModel
    {
        private ApplicationDbContext _db;

        public UserDetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public UserInfo UserInfo { get; set; }
        public string PhoneNumber { get; set; }
        public IActionResult OnGet(int id = 0)
        {
            if (id != 0)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var doctor = _db.Doctors.FirstOrDefault(d => d.UserId == userId);
                if (_db.Schedules.Any(s => s.UserId == id && s.DoctorId == doctor.Id))
                {
                    UserInfo = _db.UserInfos.FirstOrDefault(u => u.Id == id);
                    PhoneNumber = _db.Users.FirstOrDefault(u => u.Id == UserInfo.UserId).PhoneNumber;
                    return Page();
                }
            }

            return NotFound();
        }
    }
}
