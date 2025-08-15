using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;

namespace TestTaskAPI.Services
{
    public class BalanceServices : IBalanceServices
    {
        private readonly IBalanceRepository _balanceRepository;

        public BalanceServices(IBalanceRepository balanceRepository)
        {
            _balanceRepository = balanceRepository;
        }

        public async Task<IEnumerable<Balance>> GetBalanceTransfer()
        {
            var balance = await _balanceRepository.GetAllWithDetailsAsync();

            if (balance == null)
            {
                throw new KeyNotFoundException("Данные по клиентам не найдены");
            }

            return balance.OrderBy(x => x.Id).ToList();
        }

        public async Task DeleteBalance(int id)
        {
            var delBalance = await _balanceRepository.GetByIdAsync(id);
            if (delBalance == null)
            {
                throw new KeyNotFoundException($"Баланс {delBalance.Resources.Productname} не найден");
            }
            else
            {
                await _balanceRepository.DeleteAsync(delBalance);
            }
        }
    }
}
