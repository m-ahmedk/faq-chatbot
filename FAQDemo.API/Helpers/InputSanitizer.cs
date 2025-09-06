using System.Text.RegularExpressions;
using Ganss.Xss; // If you want HTML sanitization

namespace FAQDemo.API.Helpers
{
    public static class InputSanitizer
    {
        private static readonly HtmlSanitizer _htmlSanitizer = new HtmlSanitizer();

        public static string? Clean(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            // Trim and collapse multiple spaces into one
            string cleaned = Regex.Replace(input.Trim(), @"\s+", " ");

            // Lowercase emails
            if (Regex.IsMatch(cleaned, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                cleaned = cleaned.ToLowerInvariant();

            // Sanitize HTML tags
            cleaned = _htmlSanitizer.Sanitize(cleaned);

            return cleaned;
        }
    }
}
