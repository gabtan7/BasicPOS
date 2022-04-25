using BasicPOS.DataAccess.Repository;
using BasicPOS.Models;
using BasicPOS.Utility;
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
        public async Task<IActionResult> CreateUpdate(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return View(new Item());
            }

            var ItemFromDb = await _unitOfWork.Item.GetFirstOrDefault(u => u.Id == Id);

            if(ItemFromDb.IsActive)
                return View(ItemFromDb);
            else
            {
                TempData["error"] = "Invalid action, item is inactive.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUpdate(Item obj)
        {
            if (obj.Id == 0)
            {
                obj.CreatedBy = SD.LoggedInUserName;
                obj.CreatedDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    await _unitOfWork.Item.Add(obj);
                    TempData["success"] = "Company created successfully!";
                }

                else
                    return View(obj);
            }
            else
            {
                obj.UpdatedBy = SD.LoggedInUserName;
                obj.UpdatedDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    _unitOfWork.Item.Update(obj);
                    TempData["success"] = "Item updated successfully!";
                }

                else
                    return View(obj);
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

            var ItemFromDb = await _unitOfWork.Item.GetFirstOrDefault(u => u.Id == Id);

            if (ItemFromDb == null)
            {
                return NotFound();
            }

            if (ItemFromDb.IsActive)
                return View(ItemFromDb);
            else
            {
                TempData["error"] = "Invalid action, item is inactive.";
                return RedirectToAction("Index");
            }
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
