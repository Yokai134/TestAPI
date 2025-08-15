using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;

namespace TestTaskAPI.Services
{
    public class ReceiptServices : IReceiptServices
    {
        private readonly IReceiptRepository _receiptDocumentRepository;

        public ReceiptServices(IReceiptRepository receiptDocumentRepository)
        {
            _receiptDocumentRepository = receiptDocumentRepository;
        }

        public async Task<IEnumerable<ReceiptDocumet>> GetReceiptDocumetTransfer()
        {
            var receiptDocument = await _receiptDocumentRepository.GetAllAsync();

            if (receiptDocument == null)
            {
                throw new KeyNotFoundException("Данные по докуметам не найдены");
            }

            return receiptDocument.OrderBy(x => x.Id).ToList();
        }
        public async Task<ReceiptDocumet> GetReceiptDocumetByIdTransferAsync(int id)
        {
            var updReceiptDocument = await _receiptDocumentRepository.GetByIdAsync(id);
            if (updReceiptDocument == null)
            {
                throw new KeyNotFoundException("Данные по докуметам не найдены");
            }

            return updReceiptDocument;
        }
        public async Task<ReceiptDocumet> CreateReceiptDocumet(ReceiptDocumet receiptDocument)
        {
            await _receiptDocumentRepository.AddAsync(receiptDocument);
            return receiptDocument;
        }

        public async Task UpdateReceiptDocumet(ReceiptDocumet receiptDocument)
        {
            var updReceiptDocument = await _receiptDocumentRepository.GetByIdAsync(receiptDocument.Id);
            if (updReceiptDocument == null)
            {
                throw new KeyNotFoundException($"Докумет с номером {updReceiptDocument.Numberdocument} не найден");
            }
            updReceiptDocument.Date = receiptDocument.Date;
            await _receiptDocumentRepository.UpdateAsync(updReceiptDocument);
        }

        public async Task DeleteReceiptDocumet(int id)
        {
            var delReceiptDocument = await _receiptDocumentRepository.GetByIdAsync(id);
            if (delReceiptDocument == null)
            {
                throw new KeyNotFoundException($"Докумет с номером {delReceiptDocument.Numberdocument} не найден");
            }
            else
            {
                await _receiptDocumentRepository.DeleteAsync(delReceiptDocument);
            }
        }
    }
}
