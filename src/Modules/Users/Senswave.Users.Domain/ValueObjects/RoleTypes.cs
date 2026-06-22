namespace Senswave.Users.Domain.ValueObjects;

public record RoleTypes
{
    public string NormalizedName { get; private set; }
    public string Name { get; private set; }
    public string Policy { get; private set; }

    private RoleTypes(string name, string normalizedName, string policy)
    {
        NormalizedName=normalizedName;
        Name=name;
        Policy=policy;
    }

    public static RoleTypes Admin => new("admin", "System Administrator".ToUpperInvariant(), "AdminPolicy");
    public static RoleTypes User => new("user", "Application User".ToUpperInvariant(), "UserPolicy");

}
