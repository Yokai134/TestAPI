using Microsoft.EntityFrameworkCore;
using TestTaskAPI.Data;
using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Model;

namespace TestTaskAPI.Repository
{
    public class ShippingResourceRepository : BaseRepository<ShippingResource> , IShippingResourceRepository
    {
        public ShippingResourceRepository(TesttaskdbContext context) : base(context) { }

        

    }
}
