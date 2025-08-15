using TestTaskAPI.Model;

namespace TestTaskAPI.Interface.InterfaceServices
{
    public interface IShippingResourcesServices
    {
        Task<IEnumerable<ShippingResource>> GetShippingResourceTransfer(); //Все клиенты
        Task<ShippingResource> GetShippingResourceByIdTransferAsync(int id); //Клиент по ID
        Task<ShippingResource> CreateShippingResource(ShippingResource Shippingresource);
        Task UpdateShippingResource(ShippingResource Shippingresource);
        Task DeleteShippingResource(int id);
    }
}
