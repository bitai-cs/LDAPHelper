using System;
using System.Collections.Generic;
using System.Text;

namespace LdapHelperLib.Extensions
{
	public static class StringExtensions
	{
		public static string ConvertParenthesisCharsToScapedChars(this string input)
		{
			return input.Replace("(", "\\28").Replace(")", "\\29");
		}

		public static string ConvertAsteriskCharsToScapedChars(this string input)
		{
			return input.Replace("*", "\\2A");
		}

		public static string ConvertBackslashCharsToScapedChars(this string input)
		{
			return input.Replace("\\", "\\5C");
		}

		public static string ConvertspecialCharsToScapedChars(this string input)
		{
			return ConvertParenthesisCharsToScapedChars(ConvertAsteriskCharsToScapedChars(ConvertBackslashCharsToScapedChars(input)));
		}
	}
}