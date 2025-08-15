using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Net;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;
using TestTaskAPI.Services;

namespace TestTaskAPI.Controllers
{
    [ApiController]
    [Route ("api/resource")]
    public class ResourceController : ControllerBase
    {
        private readonly IResourceServices _resourceServices;

        public ResourceController(IResourceServices resourceServices)
        {
            _resourceServices = resourceServices;
        }

        [HttpGet]
        public async Task<ActionResult<Resource>> GetResource()
        {
            try
            {
                var resources = await _resourceServices.GetResourceTransfer();
                return Ok(resources);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при запросе");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Resource>> GetResourceById(int id)
        {
            try
            {
                var resources = await _resourceServices.GetResourceByIdTransferAsync(id);
                return Ok(resources);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при запросе");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Resource>> CreateClient([FromBody] Resource newResource)
        {
            if (newResource == null)
            {
                return BadRequest("Данные о ресурсе не введены.");

            }
            try
            {

                var resorceCreate = await _resourceServices.CreateResource(newResource);
                return CreatedAtAction(nameof(GetResource), new { id = resorceCreate.Id }, resorceCreate);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при запросе");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResource(int id, [FromBody] Resource updResource)
        {
            try
            {
                if (id != updResource.Id)
                    return BadRequest("ID не найден");

                await _resourceServices.UpdateResource(updResource);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при запросе");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResource(int id)
        {
            try
            {
                await _resourceServices.DeleteResource(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при запросе");
            }
        }
    }
}
