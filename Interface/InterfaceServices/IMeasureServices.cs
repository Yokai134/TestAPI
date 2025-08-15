using TestTaskAPI.Model;

namespace TestTaskAPI.Interface.InterfaceServices
{
    public interface IMeasureServices
    {
        Task<IEnumerable<Measurе>> GetMeasureTransfer(); //Все ед измерения
        Task<Measurе> GetMeasureByIdTransferAsync(int id); //Ед измерения по ID
        Task<Measurе> CreateMeasure(Measurе measure);
        Task UpdateMeasure(Measurе measure);
        Task DeleteMeasure(int id);
    }
}
