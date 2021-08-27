using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Hospital.Data;
using Hospital.Models;
using Hospital.Utilities;
using Microsoft.AspNetCore.Identity;

namespace Hospital.Pages.Users
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private ApplicationDbContext _db;
        private UserManager<IdentityUser> _userManager;

        public IndexModel(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }


        public List<UserViewModel> UserViewModels { get; set; }
        public PagingInfo PagingInfo { get; set; }

        public class UserViewModel
        {
            public IdentityUser IdentityUser { get; set; }
            public string Role { get; set; }
            public string FullName { get; set; }
        }

        public IActionResult OnGet(int pageId = 1, string searchPhone = null, string searchRole = null)
        {
            int itemPerPage = SD.ItemPerPageAdminUser;
            UserViewModels = new List<UserViewModel>();
            var users = _db.Users.ToList();
            foreach (var item in users)
            {
                string fullName = null;
                string role;
                if (_userManager.IsInRoleAsync(item, SD.PatientEndUser).Result)
                {
                    var userInfo = _db.UserInfos.FirstOrDefault(u => u.UserId == item.Id);
                    if (userInfo != null)
                        fullName = userInfo.Name + " " + userInfo.LastName;
                    role = "کاربر معمولی";
                }
                else if (_userManager.IsInRoleAsync(item, SD.DoctorEndUser).Result)
                {
                    var doctorInfo = _db.Doctors.FirstOrDefault(u => u.UserId == item.Id);
                    if (doctorInfo != null)
                        fullName = doctorInfo.FirstName + " " + doctorInfo.LastName;
                    role = "دکتر";
                }
                else
                {
                    fullName = "Admin";
                    role = "ادمین";
                }

                fullName ??= "Not set";
                UserViewModels.Add(new UserViewModel()
                {
                    IdentityUser = item,
                    FullName = fullName,
                    Role = role
                });
            }

            //Paging info
            StringBuilder param = new StringBuilder();
            param.Append("/Users?PageId=:");
            if (searchPhone != null)
            {
                UserViewModels = UserViewModels
                    .Where(u => u.IdentityUser.PhoneNumber.Contains(searchPhone))
                    .ToList();
                param.Append("&searchPhone=");
                param.Append(searchPhone);
            }
            if (searchRole != null)
            {
                foreach (var item in UserViewModels.ToList())
                {
                    if (!_userManager.IsInRoleAsync(item.IdentityUser, searchRole).Result)
                    {
                        UserViewModels.Remove(item);
                    }
                }
                param.Append("&searchPhone=");
                param.Append(searchPhone);
            }
            int total = UserViewModels.Count();

            PagingInfo = new PagingInfo()
            {
                CurrentPage = pageId,
                ItemPerPage = itemPerPage,
                TotalItems = total,
                UrlParam = param.ToString()
            };

            //users
            UserViewModels = UserViewModels
                .Skip((pageId - 1) * itemPerPage)
                .Take(itemPerPage)
                .ToList();


            return Page();
        }
    }
}
