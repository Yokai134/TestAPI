using TestTaskAPI.Model;

namespace TestTaskAPI.Interface.InterfaceRepository
{
    public interface IShippingRepository : IRepository<ShippingDocument>
    {
        Task<IEnumerable<ShippingDocument>> GetAllWithDetailsAsync();

        Task<ShippingDocument> GetAllWithDetailsAsyncId(int id);
    }
}
