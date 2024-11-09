using System.Text;

namespace Summoner.Utilities
{
    public static class XorScramblerUtility
    {
        public static string Scramble(string s, string xorPad)
        {
            StringBuilder sb = new StringBuilder(s);
            for (int i = 0; i < sb.Length; i++)
            {
                sb[i] = (char)(sb[i] ^ xorPad[i % xorPad.Length]);
            }
            
            return sb.ToString();
        }
    }
}