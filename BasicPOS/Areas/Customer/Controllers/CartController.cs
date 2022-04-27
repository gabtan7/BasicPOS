using BasicPOS.DataAccess.Repository;
using BasicPOS.Models;
using BasicPOS.Models.ViewModels;
using BasicPOS.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Stripe.Checkout;
using Microsoft.AspNetCore.Authorization;

namespace BasicPOS.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public CartVM CartVM { get; set; }

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
                {
                    _unitOfWork.Cart.UpdateCartItemStatus(cart, SD.CartStatus_CANCELLED);
                    _unitOfWork.Cart.Remove(cart);
                }

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
            _unitOfWork.Cart.UpdateCartItemStatus(CartFromDb, SD.CartStatus_CANCELLED);
            _unitOfWork.Cart.Remove(CartFromDb);

            _unitOfWork.Stock.IncrementStock(stock, 1);
            
            await _unitOfWork.Save();
            TempData["success"] = "Cart deleted successfully!";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            string currentUser = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;

            CartVM.CartList = await _unitOfWork.Cart.GetAll(u => u.IsActive == true && u.Status == SD.CartStatus_InProgress && u.ApplicationUserId == currentUser, includeProperties: "Item,Stock");

            Order order = new Order();
            order.ApplicationUserId = currentUser;
            order.CreatedBy = currentUser;
            await _unitOfWork.Order.Add(order);
            await _unitOfWork.Save();

            var orderId = order.Id;

            foreach (var cart in CartVM.CartList)
            {
                OrderLine orderLine = new()
                {
                    OrderId = order.Id,
                    ItemId = cart.ItemId,
                    StockId = cart.StockId,
                    Price = cart.Item.Price,
                    Quantity = cart.Quantity,
                    Total = cart.Item.Price * cart.Quantity,
                    CreatedBy = currentUser
                };

                order.OrderTotal += orderLine.Total;

                await _unitOfWork.OrderLine.Add(orderLine);
                await _unitOfWork.Save();
            }

            #region STRIPE
            var domain = "https://localhost:44385/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
            {
              "card",
            },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={order.Id}",
                CancelUrl = domain + $"customer/cart/index",
            };

            foreach (var cart in CartVM.CartList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(cart.Item.Price * 100),//20.00 -> 2000
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cart.Item.Name
                        },

                    },
                    Quantity = cart.Quantity,
                };
                options.LineItems.Add(sessionLineItem);

            }

            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.Order.UpdatePaymentStatus(order.Id, session.Id, session.PaymentIntentId, order.OrderTotal);
            await _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
            #endregion
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            Order order = await _unitOfWork.Order.GetFirstOrDefault(u => u.Id == id && u.IsActive);

            var service = new SessionService();

            Session session = service.Get(order.SessionId);

            string sessionPayStatus = session.PaymentStatus.ToUpper();

            if (sessionPayStatus == "PAID")
            {
                _unitOfWork.Order.UpdateStatus(id, sessionPayStatus);
                await _unitOfWork.Save();
            }

            List<Cart> cartItems = (await _unitOfWork.Cart.GetAll(u => u.ApplicationUserId == order.ApplicationUserId && u.IsActive && u.Status == SD.CartStatus_InProgress)).ToList();
            _unitOfWork.Cart.UpdateCartItemStatus(cartItems, SD.CartStatus_Done);
            _unitOfWork.Cart.RemoveRange(cartItems);
            await _unitOfWork.Save();

            OrderVM orderVM = new OrderVM();

            return View("Details", orderVM);
        }
    }
}
