using System.Linq;

namespace StellarMass.Utilities.Extensions
{
    public static class StringExtensions
    {
        public static string AllWhitespaceTrimmed(this string s)
        {
            return new string(s.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }
    }
}