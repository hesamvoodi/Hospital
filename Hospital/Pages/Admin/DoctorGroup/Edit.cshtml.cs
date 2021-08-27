using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.Models;
using Hospital.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Hospital.Pages.Admin.DoctorGroup
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly Hospital.Data.ApplicationDbContext _context;

        public EditModel(Hospital.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.DoctorGroup DoctorGroup { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DoctorGroup = await _context.DoctorGroups.FirstOrDefaultAsync(m => m.Id == id);

            if (DoctorGroup == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetInt32("id",DoctorGroup.Id);
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            int? id = HttpContext.Session.GetInt32("id");
            if (id == null)
            {
                return Page();
            }
            DoctorGroup.Id = id.Value;
            _context.Attach(DoctorGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorGroupExists(DoctorGroup.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool DoctorGroupExists(int id)
        {
            return _context.DoctorGroups.Any(e => e.Id == id);
        }
    }
}
