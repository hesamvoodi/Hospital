using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hospital.Models
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Display(Name = "بیمار")]
        public int? UserId { get; set; }

        [Required]
        [Display(Name = "زمان شروع ویزیت")]
        public DateTime StartDate { get; set; }

        [Required] 
        [Display(Name = "زمان پایان ویزیت")]
        public DateTime EndDate { get; set; }

        public int MaximumExtraMinute { get; set; }

        [Display(Name = "وقت اضافه")]
        public int ExtraMinute { get; set; } = 0;

        [ForeignKey("DoctorId")]
        [Display(Name = "دکتر")]
        public Doctor Doctor { get; set; }
    }
}
