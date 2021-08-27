using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Hospital.Pages.Admin.DoctorRequest
{
    [ApiController]
    [Route("AcceptRequest")]
    [Authorize(Roles = SD.AdminEndUser)]
    public class AcceptController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<IdentityUser> _userManager;

        public AcceptController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        [HttpPost]
        public IActionResult Accept(int id = 0)
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
                //delete userInfo
                var userInfo = _db.UserInfos.FirstOrDefault(u => u.UserId == doctorTicket.UserId);
                if (userInfo != null)
                    _db.UserInfos.Remove(userInfo);
                //delete DoctorRequest
                var doctorRequest = _db.DoctorTickets.FirstOrDefault(d => d.UserId == doctorTicket.UserId);
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
                }
                //find user
                var user = _db.Users.FirstOrDefault(u => u.Id == doctorTicket.UserId);
                //remove role
                _userManager.RemoveFromRoleAsync(user, SD.PatientEndUser).Wait();
                //add to role
                _userManager.AddToRoleAsync(user, SD.DoctorEndUser).Wait();


                doctorTicket.IsAccepted = true;
                _db.Update(doctorTicket);
                _db.SaveChanges();
                return Json(new { success = true, message = "نقش کاربر مورد نظر به دکتر تغییر کرد." });
            }
            catch
            {
                return Json(new { success = false, message = "مشکلی پیش آمده!" });
            }
        }
    }
}
