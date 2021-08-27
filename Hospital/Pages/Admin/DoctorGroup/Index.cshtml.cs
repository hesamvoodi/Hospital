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

namespace Hospital.Pages.Admin.DoctorGroup
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly Hospital.Data.ApplicationDbContext _context;

        public IndexModel(Hospital.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.DoctorGroup> DoctorGroup { get;set; }

        public async Task OnGetAsync()
        {
            DoctorGroup = await _context.DoctorGroups.Include(d => d.Doctor).ToListAsync();
        }
    }
}
