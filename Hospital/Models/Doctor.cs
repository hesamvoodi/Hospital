using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Hospital.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [Display(Name = "تخصص")]
        public int GroupId { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [Display(Name = "قیمت ویزیت")]
        public int VisitPrice { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [Display(Name = "قیمت هر دقیقه زمان اضافه")]
        public int ExtraTimePricePerMinute { get; set; }

        [Display(Name = "عکس دکتر")]
        public string ImageName { get; set; }

        [Display(Name = "رزومه")]
        public string Description { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [Display(Name = "مدت زمان ویزیت")]
        public int VisitTimeSpanInMinute { get; set; }

        [ForeignKey("GroupId")]
        [Display(Name = "تخصص")]
        public virtual DoctorGroup DoctorGroup { get; set; }

        public virtual List<Schedule> Schedules { get; set; }
    }
}
