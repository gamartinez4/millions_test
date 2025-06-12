using Million.Application.Contracts;
using Million.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Million.Infrastructure.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly List<Owner> _owners = new List<Owner>();
        private int _nextId = 1;

        public OwnerRepository()
        {
            _owners.Add(new Owner("John Doe", "123 Main St", "photo.jpg", new DateTime(1980, 1, 1)) { Id = _nextId++ });
            _owners.Add(new Owner("Jane Smith", "456 Oak Ave", "photo2.jpg", new DateTime(1990, 5, 15)) { Id = _nextId++ });
        }

        public Task<Owner> GetByIdAsync(int id)
        {
            return Task.FromResult(_owners.FirstOrDefault(o => o.Id == id));
        }

        public Task<IEnumerable<Owner>> GetAllAsync()
        {
            return Task.FromResult(_owners.AsEnumerable());
        }

        public Task<IEnumerable<Owner>> FindAsync(Expression<Func<Owner, bool>> expression)
        {
            return Task.FromResult(_owners.AsQueryable().Where(expression).AsEnumerable());
        }

        public Task AddAsync(Owner entity)
        {
            entity.Id = _nextId++;
            _owners.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(Owner entity)
        {
            var existingOwner = _owners.FirstOrDefault(o => o.Id == entity.Id);
            if (existingOwner != null)
            {
                existingOwner.Name = entity.Name;
                existingOwner.Address = entity.Address;
                existingOwner.Photo = entity.Photo;
                existingOwner.Birthday = entity.Birthday;
            }
        }

        public void Remove(Owner entity)
        {
            _owners.Remove(entity);
        }
    }
} 