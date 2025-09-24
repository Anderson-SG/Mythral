namespace Mythral.Server.Domain.Enums;


[Flags]
public enum AccountStatus
{
    Inactive = 0,
    Active = 1 << 0,
    Suspended = 1 << 1,
    Banned = 1 << 2
}