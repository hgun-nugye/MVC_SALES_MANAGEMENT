using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class SanPhamController : Controller
	{
		private readonly SanPhamService _spService;
		private readonly LoaiSPService _loaiSPService;
		private readonly NhaCCService _nhaCCService;
		private readonly GianHangService _gianHangService;

		public SanPhamController(SanPhamService service, LoaiSPService loaiSPService, NhaCCService nhaCCService, GianHangService gianHangService)
		{
			_spService = service;
			_loaiSPService = loaiSPService;
			_nhaCCService = nhaCCService;
			_gianHangService = gianHangService;
		}

		public async Task<IActionResult> Index(string? search, string? status, string? type)
		{
			ViewBag.Search = search;
			ViewBag.Status = status;
			ViewBag.Type = type;

			// Load dropdown
			await LoadDropdownsAsync(type, status);

			var data = await _spService.Search(search, status, type);
			return View(data);
		}

		public async Task<IActionResult> Details(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var sp = await _spService.GetById(id);
			if (sp == null) return NotFound();

			return View(sp);
		}

		public async Task<IActionResult> Create()
		{
			await LoadDropdownsAsync();
			return View();
		}

		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(SanPham sp, IFormFile? AnhFile)
		{
			if (!ModelState.IsValid)
			{
				await LoadDropdownsAsync(sp.MaLoai, sp.TrangThai);
				return View(sp);
			}

			// Lưu file
			string? filePath = null;
			if (AnhFile != null && AnhFile.Length > 0)
			{
				var fileName = Guid.NewGuid() + Path.GetExtension(AnhFile.FileName);
				var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
				if (!Directory.Exists(folderPath))
					Directory.CreateDirectory(folderPath);
				var savePath = Path.Combine(folderPath, fileName);
				using var stream = new FileStream(savePath, FileMode.Create);
				await AnhFile.CopyToAsync(stream);
				filePath = fileName;
			}
			else
			{
				ModelState.AddModelError("", "Vui lòng chọn ảnh minh họa!");
				await LoadDropdownsAsync(sp.MaLoai, sp.TrangThai);
				return View(sp);
			}

			await _spService.Create(sp, filePath);
			TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var sp = await _spService.GetById(id);
			if (sp == null) return NotFound();

			await LoadDropdownsAsync(sp.MaLoai, sp.TrangThai);
			return View(sp);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, SanPham sp, IFormFile? AnhFile)
		{
			if (id != sp.MaSP) return NotFound();

			if (!ModelState.IsValid)
			{
				await LoadDropdownsAsync(sp.MaLoai, sp.TrangThai);
				return View(sp);
			}

			// Giữ ảnh cũ nếu không chọn ảnh mới
			string? filePath = sp.AnhMH;
			if (AnhFile != null && AnhFile.Length > 0)
			{
				var fileName = Guid.NewGuid() + Path.GetExtension(AnhFile.FileName);
				var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
				if (!Directory.Exists(folderPath))
					Directory.CreateDirectory(folderPath);
				var savePath = Path.Combine(folderPath, fileName);
				using var stream = new FileStream(savePath, FileMode.Create);
				await AnhFile.CopyToAsync(stream);
				filePath = fileName;
			}

			await _spService.Update(sp, filePath);
			TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
			return RedirectToAction(nameof(Index));
		}
				
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var sp = await _spService.GetById(id);
			if (sp == null) return NotFound();

			return View(sp);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				await _spService.Delete(id);
				TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
			}
			catch
			{
				TempData["ErrorMessage"] = "Không thể xóa sản phẩm này!";
			}
				return RedirectToAction(nameof(Index));
		}

		// Load dropdowns
		private async Task LoadDropdownsAsync(string? selectedLoai = null, string? selectedTrangThai = null)
		{
			var loaiList = await _loaiSPService.GetAll();
			ViewBag.LoaiSP = new SelectList(loaiList, "MaLoai", "TenLoai", selectedLoai);

			var nhaCCList = await _nhaCCService.GetAll();
			ViewBag.NhaCC = new SelectList(nhaCCList, "MaNCC", "TenNCC");

			var gianHangList = await _gianHangService.GetAll();
			ViewBag.GianHang = new SelectList(gianHangList, "MaGH", "TenGH");

			var statusList = new List<SelectListItem>
			{
				new() { Text = "Còn Hàng", Value = "Còn Hàng" },
				new() { Text = "Hết Hàng", Value = "Hết Hàng" },
				new() { Text = "Cháy Hàng", Value = "Cháy Hàng" },
				new() { Text = "Sắp Hết", Value = "Sắp Hết" }
			};
			ViewBag.TrangThai = new SelectList(statusList, "Value", "Text", selectedTrangThai);
		}
	}
}
