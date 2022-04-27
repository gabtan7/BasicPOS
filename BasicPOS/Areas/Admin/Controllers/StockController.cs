using BasicPOS.DataAccess.Repository;
using BasicPOS.Models;
using BasicPOS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task<IActionResult> Search(string? keyword)
        {
            IEnumerable<Stock> stockList;

            if (string.IsNullOrEmpty(keyword))
                stockList = await _unitOfWork.Stock.GetAll(u=>u.IsActive == true, includeProperties: "Item");
            else
                stockList = await _unitOfWork.Stock.GetAll(u => u.IsActive == true && u.Item.Name.Contains(keyword), includeProperties: "Item");

            //IEnumerable<Stock> stockListGrouped;

            //stockListGrouped = stockList.GroupBy(i => i.ItemId).Select(n => new Stock
            //{
            //    ItemId = n.Key,
            //    Item = n.Select(i=>i.Item).FirstOrDefault(),
            //    AvailableQuantity = n.Sum(q=>q.Quantity)
            //}).ToList();

            return View(stockList.Where(s => s.AvailableQuantity > 0));
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
            var existingStock = await _unitOfWork.Stock.GetFirstOrDefault(u => u.ItemId == obj.ItemId && u.IsActive == true);

            string currentUser = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;

            if (existingStock != null)
            {
                existingStock.Quantity += obj.Quantity;
                existingStock.AvailableQuantity += obj.Quantity;

                if(ModelState.IsValid)
                {
                    existingStock.UpdatedBy = currentUser;
                    _unitOfWork.Stock.UntrackEntity(obj);
                    _unitOfWork.Stock.Update(existingStock);
                    TempData["success"] = "Stock added successfully!";
                }
            }

            else
            {
                obj.AvailableQuantity = obj.Quantity;

                if (ModelState.IsValid)
                {
                    obj.CreatedBy = currentUser;
                    await _unitOfWork.Stock.Add(obj);
                    TempData["success"] = "Stock created successfully!";
                }
            }

            await _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Stock obj)
        {
            if (ModelState.IsValid)
            {
                obj.UpdatedBy = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
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

            StockFromDb.UpdatedBy = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
            _unitOfWork.Stock.Remove(StockFromDb);
            await _unitOfWork.Save();
            TempData["success"] = "Stock deleted successfully!";
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(Stock obj)
        {
            //IEnumerable<Stock> stockList = await _unitOfWork.Stock.GetAll(u => u.ItemId == obj.ItemId);

            var stock = await _unitOfWork.Stock.GetFirstOrDefault(u => u.Id == obj.Id && u.IsActive == true);

            string currentUser = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;

            //if (stockList.AsEnumerable().Sum(q => q.Quantity) > 1)
            //{
            if(stock.Quantity > 1)
            { 
                //var stock = stockList.Where(q=>q.Quantity > 1).OrderBy(s => s.CreatedDate).FirstOrDefault();

                var cartFromDb = await _unitOfWork.Cart.GetFirstOrDefault(
                    u => u.ApplicationUserId == currentUser && u.ItemId == stock.ItemId && u.IsActive == true);

                Cart cart = new Cart();
                cart.ApplicationUserId = currentUser;
                cart.Quantity = 1;
                cart.StockId = stock.Id;
                cart.ItemId = stock.ItemId;

                if (cartFromDb == null)
                {
                    obj.CreatedBy = currentUser;
                    await _unitOfWork.Cart.Add(cart);
                }
                else
                {
                    obj.UpdatedBy = currentUser;
                    _unitOfWork.Cart.IncrementCount(cartFromDb, cart.Quantity);
                }

                _unitOfWork.Stock.DecrementStock(stock, cart.Quantity);

                await _unitOfWork.Save();
                TempData["success"] = "Item Added to Cart!";
                //HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
            }

            else
            {
                TempData["error"] = "Item does not have remaining stocks!";
            }

            return RedirectToAction("Search");
        }
    }
}
