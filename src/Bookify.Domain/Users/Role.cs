namespace Bookify.Domain.Users;

public sealed class Role
{
    public static readonly Role Registered = new(id: 1, name: "Registered");

    private Role(int id, string name)
    {
        Id = id;
        Name = name;
    }

    private Role() { }

    public int Id { get; init; }

    public string Name { get; init; }

    public ICollection<User> Users { get; init; } = new List<User>();
}
