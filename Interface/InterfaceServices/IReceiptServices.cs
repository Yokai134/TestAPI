using TestTaskAPI.Model;

namespace TestTaskAPI.Interface.InterfaceServices
{
    public interface IReceiptServices
    {
        Task<IEnumerable<ReceiptDocumet>> GetReceiptDocumetTransfer(); //Все документы по постуалениям
        Task<ReceiptDocumet> GetReceiptDocumetByIdTransferAsync(int id); //Документы по постуалениям, по ID
        Task<ReceiptDocumet> CreateReceiptDocumet(ReceiptDocumet ReceiptDocumet);
        Task UpdateReceiptDocumet(ReceiptDocumet ReceiptDocumet);
        Task DeleteReceiptDocumet(int id);
    }
}
