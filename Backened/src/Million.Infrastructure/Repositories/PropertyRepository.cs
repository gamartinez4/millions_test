using Million.Application.Contracts;
using Million.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Million.Infrastructure.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly List<Property> _properties = new List<Property>();
        private int _nextId = 1;

        public PropertyRepository()
        {
            // Seed with some fake data
            var owner = new Owner("John Doe", "123 Main St", "photo.jpg", new DateTime(1980, 1, 1));
            _properties.Add(new Property("Casa de Playa", "Calle 123", 150000, 2010, 1) { Id = _nextId++, Owner = owner });
            _properties.Add(new Property("Apartamento en la Ciudad", "Avenida 45", 250000, 2015, 1) { Id = _nextId++, Owner = owner });
            _properties.Add(new Property("Casa de Campo", "Vereda 7", 350000, 2020, 1) { Id = _nextId++, Owner = owner });
        }

        public Task<Property> GetByIdAsync(int id)
        {
            return Task.FromResult(_properties.FirstOrDefault(p => p.Id == id));
        }

        public Task<IEnumerable<Property>> GetAllAsync()
        {
            return Task.FromResult(_properties.AsEnumerable());
        }

        public Task<IEnumerable<Property>> FindAsync(Expression<Func<Property, bool>> expression)
        {
            return Task.FromResult(_properties.AsQueryable().Where(expression).AsEnumerable());
        }

        public Task AddAsync(Property entity)
        {
            entity.Id = _nextId++;
            _properties.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(Property entity)
        {
            var existingProperty = _properties.FirstOrDefault(p => p.Id == entity.Id);
            if (existingProperty != null)
            {
                existingProperty.Name = entity.Name;
                existingProperty.Address = entity.Address;
                existingProperty.Price = entity.Price;
                existingProperty.Year = entity.Year;
                existingProperty.OwnerId = entity.OwnerId;
            }
        }

        public void Remove(Property entity)
        {
            _properties.Remove(entity);
        }
    }
} 