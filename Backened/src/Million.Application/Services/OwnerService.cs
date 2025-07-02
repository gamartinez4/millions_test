using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Million.Application.Contracts;
using Million.Application.DTOs;
using Million.Domain.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Million.Application.Services
{
    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IConfiguration _configuration;

        public OwnerService(IOwnerRepository ownerRepository, IConfiguration configuration)
        {
            _ownerRepository = ownerRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponse> AuthenticateAsync(OwnerLoginRequest loginRequest)
        {
            var owners = await _ownerRepository.FindAsync(o => o.Username == loginRequest.Username);
            var owner = owners.FirstOrDefault();

            if (owner == null || owner.Password != loginRequest.Password) // Plain text password check
            {
                return null;
            }

            var token = GenerateJwtToken(owner);
            var ownerResponse = MapToOwnerResponse(owner);

            return new LoginResponse
            {
                Token = token,
                Owner = ownerResponse
            };
        }

        public async Task<OwnerResponse> CreateOwnerAsync(OwnerRequest ownerRequest)
        {
            var owner = new Owner
            {
                Name = ownerRequest.Name,
                Address = ownerRequest.Address,
                Photo = ownerRequest.Photo,
                Birthday = ownerRequest.Birthday,
                Username = ownerRequest.Username,
                Password = ownerRequest.Password // Storing password in plain text. Hashing is needed for production.
            };

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
                owner.Username = ownerRequest.Username;
                owner.Password = ownerRequest.Password; // Storing password in plain text. Hashing is needed for production.
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
                Birthday = owner.Birthday,
                Username = owner.Username
            };
        }

        private string GenerateJwtToken(Owner owner)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, owner.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, owner.Username),
                new Claim("uid", owner.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(120),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 