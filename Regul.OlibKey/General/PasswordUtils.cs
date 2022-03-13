using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Regul.OlibKey.General
{
    public static class PasswordUtils
    {
        public static double GetPasswordComplexity(string password)
        {
            if (string.IsNullOrEmpty(password)) return 0;

            double multi0 = 1.0;
            double multi1 = 1.0;
            double multi2 = 1.0;
            double multi3 = 0;

            int score = 0;

            List<char> usedChars = new List<char>();
            foreach (char chr in password.Where(c => !usedChars.Contains(c))) 
                usedChars.Add(chr);

            multi1 = FrequencyFactor(password.ToLower());
            score += password.Length * 15;

            Dictionary<string, double> patterns = new Dictionary<string, double>()
            {
                { @"1234567890", 0.0 },
                { @"[a-z]", 0.1 },
                { @"[ёа-я]", 0.2 },
                { @"[A-Z]", 0.2 },
                { @"[ЁА-Я]", 0.3 },
                { "[!,@#\\$%\\^&\\*?_~=;:'\"<>[]()~`\\\\|/]", 0.4 },
                { @"[¶©]", 0.5 }
            };
            foreach (KeyValuePair<string, double> pattern in patterns)
            {
                if (Regex.Match(password, pattern.Key).Length > 0)
                    multi2 += pattern.Value;
            }

            if (password.Length > 2) multi3 += 0;
            if (password.Length > 4) multi3 += 0.25;
            if (password.Length > 6) multi3 += 0.75;
            if (password.Length > 8) multi3 += 1.0;

            return (score * multi0 * multi1 * multi2);
        }
        
        private static double FrequencyFactor(string password) => 
            Map(new HashSet<char>(password.ToCharArray()).Count, 1.0, password.Length, 0.1, 1);

        private static double Map(
            double value, 
            double fromLower, 
            double fromUpper, 
            double toLower, 
            double toUpper) => 
            toLower + ((value - fromLower) / (fromUpper - fromLower) * (toUpper - toLower));
    }
}