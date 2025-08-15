using TestTaskAPI.Model;

namespace TestTaskAPI.Interface.InterfaceServices
{
    public interface IClientServices
    {
        Task<IEnumerable<Client>> GetClientTransfer(); //Все клиенты
        Task<Client> GetClientByIdTransferAsync(int id); //Клиент по ID
        Task<Client> CreateClient(Client client);
        Task UpdateClient(Client client);
        Task DeleteClient(int id);
    }
}
