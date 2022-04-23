using BasicPOS.DataAccess.Repository;
using BasicPOS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BasicPOS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ItemController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public ItemController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Item> itemList = await _unitOfWork.Item.GetAll();
            return View(itemList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Item obj)
        {
            if (ModelState.IsValid)
            {
                obj.CreatedDate = DateTime.Now;
                obj.CreatedBy = "admin";

                await _unitOfWork.Item.Add(obj);
                await _unitOfWork.Save();
                TempData["success"] = "Item created successfully!";
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

            var ItemFromDb = await _unitOfWork.Item.GetFirstOrDefault(u => u.Id == Id);

            if (ItemFromDb == null)
            {
                return NotFound();
            }

            return View(ItemFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Item obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Item.Update(obj);
                await _unitOfWork.Save();
                TempData["success"] = "Item updated successfully!";
                return RedirectToAction("Index");
            }

            return View(obj);
        }
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            var ItemFromDb = await _unitOfWork.Item.GetFirstOrDefault(u => u.Id == Id);

            if (ItemFromDb == null)
            {
                return NotFound();
            }

            return View(ItemFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? Id)
        {
            var ItemFromDb = await _unitOfWork.Item.GetFirstOrDefault(u => u.Id == Id);

            if (ItemFromDb == null)
            {
                return NotFound();
            }

            _unitOfWork.Item.Remove(ItemFromDb);
            await _unitOfWork.Save();
            TempData["success"] = "Item deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
