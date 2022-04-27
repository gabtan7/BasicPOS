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

        public async Task<IActionResult> Search(string? keyword)
        {
            IEnumerable<Item> itemList;

            if (string.IsNullOrEmpty(keyword))
                itemList = await _unitOfWork.Item.GetAll(i => i.IsActive);
            else
                itemList = await _unitOfWork.Item.GetAll(i => i.Name.Contains(keyword) && i.IsActive);

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
                if (ModelState.IsValid)
                {
                    obj.CreatedBy = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
                    await _unitOfWork.Item.Add(obj);
                    TempData["success"] = "Item created successfully!";
                }

                else
                    return View(obj);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    obj.UpdatedBy = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
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

            ItemFromDb.UpdatedBy = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
            _unitOfWork.Item.Remove(ItemFromDb);
            await _unitOfWork.Save();
            TempData["success"] = "Item deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
