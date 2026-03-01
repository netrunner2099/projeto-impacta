using System.ComponentModel;

namespace Credenciamento.Shared.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var descriptionAttribute = fieldInfo
            ?.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault() as DescriptionAttribute;

        return descriptionAttribute?.Description ?? value.ToString();
    }
}
