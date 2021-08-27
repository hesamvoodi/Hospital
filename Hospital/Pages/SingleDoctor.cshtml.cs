using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Pages
{
    public class SingleDoctorModel : PageModel
    {
        private ApplicationDbContext _db;

        public SingleDoctorModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public Models.Doctor Doctor { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [Display(Name = "روز ویزیت")]
        public string ScheduleId { get; set; }

        public SelectList DaysList { get; set; }

        public IActionResult OnGet(int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }

            Doctor = _db.Doctors.Include(d => d.DoctorGroup).FirstOrDefault(d => d.Id == id);
            var schedules = _db.Schedules.Where(s => s.UserId == null
                                                     && s.DoctorId == Doctor.Id
                                                     && DateTime.Compare(DateTime.Now, s.StartDate) < 0).OrderBy(s => s.StartDate).ToList();
            foreach (var item in schedules.ToList())
            {
                var sameDaySchedules = schedules.Where(s => Dates.IsInSameDay(s.StartDate, item.StartDate)).ToList();
                if (sameDaySchedules.Count() > 1)
                {
                    sameDaySchedules.Remove(item);
                    foreach (var same in sameDaySchedules)
                    {
                        schedules.Remove(same);
                    }
                }
            }


            List<object> newList = new List<object>();
            foreach (var item in schedules)
            {
                newList.Add(new
                {
                    Id = Convert.ToBase64String(BitConverter.GetBytes(item.Id)),
                    Date = item.StartDate.ToShamsiDate()
                });
            }

            DaysList = new SelectList(newList, "Id", "Date");

            return Page();
        }
    }
}
