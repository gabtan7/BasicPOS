using BasicPOS.DataAccess.Repository;
using BasicPOS.Models;
using BasicPOS.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BasicPOS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Cart> CartList = await _unitOfWork.Cart.GetAll(u=>u.IsActive == true, includeProperties:"Item");
            return View(CartList);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Increment(Cart obj)
        {
            obj.CreatedBy = SD.LoggedInUserName;
            obj.CreatedDate = DateTime.Now;
            //obj.AvailableQuantity = obj.Quantity;

            if (ModelState.IsValid)
            {
                await _unitOfWork.Cart.Add(obj);
                TempData["success"] = "Cart created successfully!";
            }

            await _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Cart obj)
        {
            obj.UpdatedBy = SD.LoggedInUserName;
            obj.UpdatedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _unitOfWork.Cart.Update(obj);
                TempData["success"] = "Cart updated successfully!";
            }

            await _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            var CartFromDb = await _unitOfWork.Cart.GetFirstOrDefault(u => u.Id == Id, includeProperties:"Item");

            if (CartFromDb == null)
            {
                return NotFound();
            }

            if (CartFromDb.IsActive)
                return View(CartFromDb);
            else
            {
                TempData["error"] = "Invalid action, Cart is inactive.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? Id)
        {
            var CartFromDb = await _unitOfWork.Cart.GetFirstOrDefault(u => u.Id == Id);

            if (CartFromDb == null)
            {
                return NotFound();
            }

            _unitOfWork.Cart.Remove(CartFromDb);
            await _unitOfWork.Save();
            TempData["success"] = "Cart deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
