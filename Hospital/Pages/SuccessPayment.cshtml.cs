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
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hospital.Pages
{
    [Authorize(Roles = SD.PatientEndUser)]
    public class SuccessPaymentModel : PageModel
    {
        private ApplicationDbContext _db;

        public SuccessPaymentModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool IsSucceed { get; set; }
        public Schedule Schedule { get; set; }
        public int Price { get; set; }
        public string FactorNumber { get; set; }
        public IActionResult OnGet(int? scheduleId, int? status, string token)
        {
            if (scheduleId == null ||
                status == null ||
                string.IsNullOrEmpty(token))
            {
                return NotFound();
            }
            var schedule = _db.Schedules.Include(s => s.Doctor.DoctorGroup).FirstOrDefault(s => s.Id == scheduleId);
            if (schedule == null)
                return NotFound();
            if (status == 1)
            {
                //verify api
                NameValueCollection collection = new NameValueCollection
                {
                    {"api", SD.ApiKey},
                    {"token", token}
                };
                WebClient postClient = new WebClient();

                byte[] postResult;
                try
                {
                    postResult = postClient.UploadValues("https://pay.ir/pg/verify", "POST", collection);
                }
                catch
                {
                    Schedule = _db.Schedules.Find(scheduleId.Value);
                    Price = schedule.Doctor.VisitPrice;
                    IsSucceed = false;
                    return Page();
                }

                //get last result
                string postResponse = System.Text.Encoding.Default.GetString(postResult);

                var jsPostResult = (JObject)JsonConvert.DeserializeObject(postResponse);

                //check result
                if (jsPostResult == null)
                {
                    Schedule = schedule;
                    Price = schedule.Doctor.VisitPrice;
                    IsSucceed = false;
                    return Page();
                }

                string transId = jsPostResult["transId"].ToString();
                string amount = jsPostResult["amount"].ToString();
                string cardNumber = jsPostResult["cardNumber"].ToString();
                string factorNumber = jsPostResult["factorNumber"].ToString();
                string message = jsPostResult["message"].ToString();
                string phoneNumber = jsPostResult["mobile"].ToString();

                

                if (jsPostResult["status"]?.ToString() == "0")
                {
                    Schedule = schedule;
                    Price = schedule.Doctor.VisitPrice;
                    IsSucceed = false;
                    return Page();
                }
                if (_db.Pays.Any(p => p.TransId == transId))
                {
                    Schedule = schedule;
                    Price = Convert.ToInt32(amount);
                    IsSucceed = true;
                    FactorNumber = factorNumber;
                    return Page();
                }

                //After everything succeed

                _db.Pays.Add(new Pays()
                {
                    TransId = transId,
                    Amount = Convert.ToInt32(amount),
                    CardNumber = cardNumber,
                    FactorNumber = factorNumber,
                    Message = message,
                    PhoneNumber = phoneNumber,
                    DateTime = DateTime.Now
                });
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                schedule.UserId = _db.UserInfos.FirstOrDefault(u => u.UserId == userId).Id;
                int totalPrice = Convert.ToInt32(jsPostResult["amount"].ToString());
                int extraMinute = (totalPrice - schedule.Doctor.VisitPrice) / schedule.Doctor.ExtraTimePricePerMinute;
                if (extraMinute <= schedule.MaximumExtraMinute)
                {
                    schedule.ExtraMinute = extraMinute;
                }

                _db.Update(schedule);
                _db.SaveChanges();
                IsSucceed = true;
                Schedule = schedule;
                Price = Convert.ToInt32(amount);
                FactorNumber = factorNumber;
                return Page();
            }

            Schedule = schedule;
            Price = schedule.Doctor.VisitPrice;
            IsSucceed = false;
            return Page();
        }
    }
}
