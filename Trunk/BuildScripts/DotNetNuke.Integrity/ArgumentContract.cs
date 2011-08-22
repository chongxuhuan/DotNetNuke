using System;
using System.Collections.Generic;
using System.Text;
using DotNetNuke.Integrity.Properties;
using System.Globalization;

namespace DotNetNuke.Integrity {
    internal static class ArgumentContract {
        public static void NotNull(object value, string paramName) {
            if (value == null) {
                throw new ArgumentNullException(paramName);
            }
        }
        public static void StringNotNullOrEmpty(string value, string paramName) {
            if (String.IsNullOrEmpty(value)) {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.Error_StringArgumentNullOrEmpty, paramName), paramName);
            }
        }
    }
}
