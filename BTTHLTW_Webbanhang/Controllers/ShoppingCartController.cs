using BTTHLTW_Webbanhang.Extensions;
using BTTHLTW_Webbanhang.Models;
using BTTHLTW_Webbanhang.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTTHLTW_Webbanhang.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        // Action để thêm sản phẩm vào giỏ hàng
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity
            };

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Lưu thông tin người dùng vào database (nếu đã đăng nhập)
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Tạo hoặc cập nhật thông tin giỏ hàng trong database
                var userCart = await _context.UserCarts
                    .FirstOrDefaultAsync(uc => uc.UserId == user.Id);
                if (userCart == null)
                {
                    userCart = new UserCart { UserId = user.Id, CartItems = new List<CartItem> { cartItem } };
                    _context.UserCarts.Add(userCart);
                }
                else
                {
                    var existingItem = userCart.CartItems.FirstOrDefault(i => i.ProductId == productId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity += quantity;
                    }
                    else
                    {
                        userCart.CartItems.Add(cartItem);
                    }
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            order.UserId = user?.Id;
            order.OrderDate = DateTime.UtcNow;
            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Cart");

            return View("OrderCompleted", order.Id);
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            return View(cart);
        }

        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

            if (cart != null)
            {
                cart.RemoveItem(productId);
                HttpContext.Session.SetObjectAsJson("Cart", cart);

                // Cập nhật database nếu người dùng đã đăng nhập
                var user = _userManager.GetUserAsync(User).Result;
                if (user != null)
                {
                    var userCart = _context.UserCarts.FirstOrDefault(uc => uc.UserId == user.Id);
                    if (userCart != null)
                    {
                        userCart.CartItems.RemoveAll(i => i.ProductId == productId);
                        _context.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }
}