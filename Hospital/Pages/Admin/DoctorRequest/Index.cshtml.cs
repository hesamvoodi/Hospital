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
    public class IndexModel : PageModel
    {
        private ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<DoctorTicketViewModel> DoctorTicketViewModels { get; set; }

        public class DoctorTicketViewModel
        {
            public int Id { get; set; }
            public string PhoneNumber { get; set; }
        }

        public void OnGet()
        {
            var doctorTickets = _db.DoctorTickets.Where(d => d.IsAccepted == null);
            DoctorTicketViewModels = new List<DoctorTicketViewModel>();
            foreach (var doctorTicket in doctorTickets.ToList())
            {
                string phoneNumber = _db.Users.FirstOrDefault(u => u.Id == doctorTicket.UserId).PhoneNumber;
                DoctorTicketViewModels.Add(new DoctorTicketViewModel()
                {
                    Id = doctorTicket.Id,
                    PhoneNumber = phoneNumber
                });
            }
        }
    }
}
