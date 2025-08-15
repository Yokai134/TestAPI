using Microsoft.EntityFrameworkCore;
using TestTaskAPI.Data;
using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Model;

namespace TestTaskAPI.Repository
{
    public class ClientRepository : BaseRepository<Client>, IClientRepository
    {
        public ClientRepository(TesttaskdbContext context) : base(context) { } 

    }
}
