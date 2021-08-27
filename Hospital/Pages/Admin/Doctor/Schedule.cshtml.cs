using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.Models;
using Hospital.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace Hospital.Pages.Admin.Doctor
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class ScheduleModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ScheduleModel(Hospital.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        
        public IList<ScheduleViewModel> input { get; set; }
        public string DoctorFullName { get; set; }
        public class ScheduleViewModel
        {
            public Schedule Schedule { get; set; }
            public string FullName { get; set; } 
        }

        public async Task OnGetAsync(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            DoctorFullName = doctor.FirstName + " " + doctor.LastName;

            input = new List<ScheduleViewModel>();
            var scheduleList = _context.Schedules
                .Where(s => s.DoctorId == id && DateTime.Compare(DateTime.Now, s.EndDate) <= 0)
                .Include(d => d.Doctor).ToList();
            foreach (var item in scheduleList)
            {
                input.Add(new ScheduleViewModel()
                {
                    Schedule = item
                });
                var schedule = input
                    .FirstOrDefault(i => i.Schedule.Id == item.Id);
                var endDate = schedule.Schedule.StartDate.AddMinutes(doctor.VisitTimeSpanInMinute);
                if (DateTime.Compare(schedule.Schedule.EndDate, endDate) != 0)
                {
                    schedule.Schedule.EndDate = endDate;
                    _context.Update(schedule.Schedule);
                    await _context.SaveChangesAsync();
                }
                if (item.UserId != null)
                {
                    var userInfo = await _context.UserInfos.FindAsync(item.UserId);

                    schedule.FullName = userInfo.Name + " " + userInfo.LastName;
                }
            }
        }
    }
}
