using Microsoft.EntityFrameworkCore;
using System.Resources;
using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;

namespace TestTaskAPI.Services
{
    public class ResourceServices : IResourceServices
    {
        private readonly IResourceRepository _resourceRepository;

        public ResourceServices(IResourceRepository resourceRepository)
        {
            _resourceRepository = resourceRepository;
        }

        public async Task<IEnumerable<Resource>> GetResourceTransfer()
        {
            var resource = await _resourceRepository.GetAllAsync();

            if (resource == null)
            {
                throw new KeyNotFoundException("Данные по ресурсу не найдены");
            }

            return resource.OrderBy(x => x.Id).ToList();
        }
        public async Task<Resource> GetResourceByIdTransferAsync(int id)
        {
            var updResource = await _resourceRepository.GetByIdAsync(id);
            if (updResource == null)
            {
                throw new KeyNotFoundException("Данные по ресурсу не найдены");
            }

            return updResource;
        }
        public async Task<Resource> CreateResource(Resource resource)
        {
            await _resourceRepository.AddAsync(resource);
            return resource;
        }

        public async Task UpdateResource(Resource resource)
        {
            var updResource = await _resourceRepository.GetByIdAsync(resource.Id);
            if (updResource == null)
            {
                throw new KeyNotFoundException($"Ресурс {resource.Productname} не найден");
            }
            updResource.Productname = resource.Productname;
            updResource.Isdeleted = resource.Isdeleted;
            await _resourceRepository.UpdateAsync(updResource);
        }

        public async Task DeleteResource(int id)
        {
            var delResource = await _resourceRepository.GetByIdAsync(id);
            if (delResource == null)
            {
                throw new KeyNotFoundException($"Ресурс с ID {id} не найден");
            }

            try
            {
                await _resourceRepository.DeleteAsync(delResource);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var resourceStillExists = await _resourceRepository.GetByIdAsync(id) != null;

                if (!resourceStillExists)
                {          
                    return;
                }

                throw new Exception("Не удалось удалить ресурс из-за конфликта версий. Попробуйте еще раз.", ex);
            }
        }
    }
}
