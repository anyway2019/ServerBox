namespace ServerBox.Web.Extensions;

public static class StringExtensions
{
    public static string CapitalizeFirst(this string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        if (str.Length == 1) return str.ToUpper();
        return str.Substring(0, 1).ToUpper() + str.Substring(1);
    }
}