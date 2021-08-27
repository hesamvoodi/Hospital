using System;
using System.Collections.Generic;
using System.IO;
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

namespace Hospital.Pages.Admin.Doctor
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(Hospital.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Doctor Doctor { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Doctor = await _context.Doctors
                .Include(d => d.DoctorGroup)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Doctor == null)
            {
                return NotFound();
            }
            ViewData["GroupId"] = new SelectList(_context.DoctorGroups, "Id", "Name", Doctor.Id);
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(IFormFile imgUp)
        {
            if (!ModelState.IsValid)
            {
                ViewData["GroupId"] = new SelectList(_context.DoctorGroups, "Id", "Name", Doctor.Id);
                return Page();
            }
            //image
            if (imgUp != null)
            {
                if (imgUp.Length > 0 && imgUp.Length < 2097152)
                {
                    string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/DoctorImage");
                    //delete last image
                    if (!String.IsNullOrEmpty(Doctor.ImageName) &&
                        System.IO.File.Exists(deletePath + "/" + Doctor.ImageName))
                    {
                        System.IO.File.Delete(deletePath + "/" + Doctor.ImageName);
                    }
                    //save new image
                    Doctor.ImageName = Guid.NewGuid() + Path.GetExtension(imgUp.FileName);
                    string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/DoctorImage", Doctor.ImageName);
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await imgUp.CopyToAsync(stream);
                    }
                }
                else if(imgUp.Length > 2097152)
                {
                    ModelState.AddModelError("خطا در آپلود فایل", "حجم فایل باید کمتر از 2 مگابایت باشد.");
                    ViewData["GroupId"] = new SelectList(_context.DoctorGroups, "Id", "Name", Doctor.Id);
                    return Page();
                }
            }
            _context.Doctors.Update(Doctor);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }
    }
}
