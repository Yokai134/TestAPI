using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;

namespace TestTaskAPI.Services
{
    public class StatusServices : IStatusServices
    {
        private readonly IStatusRepository _statusRepository;

        public StatusServices(IStatusRepository statusRepository) 
        { 
            _statusRepository = statusRepository; 
        }

        public async Task<IEnumerable<Status>> GetStatusTransfer()
        {
            var status = await _statusRepository.GetAllAsync();
            if (status == null)
            {
                throw new KeyNotFoundException("Данные по статусам не найдены");
            }
            return status.ToList();
        }
    }
}
