using System;
using System.Security.Cryptography;

namespace Regul.OlibKey.General
{
    public static class PasswordGenerator
    {
        private static readonly RNGCryptoServiceProvider Rand = new RNGCryptoServiceProvider();
        
        public static string Generate()
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string number = "0123456789";
            const string special = @"~!@#$%^&*():;[]{}<>,.?/\|";
            string other = OlibKeySettings.Instance.GeneratorTextOther;

            string allowed = "";
            string password = "";

            if (OlibKeySettings.Instance.GeneratorAllowLowercase) allowed += lower;
            if (OlibKeySettings.Instance.GeneratorAllowUppercase) allowed += upper;
            if (OlibKeySettings.Instance.GeneratorAllowNumber) allowed += number;
            if (OlibKeySettings.Instance.GeneratorAllowSpecial) allowed += special;
            if (OlibKeySettings.Instance.GeneratorAllowOther) allowed += other;
            if (OlibKeySettings.Instance.GeneratorAllowUnderscore && password.IndexOfAny("_".ToCharArray()) == -1)
            {
                allowed += "_";
                password += "_";
            }

            int minChars = OlibKeySettings.Instance.GenerationCount;
            int numChars = RandomInteger(minChars, minChars);

            while (password.Length < numChars)
            {
                password += allowed.Substring(RandomInteger(0, allowed.Length - 1), 1);
            }

            password = RandomString(password);

            return password;
        }

        private static int RandomInteger(int min, int max)
        {
            uint scale = uint.MaxValue;
            while (scale == uint.MaxValue)
            {
                byte[] fourBytes = new byte[4];
                Rand.GetBytes(fourBytes);
                scale = BitConverter.ToUInt32(fourBytes, 0);
            }

            return (int)(min + ((max - min) * (scale / (double)uint.MaxValue)));
        }
        
        private static string RandomString(string str)
        {
            string result = "";
            while (str.Length > 0)
            {
                int i = RandomInteger(0, str.Length - 1);
                result += str.Substring(i, 1);
                str = str.Remove(i, 1);
            }

            return result;
        }
    }
}