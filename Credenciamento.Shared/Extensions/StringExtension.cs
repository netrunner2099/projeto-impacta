namespace Credenciamento.Shared.Extensions;

public static class StringExtension
{
    // Função que remove a máscara de um CPF, CNPJ, telefone ou CEP, deixando apenas os dígitos
    public static string MaskRemove(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return new string(value.Where(char.IsDigit).ToArray());
    }

    // função para aplicar máscara de cpf em uma string
    public static string MaskCpf(this string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length != 11)
            return value;

        return Convert.ToUInt64(value).ToString(@"000\.000\.000\-00");
    }

}


