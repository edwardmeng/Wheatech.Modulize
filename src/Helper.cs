using System;
using System.Globalization;

namespace Wheatech.Modulize
{
    internal static class Helper
    {
        public static bool TryParseCulture(string cultureName, out CultureInfo culture)
        {
            culture = null;
            if (string.IsNullOrEmpty(cultureName) || string.Equals(cultureName, "neutral", StringComparison.OrdinalIgnoreCase)) return true;
            try
            {

                culture = CultureInfo.GetCultureInfo(cultureName);
            }
            catch (CultureNotFoundException)
            {
                return false;
            }
            return true;
        }
    }
}
