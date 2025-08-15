using Microsoft.EntityFrameworkCore;
using TestTaskAPI.Data;
using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Model;

namespace TestTaskAPI.Repository
{
    public class BalanceRepository : BaseRepository<Balance>, IBalanceRepository
    {
        public BalanceRepository(TesttaskdbContext context) : base(context) { }
        public async Task<IEnumerable<Balance>> GetAllWithDetailsAsync()
        {
            return await _dbContext.Balances
                .Include(b => b.Resources)  // Явно подгружаем Resources
                .Include(b => b.Measure)    // Явно подгружаем Measure
                .ToListAsync();
        }
    }
}
