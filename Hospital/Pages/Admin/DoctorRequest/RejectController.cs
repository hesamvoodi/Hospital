using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace Hospital.Pages.Admin.DoctorRequest
{
    [ApiController]
    [Route("RejectRequest")]
    [Authorize(Roles = SD.AdminEndUser)]
    public class RejectController : Controller
    {
        private ApplicationDbContext _db;

        public RejectController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpPost]
        public IActionResult Reject(int id = 0)
        {
            if (id == 0)
            {
                return Json(new { success = false, message = "مشکلی پیش آمده!" });
            }
            var doctorTicket = _db.DoctorTickets.Find(id);
            if (doctorTicket == null)
            {
                return Json(new { success = false, message = "مشکلی پیش آمده لطفا دوباره تلاش کنید!" });
            }

            try
            {
                foreach (var file in doctorTicket.FilesUpload.Split(','))
                {
                    //delete last image
                    string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/TicketFiles");
                    if (!String.IsNullOrEmpty(file) &&
                        System.IO.File.Exists(deletePath + "/" + file))
                    {
                        System.IO.File.Delete(deletePath + "/" + file);
                    }
                }
                doctorTicket.IsAccepted = false;
                _db.Update(doctorTicket);
                _db.SaveChanges();
                return Json(new { success = true, message = "درخواست مورد نظر رد شد." });
            }
            catch
            {
                return Json(new { success = false, message = "مشکلی پیش آمده!" });
            }
        }
    }
}
