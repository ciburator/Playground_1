using System;

namespace ImageToAscii.Helpers
{
    public static class StringHelper
    {
        public static string Reverse(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}