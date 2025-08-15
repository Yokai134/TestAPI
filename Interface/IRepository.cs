namespace TestTaskAPI.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T data);
        Task UpdateAsync(T data);
        Task DeleteAsync(T data);
    }
}
