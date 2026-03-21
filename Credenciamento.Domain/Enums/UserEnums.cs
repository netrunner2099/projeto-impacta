namespace Credenciamento.Domain.Enums;

public enum UserStatus : byte
{
    Inactive = 0,
    Active = 1,
    Deleted = 9
}

public enum UserRole : byte
{
    Admin = 1,
    User = 2
}


