using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Million.Application.Contracts;
using Million.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Million.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyResponse>>> GetProperties()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return Ok(properties);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyResponse>> GetProperty(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return Ok(property);
        }

        [HttpPost]
        public async Task<ActionResult<PropertyResponse>> CreateProperty(PropertyRequest propertyRequest)
        {
            var newProperty = await _propertyService.CreatePropertyAsync(propertyRequest);
            return CreatedAtAction(nameof(GetProperty), new { id = newProperty.Id }, newProperty);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(int id, PropertyRequest propertyRequest)
        {
            await _propertyService.UpdatePropertyAsync(id, propertyRequest);
            return NoContent();
        }

        [HttpPatch("{id}/for-sale")]
        public async Task<IActionResult> UpdatePropertyForSale(int id, UpdatePropertyForSaleRequest request)
        {
            try
            {
                await _propertyService.UpdatePropertyForSaleAsync(id, request);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while updating the property." });
            }
        }

        [HttpPatch("{id}/owner")]
        public async Task<IActionResult> UpdatePropertyOwner(int id, UpdatePropertyOwnerRequest request)
        {
            try
            {
                Console.WriteLine($"[PropertiesController] UpdatePropertyOwner called - ID: {id}, OwnerId: {request.OwnerId}");
                await _propertyService.UpdatePropertyOwnerAsync(id, request);
                Console.WriteLine($"[PropertiesController] UpdatePropertyOwner completed successfully");
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[PropertiesController] ArgumentException: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PropertiesController] Exception: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating the property owner." });
            }
        }

        [HttpPatch("{id}/price")]
        public async Task<IActionResult> UpdatePropertyPrice(int id, UpdatePropertyPriceRequest request)
        {
            try
            {
                Console.WriteLine($"[PropertiesController] UpdatePropertyPrice called - ID: {id}, Price: {request.Price}");
                await _propertyService.UpdatePropertyPriceAsync(id, request);
                Console.WriteLine($"[PropertiesController] UpdatePropertyPrice completed successfully");
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[PropertiesController] ArgumentException: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PropertiesController] Exception: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating the property price." });
            }
        }


    }

} 