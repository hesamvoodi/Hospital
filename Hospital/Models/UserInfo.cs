using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Models.Enums;

namespace Hospital.Models
{
    public class UserInfo
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [Display(Name = "نام")]
        public string Name { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [Display(Name = "جنسیت")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public Gender Gender { get; set; }

        [Display(Name = "قد به سانتی متر")]
        [Required(ErrorMessage = "لطفا قد خود را وارد نمایید.")]
        public int HeightCm { get; set; }

        [Display(Name = "وزن به کیلوگرم")]
        [Required(ErrorMessage = "لطفا ورن خود را وارد نمایید.")]
        public int WeightKg { get; set; }

        [Display(Name = "گروه خونی")]
        public BloodType? BloodType { get; set; }

        [Display(Name = "سن")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public int Age { get; set; }

        [Display(Name = "سابقه بیماری")]
        public string DiseaseBackground { get; set; }

        [Display(Name = "توضیحات اضافه")]
        public string ExtraDescription { get; set; }

        [Display(Name = "فایل ضمیمه شده")]
        public string FileName { get; set; }
    }
}
