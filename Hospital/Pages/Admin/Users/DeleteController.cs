using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Hospital.Data;
using Hospital.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Pages.Admin.Users
{
    [ApiController]
    [Route("OneUserDelete")]
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
        public IActionResult Delete(string id)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "مشکلی پیش آمده لطفا دوباره تلاش کنید!" });
            }

            try
            {
                if (_userManager.IsInRoleAsync(user, SD.DoctorEndUser).Result)
                {
                    var doctor = _db.Doctors.FirstOrDefault(d => d.UserId == user.Id);
                    if (doctor != null)
                        _db.Doctors.Remove(doctor);
                }
                if (_userManager.IsInRoleAsync(user, SD.PatientEndUser).Result)
                {
                    var userInfo = _db.UserInfos.FirstOrDefault(d => d.UserId == user.Id);
                    if (userInfo != null)
                        _db.UserInfos.Remove(userInfo);
                }

                if (_userManager.IsInRoleAsync(user, SD.PatientEndUser).Result)
                {
                    var userInfo = _db.UserInfos.FirstOrDefault(d => d.UserId == user.Id);
                    if (userInfo != null)
                    {
                        var schedules = _db.Schedules.Where(s => s.UserId == userInfo.Id).ToList();
                        foreach (var item in schedules)
                        {
                            item.UserId = null;
                        }
                        _db.UpdateRange(schedules);
                    }
                }
                var doctorTicket = _db.DoctorTickets.FirstOrDefault(d => d.UserId == user.Id);
                if (doctorTicket != null)
                    _db.DoctorTickets.Remove(doctorTicket);
                _db.Remove(user);
                _db.SaveChanges();
                return Json(new { success = true, message = "فیلد مورد نظر حذف شد." });
            }
            catch
            {
                return Json(new { success = false, message = "مشکلی پیش آمده!" });
            }
        }
    }
}
