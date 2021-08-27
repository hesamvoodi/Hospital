using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Utilities 
{
    public class SD
    {
        public const string AdminEndUser = "Admin";
        public const string DoctorEndUser = "Doctor";
        public const string PatientEndUser = "Patient";
        public const int RegisterExpiration = 120;
        public const int ItemPerPageAdminDoctor = 100;
        public const int ItemPerPageAdminUser = 100;
        public const int ItemPerPageOrderHistory = 100;
        public const string ApiKey = "test";
    }
}
