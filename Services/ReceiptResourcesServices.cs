using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;

namespace TestTaskAPI.Services
{
    public class ReceiptResourcesServices : IReceiptResourcesServices
    {
        private readonly IReceiptResourceRepository _receiptResourcesRepository;

        public ReceiptResourcesServices(IReceiptResourceRepository receiptResourcesRepository)
        {
             _receiptResourcesRepository = receiptResourcesRepository;
        }

        public async Task<IEnumerable<ReceiptResource>> GetReceiptResourcesTransfer()
        {
            var receiptResources = await _receiptResourcesRepository.GetAllAsync();

            if (receiptResources == null)
            {
                throw new KeyNotFoundException("Данные по ресурсам не найдены");
            }

            return receiptResources.OrderBy(x => x.Id).ToList();
        }
        public async Task<ReceiptResource> GetReceiptResourcesByIdTransferAsync(int id)
        {
            var updReceiptResources = await _receiptResourcesRepository.GetByIdAsync(id);
            if (updReceiptResources == null)
            {
                throw new KeyNotFoundException("Данные по ресурсам не найдены");
            }

            return updReceiptResources;
        }
        public async Task<ReceiptResource> CreateReceiptResources(ReceiptResource receiptResources)
        {
            await _receiptResourcesRepository.AddAsync(receiptResources);
            return receiptResources;
        }

        public async Task UpdateReceiptResources(ReceiptResource receiptResources)
        {
            var updReceiptResources = await _receiptResourcesRepository.GetByIdAsync(receiptResources.Id);
            if (updReceiptResources == null)
            {
                throw new KeyNotFoundException($"Ресурс {receiptResources.Resources.Productname} не найден");
            }
            updReceiptResources.Measureid = receiptResources.Measureid;
            updReceiptResources.Documentid = receiptResources.Documentid;
            updReceiptResources.Countresources = receiptResources.Countresources;
            updReceiptResources.Resourcesid = receiptResources.Resourcesid;
            await _receiptResourcesRepository.UpdateAsync(updReceiptResources);
        }

        public async Task DeleteReceiptResources(int id)
        {
            var delReceiptResources = await _receiptResourcesRepository.GetByIdAsync(id);
            if (delReceiptResources == null)
            {
                throw new KeyNotFoundException($"Ресурс {delReceiptResources.Resources.Productname} не найден");
            }
            else
            {
                await _receiptResourcesRepository.DeleteAsync(delReceiptResources);
            }
        }
    }
}
