using System.Text.RegularExpressions;

namespace ServerBox.Web.Extensions;

public static partial class StringExtensions
{
    public static string CapitalizeFirst(this string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        if (str.Length == 1) return str.ToUpper();
        return str.Substring(0, 1).ToUpper() + str.Substring(1);
    }

    [GeneratedRegex(
        "^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$",
        RegexOptions.IgnoreCase, "zh-CN")]
    private static partial Regex EmailRegex();

    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;
        email = email.Trim();
        var result = EmailRegex().IsMatch(email);
        return result;
    }

    [GeneratedRegex("^1\\d{10}$")]
    private static partial Regex PhoneRegex();

    public static bool IsValidPhoneNumber(this string phone)
    {
        if (phone.Length < 11)
        {
            return false;
        }

        var regex = PhoneRegex();
        return regex.IsMatch(phone);
    }
}