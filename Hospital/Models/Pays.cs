using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class Pays
    {
        [Key]
        public int Id { get; set; }

        public int Amount { get; set; }
        public string TransId { get; set; }
        public string FactorNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string CardNumber { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
    }
}
