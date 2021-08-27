using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hospital.Pages.DoctorPanel.ManageTime
{
    public class CreateModel : PageModel
    {
        private ApplicationDbContext _db;

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public CreateScheduleViewModel Schedule { get; set; }
        public class CreateScheduleViewModel
        {
            [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
            [Display(Name = "در تاریخ")]
            public DateTime Date { get; set; }

            [Required(ErrorMessage = "لطفا ساعت شروع را وارد نمایید.")]
            [Display(Name = "از ساعت")]
            public DateTime StartTime { get; set; }

            [Required(ErrorMessage = "لطفا ساعت پایان را وارد نمایید.")]
            [Display(Name = "تا ساعت")]
            public DateTime EndTime { get; set; }

            [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
            [Display(Name = "زمان استراحت میان هر ویزیت")]
            [Range(5, 86400, ErrorMessage = "عدد مورد نظر در بازه اعداد مجاز نمی‌باشد.")]
            public int RestPerPatient { get; set; }

            [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
            [Display(Name = "حداکثر زمان اضافه")]
            public int MaximumExtraMinute { get; set; }
        }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                DateTime from = new DateTime(Schedule.Date.Year, Schedule.Date.Month, Schedule.Date.Day,
                    Schedule.StartTime.Hour, Schedule.StartTime.Minute, Schedule.StartTime.Second,
                    new PersianCalendar());
                DateTime to = new DateTime(Schedule.Date.Year, Schedule.Date.Month, Schedule.Date.Day,
                    Schedule.EndTime.Hour, Schedule.EndTime.Minute, Schedule.EndTime.Second,
                    new PersianCalendar());
                //error handling
                if (DateTime.Compare(to, from) < 0)
                {
                    ModelState.AddModelError("", "ساعت شروع باید کوچکتر از ساعت پایان باشد");
                    return Page();
                }
                else if (DateTime.Compare(DateTime.Now, from) > 0)
                {
                    ModelState.AddModelError("", "از زمان انتخابی نباید گذشته باشد!");
                    return Page();
                }
                //convert to small dates
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var doctor = _db.Doctors.FirstOrDefault(d => d.UserId == userId);
                int visitTime = doctor.VisitTimeSpanInMinute;
                while (DateTime.Compare(to, from) > 0)
                {
                    if (!_db.Schedules.Where(s => s.DoctorId == doctor.Id)
                        .Any(s => DateTime.Compare(from, s.EndDate) < 0 && DateTime.Compare(from, s.StartDate) >= 0
                        || DateTime.Compare(from.AddMinutes(visitTime), s.EndDate) <= 0 && DateTime.Compare(from.AddMinutes(visitTime), s.StartDate) > 0))
                    {
                        _db.Schedules.Add(new Schedule()
                        {
                            DoctorId = doctor.Id,
                            StartDate = from,
                            EndDate = from.AddMinutes(visitTime),
                            MaximumExtraMinute = Schedule.MaximumExtraMinute
                        });
                    }
                    from = from.AddMinutes(visitTime + Schedule.RestPerPatient);
                }
                _db.SaveChanges();
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
