using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class OrderHistoryModel : PageModel
    {
        private ApplicationDbContext _db;

        public OrderHistoryModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public IList<ScheduleViewModel> Input { get; set; }
        public PagingInfo PagingInfo { get; set; }

        public class ScheduleViewModel
        {
            public Schedule Schedule { get; set; }
            public string UserFullName { get; set; }
            public string DoctorFullName { get; set; }
        }
        public void OnGet(string searchUser = null, string searchDoctor = null, int pageId = 1)
        {
            //schedule listing
            var schedules = _db.Schedules.Where(s => DateTime.Compare(DateTime.Now, s.EndDate) >= 0
                                                     && s.UserId != null)
                .Include(s => s.Doctor).ToList();
            if (searchUser != null)
            {
                foreach (var item in schedules.ToList())
                {
                    if (item.UserId != null)
                    { 
                        var userInfo = _db.UserInfos.FirstOrDefault(u => u.Id == item.UserId);
                        if (!(userInfo.Name + " " + userInfo.LastName).Contains(searchUser))
                        {
                            schedules.Remove(item);
                        }
                    }
                    else
                    {
                        schedules.Remove(item);
                    }

                }
            }

            if (searchDoctor != null)
            {
                schedules = schedules.Where(s => (s.Doctor.FirstName + "" + s.Doctor.LastName).Contains(searchDoctor))
                    .ToList();
            }
            //paging info
            StringBuilder param = new StringBuilder();
            param.Append("OrderHistory?PageId=:");
            PagingInfo = new PagingInfo()
            {
                CurrentPage = pageId,
                ItemPerPage = SD.ItemPerPageOrderHistory,
                TotalItems = schedules.Count,
                UrlParam = param.ToString()
            };

            //edit schedules for paging and add it to viewmodel
            Input = new List<ScheduleViewModel>();
            schedules = schedules.Skip((pageId - 1) * PagingInfo.ItemPerPage)
                .Take(PagingInfo.ItemPerPage).ToList();
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
