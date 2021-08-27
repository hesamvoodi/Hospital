using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace Hospital.Pages.Admin.DoctorGroup
{
    [ApiController]
    [Route("GroupDelete")]
    [Authorize(Roles = SD.AdminEndUser)]
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
            var doctorGroup = _db.DoctorGroups.Find(id);
            if (doctorGroup == null)
            {
                return Json(new {success = false, message = "مشکلی پیش آمده لطفا دوباره تلاش کنید!"});
            }

            try
            {
                _db.Remove(doctorGroup);
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
