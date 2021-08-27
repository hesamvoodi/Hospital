using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Models;
using Hospital.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hospital.Pages.UserPanel
{
    [Authorize(Roles = SD.PatientEndUser)]
    public class OrderListModel : PageModel
    {
        private ApplicationDbContext _db;

        public OrderListModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<OrderViewModel> Orders { get; set; }

        public class OrderViewModel
        {
            public Schedule Schedule { get; set; }

            public int TotalPrice { get; set; }

            public string DoctorFullName { get; set; }

            public string Status { get; set; }
        }
        public void OnGet()
        {
            Orders = new List<OrderViewModel>();
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userInfo = _db.UserInfos.FirstOrDefault(u => u.UserId == userId);
            var schedules = _db.Schedules.Where(s => s.UserId == userInfo.Id)
                .OrderByDescending(s => s.StartDate).ToList();
            foreach (var item in schedules)
            {
                var doctor = _db.Doctors.FirstOrDefault(d => d.Id == item.DoctorId);
                string status = "";
                if (DateTime.Compare(DateTime.Now, item.EndDate) >= 0)
                {
                    status = "ویزیت انجام شد";
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
                    DoctorFullName = $"{doctor.FirstName} {doctor.LastName}",
                    Status = status
                });
            }
        }
    }
}
