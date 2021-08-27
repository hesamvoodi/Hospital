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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hospital.Pages.DoctorPanel.ManageTime
{
    [Authorize(Roles = SD.DoctorEndUser)]
    public class IndexModel : PageModel
    {
        private ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<OrderViewModel> Orders { get; set; }

        public class OrderViewModel
        {
            public Schedule Schedule { get; set; }

            public int TotalPrice { get; set; }

            public string UserFullName { get; set; }

            public string Status { get; set; }
        }
        public void OnGet(string statusInput = null)
        {
            Orders = new List<OrderViewModel>();
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = _db.Doctors.FirstOrDefault(u => u.UserId == userId);
            var schedules = _db.Schedules
                .Where(s => s.DoctorId == doctor.Id && DateTime.Compare(DateTime.Now, s.EndDate) < 0)
                .OrderBy(s => s.StartDate).ToList();
            if (statusInput != null)
            {
                if (statusInput == "Reserved")
                {
                    schedules = schedules.Where(s => s.UserId != null).ToList();
                }
                else if (statusInput == "NotReserved")
                {
                    schedules = schedules.Where(s => s.UserId == null).ToList();
                }
            }
            List<object> newList = new List<object>();
            newList.Add(new
            {
                Key = "Reserved",
                Name = "رزرو شده"
            });
            newList.Add(new
            {
                Key = "NotReserved",
                Name = "رزرو نشده"
            });

            if (statusInput != null)
            {
                ViewData["ReservationStatus"] = new SelectList(newList, "Key", "Name", statusInput);
            }
            else
            {
                ViewData["ReservationStatus"] = new SelectList(newList, "Key", "Name");
            }
            foreach (var item in schedules)
            {
                var user = _db.UserInfos.FirstOrDefault(d => d.Id == item.UserId);
                string status = "";
                if (item.UserId == null)
                {
                    status = "وقت رزرو نشده";
                }
                else if (DateTime.Compare(DateTime.Now, item.StartDate) < 0)
                {
                    status = "ویزیت شروع نشده";
                }
                else
                {
                    status = "در حال برگزاری لطفا وارد شوید";
                }
                Orders.Add(new OrderViewModel()
                {
                    Schedule = item,
                    TotalPrice = doctor.VisitPrice + doctor.ExtraTimePricePerMinute * item.ExtraMinute,
                    UserFullName = user != null ? $"{user.Name} {user.LastName}" : "---",
                    Status = status
                });
            }
        }
    }
}
