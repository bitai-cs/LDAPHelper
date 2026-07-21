namespace Bitai.LDAPHelper.Extensions
{
    /// <summary>
    /// Provides LDAP-safe string escaping helpers.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Escapes parenthesis characters for LDAP filter usage.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>Escaped text.</returns>
        public static string ReplaceParenthesisCharsToScapedChars(this string input)
        {
            return input.Replace("(", "\\28").Replace(")", "\\29");
        }

        /// <summary>
        /// Escapes asterisk characters for LDAP filter usage.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>Escaped text.</returns>
        public static string ReplaceAsteriskCharsToScapedChars(this string input)
        {
            return input.Replace("*", "\\2A");
        }

        /// <summary>
        /// Escapes backslash characters for LDAP filter usage.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>Escaped text.</returns>
        public static string ReplaceBackslashCharsToScapedChars(this string input)
        {
            return input.Replace("\\", "\\5C");
        }

        /// <summary>
        /// Escapes special LDAP filter characters (backslash, asterisk, parenthesis).
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>Escaped text.</returns>
        public static string ReplaceSpecialCharsToScapedChars(this string input)
        {
            return ReplaceParenthesisCharsToScapedChars(ReplaceAsteriskCharsToScapedChars(ReplaceBackslashCharsToScapedChars(input)));
        }
    }
}
