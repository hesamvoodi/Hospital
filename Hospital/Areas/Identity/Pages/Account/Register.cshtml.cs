using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Models;
using Hospital.Utilities;
using Kavenegar.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Hospital.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _db;

        public RegisterModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ILogger<RegisterModel> logger, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string RegisterError { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
            [Phone]
            [Display(Name = "شماره همراه")]
            public string Phone { get; set; }

            [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
            [StringLength(100, ErrorMessage = "{0} باید بین {2} و {1} کارکتر باشد.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "کلمه عبور")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "تکرار کلمه عبور")]
            [Compare("Password", ErrorMessage = "کلمه عبور و تکرار با هم مطابقت ندارند.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (RegisterError != null)
            {
                ModelState.AddModelError("", RegisterError);
            }
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            var user = new IdentityUser {UserName = Input.Phone, PhoneNumber = Input.Phone};
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return Page();
                }

                
                HttpContext.Session.SetString("RegisterPassword", Input.Password);

                string phoneToken = PhoneNumber.GeneratePhoneNumberToken(6);
                DateTime dateNow = DateTime.Now;
                var phone = await _db.PhoneNumberTokens.AddAsync(new PhoneNumberToken()
                {
                    PhoneNumber = Input.Phone,
                    RequestDate = dateNow,
                    Token = phoneToken
                });
                await _db.SaveChangesAsync();

                /*try
                {
                    Kavenegar.KavenegarApi api = new Kavenegar.KavenegarApi("4558447776325478446658554556764D544350494F353731594A47494849784B6C57796D55306C747850673D");
                    var resul = api.Send("1000596446", Input.Phone, $"کد فعالسازی شما برای سایت بیمارستان: {phoneToken}");
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

                return RedirectToPage("RegisterConfirmation",new {id = phone.Entity.Id});
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
