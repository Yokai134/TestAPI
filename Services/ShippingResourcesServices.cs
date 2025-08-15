using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;

namespace TestTaskAPI.Services
{
    public class ShippingResourcesServices : IShippingResourcesServices
    {
        private readonly IShippingResourceRepository _shippingResourceRepository;

        public ShippingResourcesServices(IShippingResourceRepository shippingResourceRepository)
        {
            _shippingResourceRepository = shippingResourceRepository;
        }

        public async Task<IEnumerable<ShippingResource>> GetShippingResourceTransfer()
        {
            var ShippingResources = await _shippingResourceRepository.GetAllAsync();

            if (ShippingResources == null)
            {
                throw new KeyNotFoundException("Данные по отгруженным ресурсам не найдены");
            }

            return ShippingResources.OrderBy(x => x.Id).ToList();
        }
        public async Task<ShippingResource> GetShippingResourceByIdTransferAsync(int id)
        {
            var updShippingResources = await _shippingResourceRepository.GetByIdAsync(id);
            if (updShippingResources == null)
            {
                throw new KeyNotFoundException("Данные по отгруженным ресурсам не найдены");
            }

            return updShippingResources;
        }
        public async Task<ShippingResource> CreateShippingResource(ShippingResource shippingResources)
        {
            await _shippingResourceRepository.AddAsync(shippingResources);
            return shippingResources;
        }

        public async Task UpdateShippingResource(ShippingResource shippingResources)
        {
            var updShippingResources = await _shippingResourceRepository.GetByIdAsync(shippingResources.Id);
            if (updShippingResources == null)
            {
                throw new KeyNotFoundException($"Отгруженный ресурс {shippingResources.Resources.Productname} не найден");
            }
            updShippingResources.MeasureId = shippingResources.MeasureId;
            updShippingResources.DocumentId = shippingResources.DocumentId;
            updShippingResources.Count = shippingResources.Count;
            updShippingResources.ResourcesId = shippingResources.ResourcesId;
            await _shippingResourceRepository.UpdateAsync(updShippingResources);
        }

        public async Task DeleteShippingResource(int id)
        {
            var delShippingResources = await _shippingResourceRepository.GetByIdAsync(id);
            if (delShippingResources == null)
            {
                throw new KeyNotFoundException($"Отгруженный ресурс {delShippingResources.Resources.Productname} не найден");
            }
            else
            {
                await _shippingResourceRepository.DeleteAsync(delShippingResources);
            }
        }
    }
}
