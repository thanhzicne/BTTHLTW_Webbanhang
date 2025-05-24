using BTTHLTW_Webbanhang.Data;
using BTTHLTW_Webbanhang.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;

namespace BTTHLTW_Webbanhang.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public IActionResult Add()
        {
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl, List<IFormFile> imageUrls)
        {
            if (ModelState.IsValid)
            {
                // Xử lý ảnh chính
                if (imageUrl != null)
                {
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                // Xử lý danh sách ảnh phụ
                if (imageUrls != null && imageUrls.Any())
                {
                    product.ImageUrls = new List<string>();
                    foreach (var file in imageUrls)
                    {
                        if (file.Length > 0)
                        {
                            var imagePath = await SaveImage(file);
                            if (imagePath != null)
                            {
                                product.ImageUrls.Add(imagePath);
                            }
                        }
                    }
                }

                _productRepository.Add(product);
                return RedirectToAction("Index");
            }

            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Display a list of products
        public IActionResult Index()
        {
            var products = _productRepository.GetAll();
            return View(products);
        }

        // Display a single product
        public IActionResult Display(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Show the product update form
        public IActionResult Update(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Process the product update
        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile imageUrl, List<IFormFile> imageUrls)
        {
            if (ModelState.IsValid)
            {
                // Giữ ảnh hiện tại nếu không upload mới
                var existingProduct = _productRepository.GetById(product.Id);
                if (existingProduct != null)
                {
                    product.ImageUrl = string.IsNullOrEmpty(product.ImageUrl) ? existingProduct.ImageUrl : product.ImageUrl;
                    product.ImageUrls = product.ImageUrls ?? existingProduct.ImageUrls ?? new List<string>();
                }

                // Xử lý ảnh chính mới
                if (imageUrl != null)
                {
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                // Xử lý danh sách ảnh phụ mới
                if (imageUrls != null && imageUrls.Any())
                {
                    product.ImageUrls = product.ImageUrls ?? new List<string>();
                    foreach (var file in imageUrls)
                    {
                        if (file.Length > 0)
                        {
                            var imagePath = await SaveImage(file);
                            if (imagePath != null)
                            {
                                product.ImageUrls.Add(imagePath);
                            }
                        }
                    }
                }

                _productRepository.Update(product);
                return RedirectToAction("Index");
            }

            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Show the product delete confirmation
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Process the product deletion
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _productRepository.Delete(id);
            return RedirectToAction("Index");
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            // Kiểm tra loại file và kích thước
            if (image == null || image.Length == 0)
                return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(image.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Chỉ chấp nhận file ảnh với định dạng JPG, JPEG, PNG, GIF.");

            if (image.Length > 10 * 1024 * 1024) // Giới hạn 10MB
                throw new ArgumentException("Kích thước file không được vượt quá 10MB.");

            // Tạo thư mục wwwroot/images nếu chưa tồn tại
            var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            // Tạo tên file duy nhất để tránh ghi đè
            var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            var savePath = Path.Combine(imagesFolder, uniqueFileName);

            // Lưu file
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return "/images/" + uniqueFileName;
        }
    }
}