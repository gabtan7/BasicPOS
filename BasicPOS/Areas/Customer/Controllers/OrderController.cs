using BasicPOS.DataAccess.Repository;
using BasicPOS.Models;
using BasicPOS.Models.ViewModels;
using BasicPOS.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Stripe.Checkout;

namespace BasicPOS.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Order> OrderList = await _unitOfWork.Order.GetAll(u => u.IsActive == true && u.ApplicationUserId == ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

            return View(OrderList);
        }

        public async Task<IActionResult> OrderDetails(int? orderId)
        {
            var order = await _unitOfWork.OrderLine.GetFirstOrDefault(u => u.OrderId == orderId);

            return View(order);
        }
    }
}
