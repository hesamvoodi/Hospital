using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class DoctorTicket
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [Display(Name = "پیام")]
        public string Message { get; set; }

        [Display(Name = "فایل‌ها")]
        public string FilesUpload { get; set; }
        
        [Display(Name = "تایید شده")]
        public bool? IsAccepted { get; set; }
    }
}
