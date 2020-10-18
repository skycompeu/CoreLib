using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Extensions {
    public static class ObjectExtensions {
        public static bool IsNull(this object @object) {
            bool isNull = false;
            if (@object == null) {
                isNull = true;
            }
            return isNull;
        }

        public static bool IsNotNull(this object @object) {
            return !IsNull(@object);
        }

    }
}
