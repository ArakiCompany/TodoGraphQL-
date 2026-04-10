using MongoDB.Driver;
using TodoGraphQL.Domain.Entities;
using TodoGraphQL.Domain.Interfaces;

namespace TodoGraphQL.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly MongoDbContext _context;

    public UserRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<User?> FindByEmailAsync(string email)
        => await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();

    public async Task<User> CreateAsync(User user)
    {
        await _context.Users.InsertOneAsync(user);
        return user;
    }
    public async Task UpdateRoleAsync(string id, UserRole role)
    {
        var update = Builders<User>.Update.Set("Role", role);
        await _context.Users.UpdateOneAsync(u => u.Id == id, update);
    }

     public async Task<List<User>> GetAllAsync()
        => await _context.Users.Find(_ => true).ToListAsync();
}