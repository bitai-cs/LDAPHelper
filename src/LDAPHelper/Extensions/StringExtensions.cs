using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LDAPHelper.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceParenthesisCharsToScapedChars(this string input)
        {
            return input.Replace("(", "\\28").Replace(")", "\\29");
        }

        public static string ReplaceAsteriskCharsToScapedChars(this string input)
        {
            return input.Replace("*", "\\2A");
        }

        public static string ReplaceBackslashCharsToScapedChars(this string input)
        {
            return input.Replace("\\", "\\5C");
        }

        public static string ReplaceSpecialCharsToScapedChars(this string input)
        {
            return ReplaceParenthesisCharsToScapedChars(ReplaceAsteriskCharsToScapedChars(ReplaceBackslashCharsToScapedChars(input)));
        }
    }
}