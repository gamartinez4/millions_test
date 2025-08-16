using MongoDB.Driver;
using Million.Application.Contracts;
using Million.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Million.Infrastructure.Repositories
{
    public abstract class MongoRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> _collection;

        protected MongoRepository(IMongoDbContext context, string collectionName)
        {
            _collection = context.GetCollection<T>(collectionName);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            return await _collection.Find(expression).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty != null && idProperty.PropertyType == typeof(int))
            {
                var currentId = (int)idProperty.GetValue(entity);
                if (currentId == 0)
                {
                    // generar siguiente Id basado en el máximo existente
                    var sort = Builders<T>.Sort.Descending("_id");
                    var last = await _collection.Find(_ => true).Sort(sort).Limit(1).FirstOrDefaultAsync();
                    int nextId = 1;
                    if (last != null)
                    {
                        var lastIdValue = (int)idProperty.GetValue(last);
                        nextId = lastIdValue + 1;
                    }
                    idProperty.SetValue(entity, nextId);
                }
            }

            await _collection.InsertOneAsync(entity);
        }

        public void Update(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException("Entity must have an Id property");
            }
            var idValue = idProperty.GetValue(entity);
            Console.WriteLine($"[MongoRepository] Updating entity with ID: {idValue}");
            
            var filter = Builders<T>.Filter.Eq("_id", idValue);
            var result = _collection.ReplaceOne(filter, entity);
            
            Console.WriteLine($"[MongoRepository] Update result - Matched: {result.MatchedCount}, Modified: {result.ModifiedCount}");
        }

        public async Task UpdateAsync(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException("Entity must have an Id property");
            }
            var idValue = idProperty.GetValue(entity);
            var filter = Builders<T>.Filter.Eq("_id", idValue);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public void Remove(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException("Entity must have an Id property");
            }
            var idValue = idProperty.GetValue(entity);
            var filter = Builders<T>.Filter.Eq("_id", idValue);
            _collection.DeleteOne(filter);
        }
    }
} 