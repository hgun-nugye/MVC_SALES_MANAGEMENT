using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class LoaiSPController : Controller
	{
		private readonly LoaiSPService _service;

		public LoaiSPController(LoaiSPService service)
		{
			_service = service;
		}

		public async Task<IActionResult> Index(string? search, string? group)
		{
			ViewBag.Search = search;
			ViewBag.Group = group;

			// Dropdown nhóm sản phẩm
			var groups = await _service.GetGroups();
			ViewBag.MaNhom = new SelectList(groups, "MaNhom", "TenNhom", group);

			// Dữ liệu chính
			var model = await _service.Search(search, group);
			
			return View(model);
		}

		public async Task<IActionResult> Details(string id)
		{
			var tinh = await _service.GetById(id);

			if (tinh == null)
				return NotFound();

			return View(tinh);
		}

		// CREATE - GET
		[HttpGet]
		public async Task<IActionResult> Create()
		{
			ViewBag.MaNhom = new SelectList(await _service.GetGroups(), "MaNhom", "TenNhom");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(LoaiSP model)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.MaNhom = new SelectList(await _service.GetGroups(), "MaNhom", "TenNhom");
				return View(model);
			}

			try
			{
				await _service.Insert(model);
				TempData["SuccessMessage"] = "Thêm loại sản phẩm thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
				return View(model);
			}
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			var loai = await _service.GetById(id);

			if (loai == null)
				return NotFound();

			ViewBag.NhomSP = new SelectList(await _service.GetGroups(), "MaNhom", "TenNhom", loai.MaNhom);

			return View(loai);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(LoaiSPDto model)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.NhomSP = new SelectList(await _service.GetGroups(), "MaNhom", "TenNhom", model.MaNhom);
				return View(model);
			}

			try
			{
				await _service.Update(model);
				TempData["SuccessMessage"] = "Cập nhật thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
				ViewBag.NhomSPList = new SelectList(await _service.GetGroups(), "MaNhom", "TenNhom", model.MaNhom);
				return View(model);
			}
		}

		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			var obj = await _service.GetById(id);

			if (obj == null)
				return NotFound();

			return View(obj);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				await _service.Delete(id);
				TempData["SuccessMessage"] = "Đã xóa loại sản phẩm thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
