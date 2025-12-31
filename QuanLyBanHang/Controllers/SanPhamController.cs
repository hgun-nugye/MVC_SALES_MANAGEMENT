using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;
namespace QuanLyBanHang.Controllers
{
	public class SanPhamController : Controller
	{
		private readonly SanPhamService _spService;
		private readonly LoaiSPService _loaiSPService;
		private readonly HangSXService _hangService;
		private readonly TrangThaiService _trangThaiService;
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _environment;
		public SanPhamController(
			AppDbContext context,
			SanPhamService spService,
			LoaiSPService loaiSPService,
			HangSXService hangService,
			TrangThaiService trangThaiService,
			IWebHostEnvironment environment)
		{
			_spService = spService;
			_loaiSPService = loaiSPService;
			_hangService = hangService;
			_trangThaiService = trangThaiService;
			_environment = environment;
			_context = context;
		}

		public async Task<IActionResult> Index(string? search, string? maTT, string? maLoai)
		{
			ViewBag.Search = search;
			ViewBag.MaTT = maTT;
			ViewBag.MaLoai = maLoai;

			// Load dropdown
			await LoadDropdownsAsync(maLoai, maTT);

			var data = await _spService.Search(search, maTT, maLoai);
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
			if (string.IsNullOrEmpty(sp.MaTT))
			{
				sp.MaTT = "TT1"; // mặc định: còn bán
			}

			// Validate file upload (allow dùng lại ảnh cũ khi form reload)
			var hasExistingImage = !string.IsNullOrEmpty(sp.AnhMH);
			if (AnhFile != null && AnhFile.Length > 0)
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
					var folderPath = Path.Combine(_environment.WebRootPath, "images", "products");
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
				await LoadDropdownsAsync(sp.MaLoai, sp.MaTT, sp.MaHangSX);
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
						var fileToDelete = Path.Combine(_environment.WebRootPath, "images","products", filePath);
						if (System.IO.File.Exists(fileToDelete))
							System.IO.File.Delete(fileToDelete);
					}
					catch { }
				}

				ModelState.AddModelError("", "Lỗi khi thêm sản phẩm: " + ex.Message);
				await LoadDropdownsAsync(sp.MaLoai, sp.MaTT, sp.MaHangSX);
				return View(sp);
			}
		}

		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var sp = await _spService.GetById(id);
			if (sp == null) return NotFound();

			// Chuyển đổi thủ công sang Dto
			var spDto = new SanPhamDto
			{
				MaSP = sp.MaSP,
				TenSP = sp.TenSP,
				GiaBan = sp.GiaBan,
				AnhMH = sp.AnhMH,
				TrongLuong = sp.TrongLuong,
				MaTT = sp.MaTT,
				MoTaSP = sp.MoTaSP,
				ThanhPhan = sp.ThanhPhan,
				CongDung = sp.CongDung,
				HDSD = sp.HDSD,
				HDBaoQuan = sp.HDBaoQuan,
				MaLoai = sp.MaLoai,
				MaHangSX = sp.MaHangSX,
				DoiTuongSuDung = sp.DoiTuongSuDung
			};

			await LoadDropdownsAsync(sp.MaLoai, sp.MaTT, sp.MaHangSX);
			return View(spDto); // Truyền Dto vào View
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, SanPhamDto sp, IFormFile? AnhFile)
		{
			if (id != sp.MaSP) return NotFound();

			if (!ModelState.IsValid)
			{
				await LoadDropdownsAsync(sp.MaLoai, sp.MaTT, sp.MaHangSX);
				return View(sp);
			}

			// Giữ ảnh cũ nếu không chọn ảnh mới
			string? filePath = sp.AnhMH;
			if (AnhFile != null && AnhFile.Length > 0)
			{
				var fileName = Guid.NewGuid() + Path.GetExtension(AnhFile.FileName);
				var folderPath = Path.Combine(_environment.WebRootPath, "images", "products");
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
		private async Task LoadDropdownsAsync(string? selectedLoai = null, string? selectedMaTT = null, string? selectedHang = null)
		{
			var loaiList = await _loaiSPService.GetAll();
			ViewBag.LoaiSP = new SelectList(loaiList, "MaLoai", "TenLoai", selectedLoai);

			var hangList = await _hangService.GetAll();
			ViewBag.MaHangSX = new SelectList(hangList, "MaHangSX", "TenHangSX", selectedHang);

			// Load TrangThai from service (Proc)
			var trangThaiList = await _trangThaiService.GetAll();
			ViewBag.MaTT = new SelectList(trangThaiList, "MaTT", "TenTT", selectedMaTT);
		}

		[HttpGet]
		public async Task<IActionResult> GetGia(string id)
		{
			var sp = await _spService.GetById(id);
			return Json(new { gia = sp?.GiaBan ?? 0, ton = sp?.SoLuongTon ?? 0 });
		}
	}
}
