using Microsoft.EntityFrameworkCore;
using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;
using TestTaskAPI.Repository;

namespace TestTaskAPI.Services
{
    public class MeasureServices : IMeasureServices
    {
        private readonly IMeasureRepository _measureRepository;

        public MeasureServices(IMeasureRepository measureRepository)
        {
            _measureRepository = measureRepository;
        }

        public async Task<IEnumerable<Measurе>> GetMeasureTransfer()
        {
            var Measure = await _measureRepository.GetAllAsync();

            if (Measure == null)
            {
                throw new KeyNotFoundException("Данные по единицам измерения не найдены");
            }

            return Measure.OrderBy(x => x.Id).ToList();
        }
        public async Task<Measurе> GetMeasureByIdTransferAsync(int id)
        {
            var updMeasure = await _measureRepository.GetByIdAsync(id);
            if (updMeasure == null)
            {
                throw new KeyNotFoundException("Данные по единицам измерения не найдены");
            }

            return updMeasure;
        }
        public async Task<Measurе> CreateMeasure(Measurе measure)
        {
            await _measureRepository.AddAsync(measure);
            return measure;
        }

        public async Task UpdateMeasure(Measurе measure)
        {
            var updMeasure = await _measureRepository.GetByIdAsync(measure.Id);
            if (updMeasure == null)
            {
                throw new KeyNotFoundException($"Единицам измерени {measure.Measurename} не найден");
            }
            updMeasure.Measurename = measure.Measurename;
            updMeasure.Isdeleted = measure.Isdeleted;
            await _measureRepository.UpdateAsync(updMeasure);
        }

        public async Task DeleteMeasure(int id)
        {
            var delMeasure = await _measureRepository.GetByIdAsync(id);
            if (delMeasure == null)
            {
                throw new KeyNotFoundException($"Единицам измерени {delMeasure.Measurename} не найден");
            }

            try
            {
                await _measureRepository.DeleteAsync(delMeasure);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var resourceStillExists = await _measureRepository.GetByIdAsync(id) != null;

                if (!resourceStillExists)
                {
                    return;
                }

                throw new Exception("Не удалось удалить ресурс из-за конфликта версий. Попробуйте еще раз.", ex);
            }
        }
    }
}
