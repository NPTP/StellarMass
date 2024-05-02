using System.Linq;

namespace StellarMass.Utilities.Extensions
{
    public static class StringExtensions
    {
        public static string AllWhitespaceTrimmed(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            
            return new string(s.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }

        public static string CapitalizeFirst(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            
            string upper = s.ToUpper();
            if (s.Length == 1) return upper;
            return upper[0] + s[1..];
        }
    }
}