using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.Models;
using Hospital.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Hospital.Pages.Admin.Doctor
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<DoctorViewModel> input { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public class DoctorViewModel
        {
            public Models.Doctor Doctor { get; set; }
            public string Phone { get; set; }
        }

        public void OnGetAsync(int pageId = 1, string searchName = null)
        {
            input = new List<DoctorViewModel>();


            int itemPerPage = SD.ItemPerPageAdminDoctor;
            StringBuilder param = new StringBuilder();
            param.Append("/Doctor?pageId=:");
            var doctors = _context.Doctors.Include(d => d.DoctorGroup).AsEnumerable();
            if (searchName != null)
            {
                doctors = doctors.Where(d => (d.FirstName + " " + d.LastName).Contains(searchName));
                param.Append("&searchName=");
                param.Append(searchName);
            }

            var pageDoctors = doctors.Skip((pageId - 1) * itemPerPage).Take(itemPerPage).ToList();
            foreach (var item in pageDoctors)
            {
                input.Add(new DoctorViewModel()
                {
                    Doctor = item,
                    Phone = _userManager.FindByIdAsync(item.UserId).Result.PhoneNumber
                });
            }

            var total = _context.Doctors.ToList();
            PagingInfo = new PagingInfo()
            {
                CurrentPage = pageId,
                ItemPerPage = itemPerPage,
                TotalItems = doctors.Count(),
                UrlParam = param.ToString()
            };

        }
    }
}
