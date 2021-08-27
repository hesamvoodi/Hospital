using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Models;
using Hospital.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hospital.Pages
{
    [Authorize(Roles = SD.PatientEndUser)]
    public class FinalApprovalModel : PageModel
    {
        private ApplicationDbContext _db;

        public FinalApprovalModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public Schedule Schedule { get; set; }
        public Models.Doctor Doctor { get; set; }
        public DoctorGroup DoctorGroup { get; set; }
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
            Schedule = _db.Schedules.Include(d => d.Doctor.DoctorGroup).FirstOrDefault(s => s.Id == idInt);
            if (Schedule == null)
            {
                return NotFound();
            }

            Doctor = _db.Doctors.FirstOrDefault(d => d.Id == Schedule.DoctorId);
            DoctorGroup = _db.DoctorGroups.FirstOrDefault(d => d.Id == Doctor.GroupId);

            return Page();
        }

        public IActionResult OnPost(string scheduleId, int extraMinute = 0)
        {
            if (string.IsNullOrEmpty(scheduleId))
            {
                return Redirect($"{Request.Path}?id={Request.Query["id"]}");
            }

            int id;
            try
            {
                id = BitConverter.ToInt32(Convert.FromBase64String(scheduleId), 0);
            }
            catch
            {
                return Redirect($"{Request.Path}?id={Request.Query["id"]}");
            }
            var schedule = _db.Schedules.Include(s => s.Doctor).FirstOrDefault(s => s.Id == id);
            if (schedule == null || schedule.UserId != null)
            {
                return Redirect($"{Request.Path}?id={Request.Query["id"]}");
            }
            if (extraMinute > schedule.MaximumExtraMinute || extraMinute < 0)
            {
                return Redirect($"{Request.Path}?id={Request.Query["id"]}");
            }

            //-----------------------------get first token--------------------------------------

            int totalPrice = schedule.Doctor.VisitPrice + extraMinute * schedule.Doctor.ExtraTimePricePerMinute;
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string phoneNumber = _db.Users.FirstOrDefault(u => u.Id == userId).PhoneNumber;

            NameValueCollection collection = new NameValueCollection
            {
                {"api", SD.ApiKey},
                {"amount", totalPrice.ToString()},
                {"redirect", $"https://localhost:44330/SuccessPayment?scheduleId={id}"},
                {"mobile", phoneNumber}
            };

            WebClient postClient = new WebClient();

            var postResult = postClient.UploadValues("https://pay.ir/pg/send", "POST", collection);

            //get response

            string postResponse = System.Text.Encoding.Default.GetString(postResult);

            var jsPostResult = (JObject)JsonConvert.DeserializeObject(postResponse);

            if (jsPostResult != null && jsPostResult["status"]?.ToString() == "0")
            {
                return Redirect($"{Request.Path}?id={Request.Query["id"]}");
            }

            string token = jsPostResult["token"].ToString();

            //-----------------------------redirect--------------------------------------
            
            return Redirect($"https://pay.ir/pg/{token}");
        }
    }
}
