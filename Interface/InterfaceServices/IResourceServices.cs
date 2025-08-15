using TestTaskAPI.Model;

namespace TestTaskAPI.Interface.InterfaceServices
{
    public interface IResourceServices
    {
        Task<IEnumerable<Resource>> GetResourceTransfer(); //Все ресурсы
        Task<Resource> GetResourceByIdTransferAsync(int id); //Ресурсы по ID
        Task<Resource> CreateResource(Resource Resource);
        Task UpdateResource(Resource Resource);
        Task DeleteResource(int id);
    }
}
