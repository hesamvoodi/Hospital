using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace Hospital.Pages.DoctorPanel.ManageTime
{
    [ApiController]
    [Route("TimeDelete")]
    [Authorize(Roles = SD.DoctorEndUser)]
    public class DeleteController : Controller
    {
        private ApplicationDbContext _db;

        public DeleteController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var schedule = _db.Schedules.Find(id);
            var doctor = _db.Doctors.FirstOrDefault(d => d.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (schedule == null || schedule.DoctorId != doctor.Id || schedule.UserId != null)
            {
                return Json(new { success = false, message = "مشکلی پیش آمده لطفا دوباره تلاش کنید!" });
            }

            try
            {
                _db.Remove(schedule);
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
