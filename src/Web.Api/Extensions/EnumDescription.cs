using System.ComponentModel;
using System.Reflection;
namespace Web.Api.Extensions;

public static class EnumDescription
{
    public static string GetDescription(this Enum value)
    {
        Type? type = value.GetType();
        string? name = value != null && type != null ? Enum.GetName(type, value) : string.Empty;
        if (type != null && !string.IsNullOrEmpty(name))
        {
            FieldInfo? field = type.GetField(name);
            if (field != null)
            {
                DescriptionAttribute? attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attr != null)
                    return attr.Description;
            }
        }
        return string.Empty;
    }
}