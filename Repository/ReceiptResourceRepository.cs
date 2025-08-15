using Microsoft.EntityFrameworkCore;
using TestTaskAPI.Data;
using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Model;

namespace TestTaskAPI.Repository
{
    public class ReceiptResourceRepository : BaseRepository<ReceiptResource> , IReceiptResourceRepository
    {
        public ReceiptResourceRepository(TesttaskdbContext context) : base(context) { }

    }
}
