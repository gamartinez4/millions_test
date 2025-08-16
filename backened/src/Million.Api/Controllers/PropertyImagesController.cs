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
    public class PropertyImagesController : ControllerBase
    {
        private readonly IPropertyImageService _imageService;

        public PropertyImagesController(IPropertyImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("all")]
        public async Task<ActionResult<IEnumerable<PropertyImageResponse>>> GetAllImages()
        {
            var images = await _imageService.GetAllImagesAsync();
            return Ok(images);
        }

        [HttpGet("property/{propertyId}")]
        public async Task<ActionResult<IEnumerable<PropertyImageResponse>>> GetImagesForProperty(int propertyId)
        {
            var images = await _imageService.GetImagesForPropertyAsync(propertyId);
            return Ok(images);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyImageResponse>> Get(int id)
        {
            var image = await _imageService.GetImageByIdAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return Ok(image);
        }

        [HttpPost]
        public async Task<ActionResult<PropertyImageResponse>> Post([FromBody] PropertyImageRequest imageRequest)
        {
            var createdImage = await _imageService.AddImageToPropertyAsync(imageRequest);
            return CreatedAtAction(nameof(Get), new { id = createdImage.Id }, createdImage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] PropertyImageRequest imageRequest)
        {
            await _imageService.UpdateImageAsync(id, imageRequest);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _imageService.DeleteImageAsync(id);
            return NoContent();
        }
    }
} 