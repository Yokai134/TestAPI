using Microsoft.EntityFrameworkCore;
using TestTaskAPI.Data;
using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Model;

namespace TestTaskAPI.Repository
{
    public class ReceiptRepository : BaseRepository<ReceiptDocumet> , IReceiptRepository
    {
        public ReceiptRepository(TesttaskdbContext context) : base(context) { }

    }
}
