using BTTHLTW_Webbanhang.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BTTHLTW_Webbanhang.Repository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}