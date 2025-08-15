using Microsoft.EntityFrameworkCore;
using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;

namespace TestTaskAPI.Services
{
    public class ShippingServices : IShippingServices
    {
        private readonly IShippingRepository _shippingRepository;

        public ShippingServices(IShippingRepository shippingRepository)
        {
            _shippingRepository = shippingRepository;
        }

        public async Task<IEnumerable<ShippingDocument>> GetShippingDocumentTransfer()
        {
            var shipping = await _shippingRepository.GetAllWithDetailsAsync();

            if (shipping == null)
            {
                throw new KeyNotFoundException("Данные по документу не найдены");
            }

            return shipping.OrderBy(x => x.Id).ToList();
        }
        public async Task<ShippingDocument> GetShippingDocumentByIdTransferAsync(int id)
        {
            var updShipping = await _shippingRepository.GetAllWithDetailsAsyncId(id);
            if (updShipping == null)
            {
                throw new KeyNotFoundException("Данные по документу не найдены");
            }

            return updShipping;
        }
        public async Task<ShippingDocument> CreateShippingDocument(ShippingDocument shipping)
        {
            int clientId = shipping.ClientId;  
            shipping.Client = null;      
            shipping.ClientId = clientId;
            int? statusId = shipping.StatusId;
            shipping.Status = null;
            shipping.StatusId = statusId;

            await _shippingRepository.AddAsync(shipping);
            return shipping;
        }

        public async Task UpdateShippingDocument(ShippingDocument shipping)
        {
            var updShipping = await _shippingRepository.GetByIdAsync(shipping.Id);
            if (updShipping == null)
            {
                throw new KeyNotFoundException($"Докумет с номером {updShipping.DocumentNumber} не найден");
            }
            updShipping.ClientId = shipping.ClientId;
            updShipping.StatusId = shipping.StatusId;
            await _shippingRepository.UpdateAsync(updShipping);
        }

        public async Task DeleteShippingDocument(int id)
        {
            var delShipping = await _shippingRepository.GetByIdAsync(id);
            if (delShipping == null)
            {
                throw new KeyNotFoundException($"Докумет с номером {delShipping.DocumentNumber} не найден");
            }
            else
            {
                await _shippingRepository.DeleteAsync(delShipping);
            }
        }
    }
}
