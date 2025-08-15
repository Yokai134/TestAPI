using TestTaskAPI.Model;

namespace TestTaskAPI.Interface.InterfaceServices
{
    public interface IStatusServices
    {
        Task<IEnumerable<Status>> GetStatusTransfer();
    }
}
