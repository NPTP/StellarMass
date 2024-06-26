using System.Linq;
using System.Text.RegularExpressions;

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

        public static string LowercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            
            string lower = s.ToLower();
            if (s.Length == 1) return lower;
            return lower[0] + s[1..];
        }
        
        public static string AlphaNumericCharactersOnly(this string s)
        {
            return Regex.Replace(s, "[^a-zA-Z0-9]", string.Empty);
        }
        
        public static string LeadingWhitespace(this string s)
        {
            return s == string.Empty ? string.Empty : s.Replace(s.TrimStart(), string.Empty);
        }
    }
}