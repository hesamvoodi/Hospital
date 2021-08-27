using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Pages
{
    public class DoctorListModel : PageModel
    {
        private ApplicationDbContext _db;

        public DoctorListModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<Models.Doctor> Doctors { get; set; }

        public string DoctorNameSearch { get; set; }

        public void OnGet(int? doctorGroupSearch, string doctorNameSearch = null)
        {
            Doctors = _db.Doctors.Include(d => d.DoctorGroup).ToList();
            ViewData["GroupId"] = new SelectList(_db.DoctorGroups, "Id", "Name");
            if (doctorGroupSearch != null)
            {
                Doctors = Doctors.Where(d => d.GroupId == doctorGroupSearch).ToList();
                ViewData["GroupId"] = new SelectList(_db.DoctorGroups, "Id", "Name", doctorGroupSearch);
            }

            if (doctorNameSearch != null)
            {
                Doctors = Doctors.Where(d => $"{d.FirstName} {d.LastName}".Contains(doctorNameSearch)).ToList();
                DoctorNameSearch = doctorNameSearch;
            }
        }
    }
}
