using Microsoft.EntityFrameworkCore;
using TestTaskAPI.Data;
using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Model;

namespace TestTaskAPI.Repository
{
    public class ShippingRepository : BaseRepository<ShippingDocument>, IShippingRepository
    {
        public ShippingRepository(TesttaskdbContext context) : base(context) { }

        public async Task<IEnumerable<ShippingDocument>> GetAllWithDetailsAsync()
        {
            return await _dbContext.Shippingdocuments
                .Include(b => b.Client)
                .Include(b => b.Status)
                .ToListAsync();
        }

        public async Task<ShippingDocument> GetAllWithDetailsAsyncId(int id)
        {
            return await _dbContext.Shippingdocuments
              .Include(b => b.Client)
              .Include(b => b.Status)
              .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
