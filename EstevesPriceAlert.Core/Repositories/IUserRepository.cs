using EstevesPriceAlert.Core.Entities;

namespace EstevesPriceAlert.Core.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<User> GetByIdAsync(Guid id);
        Task<List<User>> GetAllAsync();
    }
}
