using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;
using TestTaskAPI.Repository;

namespace TestTaskAPI.Services
{
    public class ClientServices : IClientServices
    {
        private readonly IClientRepository _clientRepository;

        public ClientServices(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<IEnumerable<Client>> GetClientTransfer()
        {
            var client = await _clientRepository.GetAllAsync();

            if(client == null)
            {
                throw new KeyNotFoundException("Данные по клиентам не найдены");
            }

            return client.OrderBy(x=>x.Id).ToList();  
        }
        public async Task<Client> GetClientByIdTransferAsync(int id)
        {
            var updClient = await _clientRepository.GetByIdAsync(id);
            if (updClient == null)
            {
                throw new KeyNotFoundException("Данные по клиентам не найдены");
            }

            return updClient;
        }
        public async Task<Client> CreateClient(Client client)
        {
            await _clientRepository.AddAsync(client);
            return client;
        }

        public async Task UpdateClient(Client client)
        {
            var updClient = await _clientRepository.GetByIdAsync(client.Id);
            if(updClient == null)
            {
                throw new KeyNotFoundException($"Клиент {client.Clientname} не найден");
            }
            updClient.Clientname = client.Clientname;
            updClient.Address = client.Address;
            updClient.Isdeleted = client.Isdeleted;
            await _clientRepository.UpdateAsync(updClient);
        }
       
        public async Task DeleteClient(int id)
        {
            var delClient = await _clientRepository.GetByIdAsync(id);
            if(delClient == null)
            {
                throw new KeyNotFoundException($"Клиент {delClient.Clientname} не найден");
            }

            try
            {
                await _clientRepository.DeleteAsync(delClient);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var resourceStillExists = await _clientRepository.GetByIdAsync(id) != null;

                if (!resourceStillExists)
                {
                    return;
                }

                throw new Exception("Не удалось удалить ресурс из-за конфликта версий. Попробуйте еще раз.", ex);
            }
        }


    }
}
