using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hospital.Data;
using Hospital.Models;
using Hospital.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace Hospital.Pages.Admin.DoctorGroup
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class CreateModel : PageModel
    {
        private readonly Hospital.Data.ApplicationDbContext _context;

        public CreateModel(Hospital.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty] 
        public Models.DoctorGroup DoctorGroup { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.DoctorGroups.Add(DoctorGroup);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
