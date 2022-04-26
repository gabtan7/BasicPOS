using BasicPOS.DataAccess.Repository;
using BasicPOS.Models;
using BasicPOS.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BasicPOS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StockController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Stock> StockList = await _unitOfWork.Stock.GetAll(includeProperties:"Item");
            return View(StockList);
        }

        public async Task<IActionResult> Create(int? itemId)
        {
            var itemFromDb = await _unitOfWork.Item.GetFirstOrDefault(u => u.Id == itemId);

            Stock newStock = new Stock{ Item = itemFromDb};

            return View(newStock);
        }
        public async Task<IActionResult> Update(int? Id)
        {
            var stockFromDb = await _unitOfWork.Stock.GetFirstOrDefault(u => u.Id == Id, includeProperties:"Item");

            return View(stockFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Stock obj)
        {
            obj.CreatedBy = SD.LoggedInUserName;
            obj.CreatedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                await _unitOfWork.Stock.Add(obj);
                TempData["success"] = "Stock created successfully!";
            }

            await _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Stock obj)
        {
            obj.UpdatedBy = SD.LoggedInUserName;
            obj.UpdatedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _unitOfWork.Stock.Update(obj);
                TempData["success"] = "Stock updated successfully!";
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

            var StockFromDb = await _unitOfWork.Stock.GetFirstOrDefault(u => u.Id == Id, includeProperties:"Item");

            if (StockFromDb == null)
            {
                return NotFound();
            }

            if (StockFromDb.IsActive)
                return View(StockFromDb);
            else
            {
                TempData["error"] = "Invalid action, Stock is inactive.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? Id)
        {
            var StockFromDb = await _unitOfWork.Stock.GetFirstOrDefault(u => u.Id == Id);

            if (StockFromDb == null)
            {
                return NotFound();
            }

            _unitOfWork.Stock.Remove(StockFromDb);
            await _unitOfWork.Save();
            TempData["success"] = "Stock deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
