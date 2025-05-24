﻿namespace BTTHLTW_Webbanhang.Data
{ 
    using BTTHLTW_Webbanhang.Models;
    using System.Collections.Generic;
        public interface IProductRepository
        {
            IEnumerable<Product> GetAll();
            Product GetById(int id);
            void Add(Product product);
            void Update(Product product);
            void Delete(int id);
        }
}
