using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Hospital.Pages.Doctor
{
    [ApiController]
    [Route("DoctorDelete")]
    [Authorize(Roles = SD.AdminEndUser)]
    public class DeleteController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<IdentityUser> _userManager;

        public DeleteController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var doctor = _db.Doctors.FirstOrDefault(u => u.Id == id);
            if (doctor == null)
            {
                return Json(new {success = false, message = "مشکلی پیش آمده لطفا دوباره تلاش کنید!"});
            }
            try
            {
                foreach (var doctorTicket in _db.DoctorTickets.Where(d => d.UserId == doctor.UserId).ToList())
                {
                    _db.DoctorTickets.Remove(doctorTicket);
                }
                _db.Doctors.Remove(doctor);
                var user = _db.Users.FirstOrDefault(u => u.Id == doctor.UserId);
                _db.Users.Remove(user);
                _db.SaveChanges();
                return Json(new {success = true, message = "فیلد مورد نظر حذف شد."});
            }
            catch
            {
                return Json(new { success = false, message = "مشکلی پیش آمده!" });
            }
        }
    }
}
