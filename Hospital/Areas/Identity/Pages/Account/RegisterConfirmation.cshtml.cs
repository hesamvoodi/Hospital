using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Models;
using Hospital.Utilities;
using Kavenegar.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Hospital.Areas.Identity.Pages.Account
{
    public class RegisterConfirmationModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterConfirmationModel(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<RegisterModel> logger)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [BindProperty]
        public int Id { get; set; }

        [TempData]
        public string RegisterError { get; set; }


        //delete this
        public string DemoCode { get; set; }

        public int RemainingTime { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [Display(Name = "کد تایید")]
        public string Number { get; set; }

        public IActionResult OnGet(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            Id = id;
            var phone = _db.PhoneNumberTokens.Find(id);
            //delete this
            DemoCode = phone.Token;


            RemainingTime = SD.RegisterExpiration - Dates.CompareDateFromSecond(DateTime.Now, phone.RequestDate);
            if (RemainingTime < 0)
                RemainingTime = 0;
            if (Dates.CompareDateFromSecond(DateTime.Now, phone.RequestDate) > SD.RegisterExpiration)
            {
                ModelState.AddModelError("انقضای کد.", "زمان استفاده از کد به پایان رسیده روی ارسال مجدد کد کلیک نمایید.");
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                if (Id == 0 || String.IsNullOrEmpty(Number))
                {
                    return NotFound();
                }

                var phoneTokenEntity = _db.PhoneNumberTokens.Find(Id);
                if (CheckPhoneToken(phoneTokenEntity, Number, SD.RegisterExpiration))
                {
                    string password = HttpContext.Session.GetString("RegisterPassword");
                    if (String.IsNullOrEmpty(password))
                    {
                        RegisterError = "زمان ثبت نام به پایان رسید لطفا دوباره ثبت نام کنید.";
                        return RedirectToPage("register");
                    }

                    var user = new IdentityUser
                    {
                        UserName = phoneTokenEntity.PhoneNumber,
                        PhoneNumber = phoneTokenEntity.PhoneNumber,
                        PhoneNumberConfirmed = true
                    };
                    var result = _userManager.CreateAsync(user, password).Result;
                    if (result.Succeeded || _userManager.FindByNameAsync(user.UserName) != null)
                    {
                        _logger.LogInformation("کاربر حساب جدیدی ساخت.");
                        var usr = _db.Users.FirstOrDefault(u => u.UserName == user.UserName);
                        if (!_roleManager.RoleExistsAsync(SD.PatientEndUser).Result)
                        {
                            var roleRes =_roleManager.CreateAsync(new IdentityRole(SD.PatientEndUser)).Result;
                        }
                        if (!_roleManager.RoleExistsAsync(SD.AdminEndUser).Result)
                        {
                            var roleRes = _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser)).Result;
                        }
                        if (!_roleManager.RoleExistsAsync(SD.DoctorEndUser).Result)
                        {
                            var roleRes = _roleManager.CreateAsync(new IdentityRole(SD.DoctorEndUser)).Result;
                        }

                        var identityResult = _userManager.AddToRoleAsync(user, SD.PatientEndUser).Result; 
                        return Partial("RegisterFinished");
                    }
                    RegisterError = "ثبت نام با مشکل مواجه شد لطفا دوباره تلاش کنید.";
                    return RedirectToPage("register");
                }

                var phone = _db.PhoneNumberTokens.Find(Id);
                RemainingTime = SD.RegisterExpiration - Dates.CompareDateFromSecond(DateTime.Now, phone.RequestDate);
                if (RemainingTime < 0)
                    RemainingTime = 0;
                if (Dates.CompareDateFromSecond(DateTime.Now, phone.RequestDate) > SD.RegisterExpiration)
                {
                    ModelState.AddModelError("انقضای کد.", "زمان استفاده از کد به پایان رسیده روی ارسال مجدد کد کلیک نمایید.");
                }
                return Page();
            }
            var phone2 = _db.PhoneNumberTokens.Find(Id);
            RemainingTime = SD.RegisterExpiration - Dates.CompareDateFromSecond(DateTime.Now, phone2.RequestDate);
            if (RemainingTime < 0)
                RemainingTime = 0;
            return Page();
        }

        public bool CheckPhoneToken(PhoneNumberToken phoneEntity, string inputNumber, int tokenExpiration)
        {
            if (phoneEntity.Token == inputNumber &&
                Dates.CompareDateFromSecond(DateTime.Now, phoneEntity.RequestDate) <= tokenExpiration)
            {
                return true;
            }
            else if (Dates.CompareDateFromSecond(DateTime.Now, phoneEntity.RequestDate) > tokenExpiration)
            {
                ModelState.AddModelError("انقضای کد.", "زمان استفاده از کد به پایان رسیده روی ارسال مجدد کد کلیک نمایید.");
            }
            else
            {
                ModelState.AddModelError("کد اشتباه.", "کد وارد شده اشتباه است.");
            }

            return false;
        }

        public IActionResult OnPostSendCode()
        {
            var phoneTokenEntity = _db.PhoneNumberTokens.Find(Id);
            if (Dates.CompareDateFromSecond(DateTime.Now, phoneTokenEntity.RequestDate) > SD.RegisterExpiration)
            {
                string phoneToken = PhoneNumber.GeneratePhoneNumberToken(6);
                DateTime dateNow = DateTime.Now;
                var phone = _db.PhoneNumberTokens.Add(new PhoneNumberToken()
                {
                    PhoneNumber = phoneTokenEntity.PhoneNumber,
                    RequestDate = dateNow,
                    Token = phoneToken
                });
                _db.SaveChanges();

                /*try
                {
                    Kavenegar.KavenegarApi api = new Kavenegar.KavenegarApi("4558447776325478446658554556764D544350494F353731594A47494849784B6C57796D55306C747850673D");
                    var result = api.Send("1000596446", phone.Entity.PhoneNumber, $"کد فعالسازی شما برای سایت بیمارستان: {phoneToken}");
                }
                catch (ApiException ex)
                {
                    // در صورتی که خروجی وب سرویس 200 نباشد این خطارخ می دهد.
                    ModelState.AddModelError("خطای پیامک فرستادن", "Message : " + ex.Message);
                    return Page();
                }
                catch (HttpException ex)
                {
                    // در زمانی که مشکلی در برقرای ارتباط با وب سرویس وجود داشته باشد این خطا رخ می دهد
                    ModelState.AddModelError("خطای پیامک فرستادن", "Message : " + ex.Message);
                    return Page();
                }*/
                return RedirectToPage("RegisterConfirmation", new { id = phone.Entity.Id });
            }

            ModelState.AddModelError("", "زمان ارسال کد تمام نشده لطفا منتظر بمانید.");
            return RedirectToPage(new { id = phoneTokenEntity.Id });
        }
    }
}
