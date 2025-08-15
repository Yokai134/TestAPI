using Microsoft.EntityFrameworkCore;
using TestTaskAPI.Data;
using TestTaskAPI.Interface;
using TestTaskAPI.Model;

namespace TestTaskAPI.Repository
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly TesttaskdbContext _dbContext;

        public BaseRepository(TesttaskdbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
        public async Task AddAsync(T data)
        {
            await _dbContext.Set<T>().AddAsync(data);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T data)
        {
            _dbContext.Set<T>().Update(data);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T data)
        {
            _dbContext.Remove(data);
            //if (data is Client delClient)
            //{
            //     delClient.Isdeleted = true;
            //    _dbContext.Update(data);
            //}
            //else
            //{
            //    _dbContext.Remove(data);
            //}
            await _dbContext.SaveChangesAsync();
        }
    }
}
