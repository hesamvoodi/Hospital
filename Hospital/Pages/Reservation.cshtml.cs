using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Models;
using Hospital.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Pages
{
    [Authorize(Roles = SD.PatientEndUser)]
    public class ReservationModel : PageModel
    {
        private ApplicationDbContext _db;

        public ReservationModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<Schedule> Schedules { get; set; }
        public Models.Doctor Doctor { get; set; }

        public IActionResult OnGet(string scheduleId)
        {
            if (scheduleId == null)
            {
                return NotFound();
            }
            int? id;
            try
            {
                id = BitConverter.ToInt32(Convert.FromBase64String(scheduleId), 0);
            }
            catch
            {
                return NotFound();
            }

            if (id == 0)
            {
                return NotFound();
            }
            var findSchedule = _db.Schedules.FirstOrDefault(s => s.Id == id);
            if (findSchedule == null)
            {
                return NotFound();
            }
            Schedules = _db.Schedules.Where(s =>
                s.StartDate.Year == findSchedule.StartDate.Year && 
                s.StartDate.Month == findSchedule.StartDate.Month &&
                s.StartDate.Day == findSchedule.StartDate.Day &&
                DateTime.Compare(DateTime.Now, s.StartDate) < 0 &&
                s.UserId == null).Include(s => s.Doctor.DoctorGroup).ToList();
            Doctor = _db.Doctors.FirstOrDefault(d => d.Id == findSchedule.DoctorId);
            return Page();
        }
    }
}
