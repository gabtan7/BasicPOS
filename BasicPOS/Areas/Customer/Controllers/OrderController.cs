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

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Order> orderList;

            if (User.IsInRole(SD.Role_Admin))
                orderList = await _unitOfWork.Order.GetAll(u => u.IsActive == true);

            else
                orderList = await _unitOfWork.Order.GetAll(u => u.IsActive == true && u.ApplicationUserId == ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

            return View(orderList.OrderByDescending(x=>x.Id));
        }

        public async Task<IActionResult> Details(int? Id)
        {
            OrderVM = new OrderVM();
            OrderVM.Order = await _unitOfWork.Order.GetFirstOrDefault(u => u.Id == Id && u.IsActive);
            OrderVM.Order.DateApproved = DateTime.Now;
            OrderVM.OrderLine = await _unitOfWork.OrderLine.GetAll(u => u.OrderId == Id && u.IsActive, includeProperties: "Item");

            return View(OrderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve()
        {
            OrderVM.Order = await _unitOfWork.Order.GetFirstOrDefault(u => u.Id == OrderVM.Order.Id && u.IsActive);
            OrderVM.OrderLine = await _unitOfWork.OrderLine.GetAll(u => u.OrderId == OrderVM.Order.Id && u.IsActive, includeProperties: "Item");

            if (OrderVM.Order.OrderStatus == SD.OrderStatus_Paid)
            {
                _unitOfWork.Order.UpdateStatus(OrderVM.Order.Id, SD.OrderStatus_Done);
                await _unitOfWork.Save();
            }

            else
                TempData["error"] = "Order already approved!";

            return RedirectToAction("Details", new {id = OrderVM.Order.Id});
        }
    }
}
