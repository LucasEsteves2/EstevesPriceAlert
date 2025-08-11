using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstevesPriceAlert.Core.Entities;
using EstevesPriceAlert.Core.Repositories;
using MongoDB.Driver;

namespace EstevesPriceAlert.InfraEstructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly IMongoCollection<User> _collection;

        public UserRepository(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<User>("users");
        }

        public async Task AddAsync(User user)
        {
            await _collection.InsertOneAsync(user);
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _collection.Find(c => c.Id == id).SingleOrDefaultAsync();
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task UpdateAsync(User user)
        {
            await _collection.ReplaceOneAsync(c => c.Id == user.Id, user);
        }
    }
}
