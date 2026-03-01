namespace Credenciamento.Shared.Extensions;
public static class StringExtension
{
    /// <summary>
    /// Função que remove a máscara de um CPF, CNPJ, telefone ou CEP, deixando apenas os dígitos 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string MaskRemove(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return new string(value.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Função para aplicar máscara de cpf em uma string 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string MaskCpf(this string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length != 11)
            return value;

        return Convert.ToUInt64(value).ToString(@"000\.000\.000\-00");
    }


    /// <summary>
    /// Retorna a String com as primeiras letras maiúsculas.
    /// </summary>
    /// <param name="value">Texto a formatar.</param>
    /// <returns>String com a primeira letra maiúscula.</returns>
    public static String ToTitleCase(this string value)
    {
        String Retorno = "";
        if (!string.IsNullOrEmpty(value) && value.Length > 0)
        {
            Retorno = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
        }
        return Retorno;
    }
}


