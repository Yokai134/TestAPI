using TestTaskAPI.Model;

namespace TestTaskAPI.Interface.InterfaceRepository
{
    public interface IBalanceRepository : IRepository<Balance>
    {
        Task<IEnumerable<Balance>> GetAllWithDetailsAsync();
    }
}
