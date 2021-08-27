using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hospital.Pages.Admin.DoctorRequest
{
    public class ShowMessageModel : PageModel
    {
        private ApplicationDbContext _db;

        public ShowMessageModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public DoctorTicket DoctorTicket { get; set; }
        public IActionResult OnGet(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            int? idInt;
            try
            {
                idInt = BitConverter.ToInt32(Convert.FromBase64String(id), 0);
            }
            catch
            {
                return NotFound();
            }

            DoctorTicket = _db.DoctorTickets.FirstOrDefault(d => d.Id == idInt);
            if (DoctorTicket == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
