using BTTHLTW_Webbanhang.Models;

namespace BTTHLTW_Webbanhang.Data
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();
    }
}
