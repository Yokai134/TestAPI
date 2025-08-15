using TestTaskAPI.Model;

namespace TestTaskAPI.Interface.InterfaceServices
{
    public interface IReceiptResourcesServices
    {
        Task<IEnumerable<ReceiptResource>> GetReceiptResourcesTransfer(); //Все ресурсы документа
        Task<ReceiptResource> GetReceiptResourcesByIdTransferAsync(int id); //Ресурс документа по ID
        Task<ReceiptResource> CreateReceiptResources(ReceiptResource ReceiptResources);
        Task UpdateReceiptResources(ReceiptResource ReceiptResources);
        Task DeleteReceiptResources(int id);
    }
}
