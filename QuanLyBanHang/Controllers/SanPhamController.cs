using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;
using System.Linq;

namespace QuanLyBanHang.Controllers
{
	public class SanPhamController : Controller
	{
		private readonly SanPhamService _spService;
		private readonly LoaiSPService _loaiSPService;
		private readonly HangService _hangService;
		private readonly IWebHostEnvironment _environment;
		public SanPhamController(
			SanPhamService service,
			LoaiSPService loaiSPService,
			HangService hangService,
			IWebHostEnvironment environment)
		{
			_spService = service;
			_loaiSPService = loaiSPService;
			_hangService = hangService;
			_environment = environment;
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
			// Bỏ qua validation cho MaSP vì nó được tự động generate trong stored procedure
			ModelState.Remove("MaSP");
			ModelState.Remove("AnhFile"); // Bỏ qua validation cho AnhFile vì dùng parameter riêng

			// Validate file upload (allow dùng lại ảnh cũ khi form reload)
			var hasExistingImage = !string.IsNullOrEmpty(sp.AnhMH);
			if ((AnhFile == null || AnhFile.Length == 0) && !hasExistingImage)
			{
				ModelState.AddModelError("AnhFile", "Vui lòng chọn ảnh minh họa!");
			}
			else if (AnhFile != null && AnhFile.Length > 0)
			{
				if (AnhFile.Length > 5 * 1024 * 1024) // 5MB
				{
					ModelState.AddModelError("AnhFile", "Kích thước ảnh không được vượt quá 5MB!");
				}
				else
				{
					var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
					var extension = Path.GetExtension(AnhFile.FileName).ToLowerInvariant();
					if (!allowedExtensions.Contains(extension))
					{
						ModelState.AddModelError("AnhFile", "Chỉ chấp nhận file ảnh: JPG, PNG, GIF, WEBP!");
					}
				}
			}

			// Lưu file ngay khi hợp lệ để giữ lại khi reload form
			string? filePath = sp.AnhMH;
			var hasFileError = ModelState.TryGetValue("AnhFile", out var fileState) && fileState.Errors.Count > 0;
			var shouldSaveFile = AnhFile != null && AnhFile.Length > 0 && !hasFileError;
			if (shouldSaveFile)
			{
				try
				{
					var fileName = Guid.NewGuid().ToString() + Path.GetExtension(AnhFile!.FileName);
					var folderPath = Path.Combine(_environment.WebRootPath, "images");
					if (!Directory.Exists(folderPath))
						Directory.CreateDirectory(folderPath);

					var savePath = Path.Combine(folderPath, fileName);
					using var stream = new FileStream(savePath, FileMode.Create);
					await AnhFile.CopyToAsync(stream);
					filePath = fileName;
					sp.AnhMH = fileName; // giữ lại khi return View
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", "Lỗi khi lưu file: " + ex.Message);
				}
			}

			if (!ModelState.IsValid)
			{
				// Giữ lại đường dẫn ảnh đã upload để không phải chọn lại
				sp.AnhMH = filePath;
				await LoadDropdownsAsync(sp.MaLoai, sp.TrangThai, sp.MaHang);
				return View(sp);
			}

			try
			{
				await _spService.Create(sp, filePath);
				TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				// Xóa file đã upload nếu có lỗi
				if (!string.IsNullOrEmpty(filePath))
				{
					try
					{
						var fileToDelete = Path.Combine(_environment.WebRootPath, "images", filePath);
						if (System.IO.File.Exists(fileToDelete))
							System.IO.File.Delete(fileToDelete);
					}
					catch { }
				}

				ModelState.AddModelError("", "Lỗi khi thêm sản phẩm: " + ex.Message);
				await LoadDropdownsAsync(sp.MaLoai, sp.TrangThai, sp.MaHang);
				return View(sp);
			}
		}

		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var sp = await _spService.GetById(id);
			if (sp == null) return NotFound();

			await LoadDropdownsAsync(sp.MaLoai, sp.TrangThai, sp.MaHang);
			return View(sp);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, SanPham sp, IFormFile? AnhFile)
		{
			if (id != sp.MaSP) return NotFound();

			if (!ModelState.IsValid)
			{
				await LoadDropdownsAsync(sp.MaLoai, sp.TrangThai, sp.MaHang);
				return View(sp);
			}

			// Giữ ảnh cũ nếu không chọn ảnh mới
			string? filePath = sp.AnhMH;
			if (AnhFile != null && AnhFile.Length > 0)
			{
				var fileName = Guid.NewGuid() + Path.GetExtension(AnhFile.FileName);
				var folderPath = Path.Combine(_environment.WebRootPath, "images");
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
		private async Task LoadDropdownsAsync(string? selectedLoai = null, string? selectedTrangThai = null, string? selectedHang = null)
		{
			var loaiList = await _loaiSPService.GetAll();
			ViewBag.LoaiSP = new SelectList(loaiList, "MaLoai", "TenLoai", selectedLoai);

			var hangList = await _hangService.GetAll();
			ViewBag.MaHang = new SelectList(hangList, "MaHang", "TenHang", selectedHang);

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
