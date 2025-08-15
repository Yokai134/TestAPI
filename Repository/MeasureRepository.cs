using Microsoft.EntityFrameworkCore;
using TestTaskAPI.Data;
using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Model;

namespace TestTaskAPI.Repository
{
    public class MeasureRepository : BaseRepository<Measurе>, IMeasureRepository
    {
        public MeasureRepository(TesttaskdbContext context) : base(context) { }


    }
}
