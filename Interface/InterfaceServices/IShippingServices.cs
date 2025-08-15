using TestTaskAPI.Model;

namespace TestTaskAPI.Interface.InterfaceServices
{
    public interface IShippingServices
    {
        Task<IEnumerable<ShippingDocument>> GetShippingDocumentTransfer(); //Все докуметы отгрузки
        Task<ShippingDocument> GetShippingDocumentByIdTransferAsync(int id); //Докуметы отгрузки по ID
        Task<ShippingDocument> CreateShippingDocument(ShippingDocument ShippingDocument);
        Task UpdateShippingDocument(ShippingDocument ShippingDocument);
        Task DeleteShippingDocument(int id);
    }
}
