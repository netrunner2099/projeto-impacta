namespace Credenciamento.Shared.Helpers;

public static class CryptHelpers
{
    private const int SaltSize = 16; // 128 bits
    private const int SubkeySize = 32; // 256 bits
    private const int DefaultIterations = 1000; // ASP.NET Identity 2.x default

    public static string RandomPasswordGenerate(
        int size = 8,
        bool upper = true,
        bool lower = true,
        bool number = true,
        bool special = false,
        bool repeat = false)
    {
        // Validação: pelo menos um tipo deve ser selecionado
        if (!upper && !lower && !number && !special)
            throw new ArgumentException("Pelo menos um tipo de caractere deve ser habilitado (upper, lower, number ou special).");

        // Define os caracteres disponíveis
        string upperValues = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string lowerValues = "abcdefghijklmnopqrstuvwxyz";
        string numberValues = "0123456789";
        string specialValues = "!@#$%&*()_-+=[]{}|;:,.<>?";

        string layout = "";
        if (upper) layout += upperValues;
        if (lower) layout += lowerValues;
        if (number) layout += numberValues;
        if (special) layout += specialValues;

        // Se não permitir repetição e tamanho > caracteres disponíveis, limita o tamanho
        if (!repeat && size > layout.Length)
            size = layout.Length;

        HashSet<char> usedChars = repeat ? null : new HashSet<char>();
        StringBuilder sb = new StringBuilder();
        using (var rng = RandomNumberGenerator.Create()) // Criptograficamente seguro
        {
            while (sb.Length < size)
            {
                // Gera índice aleatório
                byte[] randomBytes = new byte[4];
                rng.GetBytes(randomBytes);
                int pos = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % layout.Length;

                char ch = layout[pos];

                // Verifica unicidade se repeat=false
                if (repeat || !usedChars.Contains(ch))
                {
                    sb.Append(ch);
                    if (!repeat) usedChars.Add(ch);
                }
            }
        }

        return sb.ToString();
    }

    public static string HashPassword(string password)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] subkey;
        using (var deriveBytes = new Rfc2898DeriveBytes(
            password,
            salt,
            DefaultIterations,
            HashAlgorithmName.SHA1 // Identity 2.x padrão!
        ))
        {
            subkey = deriveBytes.GetBytes(SubkeySize);
        }

        var outputBytes = new byte[1 + SaltSize + SubkeySize + sizeof(int)];
        outputBytes[0] = 0x00; // format marker

        Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
        Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, SubkeySize);
        Buffer.BlockCopy(BitConverter.GetBytes(DefaultIterations), 0, outputBytes, 1 + SaltSize + SubkeySize, sizeof(int));

        return Convert.ToBase64String(outputBytes);
    }

    public static bool VerifyHashedPassword(string hashedPassword, string password)
    {
        if (hashedPassword == null) throw new ArgumentNullException(nameof(hashedPassword));
        if (password == null) throw new ArgumentNullException(nameof(password));
        byte[] decodedHashedPassword = Convert.FromBase64String(hashedPassword);

        if (decodedHashedPassword.Length != 1 + SaltSize + SubkeySize + sizeof(int) || decodedHashedPassword[0] != 0x00)
            return false;

        byte[] salt = new byte[SaltSize];
        Buffer.BlockCopy(decodedHashedPassword, 1, salt, 0, SaltSize);
        byte[] storedSubkey = new byte[SubkeySize];
        Buffer.BlockCopy(decodedHashedPassword, 1 + SaltSize, storedSubkey, 0, SubkeySize);
        int iterations = BitConverter.ToInt32(decodedHashedPassword, 1 + SaltSize + SubkeySize);

        byte[] generatedSubkey;
        using (var deriveBytes = new Rfc2898DeriveBytes(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA1
        ))
        {
            generatedSubkey = deriveBytes.GetBytes(SubkeySize);
        }

        return ByteArraysEqual(storedSubkey, generatedSubkey);
    }

    private static bool ByteArraysEqual(byte[] a, byte[] b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a == null || b == null || a.Length != b.Length) return false;
        bool areSame = true;
        for (int i = 0; i < a.Length; i++)
            areSame &= (a[i] == b[i]);
        return areSame;
    }
}

