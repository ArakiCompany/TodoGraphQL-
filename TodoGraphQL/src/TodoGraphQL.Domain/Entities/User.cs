using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public enum UserRole
{
    User,
    Business,
    Admin
}

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    
    [BsonRepresentation(BsonType.String)]
    public UserRole Role { get; private set; } = UserRole.User;


    private User() { }

    public static User Create(string email, string passwordHash,  UserRole role = UserRole.User)
    {
        return new User
        {
            Email = email.Trim().ToLower(),
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            Role = role,
        };
    }
}