using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Extensions {
    public static class EnumExtensions {
        public static T GetAttribute<T>(this Enum value) where T : Attribute {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0
              ? (T)attributes[0]
              : null;
        }


        public static List<Enum> EnumGetValues(this Enum @enum) {
            
            
            //var enums = Enum.GetValues(@enum.GetType());
            return default;
        }
    }
}
