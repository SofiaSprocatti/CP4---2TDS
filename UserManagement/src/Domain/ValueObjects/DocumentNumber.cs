namespace UserManagement.Domain.ValueObjects;

public class DocumentNumber
{
    public string Value { get; private set; }
    public string Type { get; private set; }

    private DocumentNumber(string value, string type)
    {
        Value = value;
        Type = type;
    }

    public static DocumentNumber CreateCPF(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF cannot be empty");

        var cleanCpf = cpf.Replace(".", "").Replace("-", "").Replace(" ", "");

        if (!IsValidCPF(cleanCpf))
            throw new ArgumentException("Invalid CPF format");

        return new DocumentNumber(FormatCPF(cleanCpf), "CPF");
    }

    public static DocumentNumber CreateCNPJ(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            throw new ArgumentException("CNPJ cannot be empty");

        var cleanCnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Replace(" ", "");

        if (!IsValidCNPJ(cleanCnpj))
            throw new ArgumentException("Invalid CNPJ format");

        return new DocumentNumber(FormatCNPJ(cleanCnpj), "CNPJ");
    }

    private static bool IsValidCPF(string cpf)
    {
        if (cpf.Length != 11 || !cpf.All(char.IsDigit))
            return false;

        if (cpf.All(c => c == cpf[0]))
            return false;

        return true; 
    }

    private static bool IsValidCNPJ(string cnpj)
    {
        if (cnpj.Length != 14 || !cnpj.All(char.IsDigit))
            return false;

        return true; 
    }

    private static string FormatCPF(string cpf)
    {
        return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
    }

    private static string FormatCNPJ(string cnpj)
    {
        return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
    }

    public override string ToString() => $"{Type}: {Value}";

    public override bool Equals(object? obj)
    {
        return obj is DocumentNumber other && Value == other.Value && Type == other.Type;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value, Type);
    }
}