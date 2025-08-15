using Microsoft.AspNetCore.Mvc;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;

namespace TestTaskAPI.Controllers
{
    [ApiController]
    [Route ("api/balance")]
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceServices _balanceServices;

        public BalanceController(IBalanceServices balanceServices)
        {
            _balanceServices = balanceServices;
        }

        [HttpGet]
        public async Task<ActionResult<Balance>> GetBalance()
        {
            try
            {
                var balance = await _balanceServices.GetBalanceTransfer();
                return Ok(balance);
            }
            catch(KeyNotFoundException ex)
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
