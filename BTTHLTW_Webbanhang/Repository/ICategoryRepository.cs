using BTTHLTW_Webbanhang.Models;
using System.Threading.Tasks;

namespace BTTHLTW_Webbanhang.Repository
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
    }
}