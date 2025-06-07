using BTTHLTW_Webbanhang.Models;
using System.Collections.Generic;

namespace BTTHLTW_Webbanhang.ViewModels
{
    public class CategoryDetailsViewModel
    {
        public Category Category { get; set; }
        public List<Product> Products { get; set; }
    }
}
