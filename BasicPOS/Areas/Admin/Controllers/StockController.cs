using BasicPOS.DataAccess.Repository;
using BasicPOS.Models;
using Microsoft.AspNetCore.Mvc;

namespace BasicPOS.Web.Areas.Admin.Controllers
{
    public class StockController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Stock> StockList = await _unitOfWork.Stock.GetAll();
            return View(StockList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Stock obj)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Stock.Add(obj);
                await _unitOfWork.Save();
                TempData["success"] = "Stock created successfully!";
                return RedirectToAction("Index");
            }

            return View(obj);
        }
        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            var StockFromDb = await _unitOfWork.Stock.GetFirstOrDefault(u => u.Id == Id);

            if (StockFromDb == null)
            {
                return NotFound();
            }

            return View(StockFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Stock obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Stock.Update(obj);
                await _unitOfWork.Save();
                TempData["success"] = "Stock updated successfully!";
                return RedirectToAction("Index");
            }

            return View(obj);
        }
    }
}
