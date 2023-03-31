using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace RPU.Utility;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumType)
    {
        return enumType
            .GetType()
            .GetMember(enumType.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()
            .GetName();
    }
}
