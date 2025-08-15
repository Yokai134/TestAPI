using Microsoft.AspNetCore.Mvc;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;
using TestTaskAPI.Services;

namespace TestTaskAPI.Controllers
{
    [ApiController]
    [Route ("api/status")]
    public class StatusController : ControllerBase
    {
        private readonly IStatusServices _statusServices;
        public StatusController(IStatusServices statusServices)
        {
            _statusServices = statusServices;
        }
        [HttpGet]
        public async Task<ActionResult<Status>> GetStatus()
        {
            try
            {
                var status = await _statusServices.GetStatusTransfer();
                return Ok(status);
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
