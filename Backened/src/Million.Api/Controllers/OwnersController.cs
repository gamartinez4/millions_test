using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Million.Application.Contracts;
using Million.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Million.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OwnersController : ControllerBase
    {
        private readonly IOwnerService _ownerService;

        public OwnersController(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] OwnerLoginRequest loginRequest)
        {
            var loginResponse = await _ownerService.AuthenticateAsync(loginRequest);

            if (loginResponse == null)
            {
                return Unauthorized();
            }

            return Ok(loginResponse);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OwnerResponse>>> Get()
        {
            var owners = await _ownerService.GetAllOwnersAsync();
            return Ok(owners);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OwnerResponse>> Get(int id)
        {
            var owner = await _ownerService.GetOwnerByIdAsync(id);
            if (owner == null)
            {
                return NotFound();
            }
            return Ok(owner);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<OwnerResponse>> Post([FromBody] OwnerRequest ownerRequest)
        {
            var createdOwner = await _ownerService.CreateOwnerAsync(ownerRequest);
            return CreatedAtAction(nameof(Get), new { id = createdOwner.Id }, createdOwner);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] OwnerRequest ownerRequest)
        {
            await _ownerService.UpdateOwnerAsync(id, ownerRequest);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _ownerService.DeleteOwnerAsync(id);
            return NoContent();
        }
    }
} 