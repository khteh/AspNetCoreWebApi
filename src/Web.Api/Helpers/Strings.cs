using System.Text.RegularExpressions;
namespace Web.Api.Helpers;
public static class Strings
{
    public static string RemoveAllNonPrintableCharacters(string target) => Regex.Replace(target, @"\p{C}+", string.Empty);
}