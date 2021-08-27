using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class DoctorGroup
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [Display(Name = "نام تخصص")]
        public string Name { get; set; }


        /// <summary>
        /// Navigation
        /// </summary>
        public virtual List<Doctor> Doctor { get; set; }
    }
}
