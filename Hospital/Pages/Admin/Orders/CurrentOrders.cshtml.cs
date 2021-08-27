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

namespace Hospital.Pages.Admin.Orders
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class CurrentOrdersModel : PageModel 
    {
        private ApplicationDbContext _db;

        public CurrentOrdersModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public IList<ScheduleViewModel> Input { get; set; }
        
        public class ScheduleViewModel
        {
            public Schedule Schedule { get; set; }
            public string UserFullName { get; set; }
            public string DoctorFullName { get; set; }
        }
        public void OnGet()
        {
            Input = new List<ScheduleViewModel>();
            var schedules = _db.Schedules.Where(s => DateTime.Compare(DateTime.Now, s.EndDate) <= 0
                                                     && s.UserId != null)
                                                    .Include(s => s.Doctor).ToList();
            foreach (var item in schedules)
            {
                string doctorFullName = item.Doctor.FirstName + " " + item.Doctor.LastName;
                var userInfo = _db.UserInfos.FirstOrDefault(u => u.Id == item.UserId);
                string userFullName = userInfo != null ? $"{userInfo.Name} {userInfo.LastName}" : "---";
                Input.Add(new ScheduleViewModel()
                {
                    Schedule = item,
                    DoctorFullName = doctorFullName,
                    UserFullName = userFullName
                });
            }
        }
    }
}
