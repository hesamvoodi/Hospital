using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Models;
using Hospital.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        public List<DoctorGroup> DoctorGroups { get; set; } 
        public List<Models.Doctor> Doctors { get; set; }
        public int PatientCount { get; set; }
        public int DoctorCount { get; set; }
        public void OnGet()
        {
            DoctorGroups = _db.DoctorGroups.Include(d => d.Doctor).Take(6).ToList();
            Doctors = _db.Doctors.Include(d => d.DoctorGroup).Take(3).ToList();
            PatientCount = _userManager.GetUsersInRoleAsync(SD.PatientEndUser).Result.Count;
            DoctorCount = _userManager.GetUsersInRoleAsync(SD.DoctorEndUser).Result.Count;
        }
    }
}
