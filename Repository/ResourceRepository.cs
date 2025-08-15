using Microsoft.EntityFrameworkCore;
using TestTaskAPI.Data;
using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Model;

namespace TestTaskAPI.Repository
{
    public class ResourceRepository : BaseRepository<Resource>, IResourceRepository
    {
        public ResourceRepository(TesttaskdbContext context) : base(context) { }

    }
}
