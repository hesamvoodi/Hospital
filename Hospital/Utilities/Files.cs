using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Hospital.Utilities
{
    public static class Files
    {
        public static bool IsImage(this string postedFile)
        {

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile).ToLower() != ".jpg"
                && Path.GetExtension(postedFile).ToLower() != ".png"
                && Path.GetExtension(postedFile).ToLower() != ".gif"
                && Path.GetExtension(postedFile).ToLower() != ".jpeg")
            {
                return false;
            }
            return true;
        }
    }
}
