using Million.Application.Contracts;
using Million.Application.DTOs;
using Million.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Million.Application.Services
{
    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _ownerRepository;

        public OwnerService(IOwnerRepository ownerRepository)
        {
            _ownerRepository = ownerRepository;
        }

        public async Task<OwnerResponse> CreateOwnerAsync(OwnerRequest ownerRequest)
        {
            var owner = new Owner(
                ownerRequest.Name,
                ownerRequest.Address,
                ownerRequest.Photo,
                ownerRequest.Birthday
            );

            await _ownerRepository.AddAsync(owner);

            return MapToOwnerResponse(owner);
        }

        public async Task DeleteOwnerAsync(int id)
        {
            var owner = await _ownerRepository.GetByIdAsync(id);
            if (owner != null)
            {
                _ownerRepository.Remove(owner);
            }
        }

        public async Task<IEnumerable<OwnerResponse>> GetAllOwnersAsync()
        {
            var owners = await _ownerRepository.GetAllAsync();
            return owners.Select(MapToOwnerResponse);
        }

        public async Task<OwnerResponse> GetOwnerByIdAsync(int id)
        {
            var owner = await _ownerRepository.GetByIdAsync(id);
            return owner == null ? null : MapToOwnerResponse(owner);
        }

        public async Task UpdateOwnerAsync(int id, OwnerRequest ownerRequest)
        {
            var owner = await _ownerRepository.GetByIdAsync(id);
            if (owner != null)
            {
                owner.Name = ownerRequest.Name;
                owner.Address = ownerRequest.Address;
                owner.Photo = ownerRequest.Photo;
                owner.Birthday = ownerRequest.Birthday;
                _ownerRepository.Update(owner);
            }
        }

        private OwnerResponse MapToOwnerResponse(Owner owner)
        {
            return new OwnerResponse
            {
                Id = owner.Id,
                Name = owner.Name,
                Address = owner.Address,
                Photo = owner.Photo,
                Birthday = owner.Birthday
            };
        }
    }
} 