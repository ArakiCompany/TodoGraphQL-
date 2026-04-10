using TodoGraphQL.Domain.Entities;

namespace TodoGraphQL.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task UpdateRoleAsync(string id, UserRole role);
    Task<List<User>> GetAllAsync(); 
}