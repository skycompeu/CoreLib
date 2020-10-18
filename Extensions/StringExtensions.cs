using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreLib.Extensions {
    public static class StringExtensions {


        public static bool StringIsNullOrEmpty(this string @value) {
            return string.IsNullOrEmpty(value);
        }

        public static bool StringIsNullOrWhiteSpace(this string @value) {
            return string.IsNullOrWhiteSpace(value);
        }

        //IsNullOrWhiteSpace


    }
}
