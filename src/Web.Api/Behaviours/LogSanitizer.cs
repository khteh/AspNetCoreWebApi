using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using Web.Api.Extensions;
namespace Web.Api.Behaviours;

/// <summary>
/// Describes MediatR requests / responses for logging without leaking credentials.
/// Records' compiler-generated ToString() prints every property (passwords, access / refresh tokens, keys), so it must never be logged directly.
/// Any property whose name contains password, token, secret or key is masked; nested objects are reduced to their type name.
/// New commands are covered automatically as long as sensitive members follow that naming.
/// </summary>
public static partial class LogSanitizer
{
    public const string Mask = "***";
    [GeneratedRegex("password|token|secret|key", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex SensitiveName();
    public static bool IsSensitive(string memberName) => SensitiveName().IsMatch(memberName);

    public static string Describe(object? value)
    {
        if (value == null)
            return "null";
        IEnumerable<string> members = value.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetIndexParameters().Length == 0 && p.Name != "EqualityContract")
            .Select(p => $"{p.Name} = {(IsSensitive(p.Name) ? Mask : DescribeValue(SafeGet(p, value)))}");
        return $"{value.GetGenericTypeName()} {{ {string.Join(", ", members)} }}";
    }
    private static object? SafeGet(PropertyInfo property, object instance)
    {
        try
        {
            return property.GetValue(instance);
        }
        catch
        {
            return "<unavailable>";
        }
    }
    private static string DescribeValue(object? value) => value switch
    {
        null => "null",
        string s => s,
        Enum or Guid or DateTime or DateTimeOffset or TimeSpan => value.ToString()!,
        _ when value.GetType().IsPrimitive || value is decimal => value.ToString()!,
        ICollection c => $"[{c.Count} item(s)]",
        IEnumerable => "[...]",
        _ => value.GetGenericTypeName()
    };
}
