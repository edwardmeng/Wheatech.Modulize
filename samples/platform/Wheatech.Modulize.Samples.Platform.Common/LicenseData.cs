using System.Text.RegularExpressions;

namespace Wheatech.Modulize.Samples.Platform.Common
{
    public class LicenseData
    {
        public string FullName { get; set; }

        public string ShortName { get; set; }

        public string Url { get; set; }

        public Regex Pattern { get; set; }
    }
}
