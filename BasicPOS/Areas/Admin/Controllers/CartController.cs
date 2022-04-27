using BasicPOS.DataAccess.Repository;
using BasicPOS.Models;
using BasicPOS.Models.ViewModels;
using BasicPOS.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BasicPOS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Cart> CartList = await _unitOfWork.Cart.GetAll(u => u.IsActive == true && u.ApplicationUserId == ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value, includeProperties: "Item,Stock");

            CartVM cartVM = new CartVM
            {
                CartList = CartList,
                CartTotal = CartList.AsEnumerable().Sum(c=>(c.Quantity * c.Item.Price))
            };

            return View(cartVM);
        }

        public async Task<IActionResult> IncrementCartItem(int cartId)
        {
            var cart = await _unitOfWork.Cart.GetFirstOrDefault(u => u.Id == cartId && u.IsActive == true);

            if (cart != null)
            {
                var stock = await _unitOfWork.Stock.GetFirstOrDefault(u => u.Id == cart.StockId);

                if(stock.AvailableQuantity > 0)
                {
                    _unitOfWork.Cart.IncrementCount(cart, 1);
                    cart.UpdatedBy = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;

                    _unitOfWork.Stock.DecrementStock(stock, 1);

                    await _unitOfWork.Save();
                }

                else
                    TempData["error"] = "Insufficient stock quantity!";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DecrementCartItem(int cartId)
        {
            var cart = await _unitOfWork.Cart.GetFirstOrDefault(u => u.Id == cartId && u.IsActive == true);

            if (cart != null)
            {
                var stock = await _unitOfWork.Stock.GetFirstOrDefault(u => u.Id == cart.StockId && u.IsActive == true);

                _unitOfWork.Cart.DecrementCount(cart, 1);

                if (cart.Quantity <= 0)
                    _unitOfWork.Cart.Remove(cart);

                cart.UpdatedBy = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;

                _unitOfWork.Stock.IncrementStock(stock, 1);

                await _unitOfWork.Save();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? Id)
        {
            var CartFromDb = await _unitOfWork.Cart.GetFirstOrDefault(u => u.Id == Id);

            if (CartFromDb == null)
            {
                return NotFound();
            }

            var stock = await _unitOfWork.Stock.GetFirstOrDefault(u => u.Id == CartFromDb.StockId);

            CartFromDb.UpdatedBy = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
            _unitOfWork.Cart.Remove(CartFromDb);

            _unitOfWork.Stock.IncrementStock(stock, 1);
            
            await _unitOfWork.Save();
            TempData["success"] = "Cart deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
