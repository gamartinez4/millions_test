using Million.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Million.Application.Contracts
{
    public interface IOwnerService
    {
        Task<IEnumerable<OwnerResponse>> GetAllOwnersAsync();
        Task<OwnerResponse> GetOwnerByIdAsync(int id);
        Task<OwnerResponse> CreateOwnerAsync(OwnerRequest ownerRequest);
        Task UpdateOwnerAsync(int id, OwnerRequest ownerRequest);
        Task DeleteOwnerAsync(int id);
        Task<LoginResponse> AuthenticateAsync(OwnerLoginRequest loginRequest);
    }
} 