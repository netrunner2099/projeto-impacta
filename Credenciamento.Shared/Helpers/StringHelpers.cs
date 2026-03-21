namespace Credenciamento.Shared.Helpers;

public static class StringHelpers
{

    public static string ToBase64(string input)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
    }

    public static string FromBase64(string input) { 
        return Encoding.UTF8.GetString(Convert.FromBase64String(input));
    }
}


