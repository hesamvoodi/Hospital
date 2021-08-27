using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Utilities
{
    public static class PhoneNumber
    {
        public static string GeneratePhoneNumberToken(int length)
        {
            Random random = new Random();
            string code = "";
            for (int i = 0; i < length; i++)
            {
                code += random.Next(0, 9);
            }

            return code;
        }
    }
}
