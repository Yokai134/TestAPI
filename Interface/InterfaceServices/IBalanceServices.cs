using TestTaskAPI.Model;

namespace TestTaskAPI.Interface.InterfaceServices
{
    public interface IBalanceServices
    {
        Task<IEnumerable<Balance>> GetBalanceTransfer(); //Всь баланс
        Task DeleteBalance(int id);
    }
}
