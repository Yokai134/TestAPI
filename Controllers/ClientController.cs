using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Sockets;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;

namespace TestTaskAPI.Controllers
{
    [ApiController]
    [Route("api/client")]
    public class ClientController : ControllerBase
    {
        private readonly IClientServices _clientServices;
        public ClientController(IClientServices clientServices)
        {
            _clientServices = clientServices;
        }

        [HttpGet]
        public async Task<ActionResult<Client>> GetClient()
        {
            try
            {
                var client = await _clientServices.GetClientTransfer();
                return Ok(client);
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
        public async Task<ActionResult<Client>> GetClientByID(int id)
        {
            try
            {
                var client = await _clientServices.GetClientByIdTransferAsync(id);
                return Ok(client);
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
        public async Task<ActionResult<Client>> CreateClient([FromBody] Client newClient)
        {
            if (newClient == null)
            {
                return BadRequest("Данные клиента не введены.");

            }
            try
            {
                
                var clientCreate = await _clientServices.CreateClient(newClient);
                return CreatedAtAction(nameof(GetClient), new { id = clientCreate.Id }, clientCreate);
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
        public async Task<IActionResult> UpdateClient(int id, [FromBody] Client updClient)
        {
            try
            {
                if (id != updClient.Id)
                    return BadRequest("ID не найден");

                await _clientServices.UpdateClient(updClient);
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
        public async Task<IActionResult> DeleteClient(int id)
        {
            try
            {            
                await _clientServices.DeleteClient(id);
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
