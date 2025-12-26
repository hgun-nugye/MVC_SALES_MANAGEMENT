using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;
using System.Diagnostics;

namespace QuanLyBanHang.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly SanPhamService _sanPhamService;
		private readonly KhachHangService _khachHangService;
		private readonly NhanVienService _nhanVienService;
		private readonly XaService _xaService;
		private readonly TinhService _tinhService;
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _environment;


		public HomeController(
			ILogger<HomeController> logger,
			SanPhamService sanPhamService,
			KhachHangService khachHangService,
			NhanVienService nhanVienService,
			XaService xaService,
			TinhService tinhService,
			AppDbContext context
,
			IWebHostEnvironment environment)
		{
			_logger = logger;
			_sanPhamService = sanPhamService;
			_khachHangService = khachHangService;
			_nhanVienService = nhanVienService;
			_xaService = xaService;
			_tinhService = tinhService;
			_context = context;
			_environment = environment;
		}

		public async Task<IActionResult> Index()
		{
			var sanPhams = await _sanPhamService.GetAll();
			return View(sanPhams);
		}

		[HttpGet]
		public IActionResult Login()
		{
			if (HttpContext.Session.GetString("IsLoggedIn") == "true")
			{
				return RedirectToAction("Index");
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(string username, string password)
		{
			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{
				TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!";
				return View();
			}

			// 1. Kiểm tra trong bảng KhachHang
			var khachHang = await _context.KhachHang
				.FirstOrDefaultAsync(kh => kh.TenDNKH == username);

			// Sử dụng BCrypt.Verify để so sánh mật khẩu nhập vào với mật khẩu hash trong DB
			if (khachHang != null && BCrypt.Net.BCrypt.Verify(password, khachHang.MatKhauKH))
			{
				// Đăng nhập thành công - Khách hàng
				Helpers.SessionHelper.SetCustomerSession(
					HttpContext.Session,
					khachHang.MaKH,
					khachHang.TenKH,
					khachHang.AnhKH ?? ""
				);

				TempData["SuccessMessage"] = "Đăng nhập thành công!";
				return RedirectToAction("Index", "SanPham");
			}

			// 2. Kiểm tra trong bảng NhanVien
			var nhanVien = await _context.NhanVien
				.FirstOrDefaultAsync(nv => nv.TenDNNV == username);

			// Sử dụng BCrypt.Verify cho Nhân viên
			if (nhanVien != null && BCrypt.Net.BCrypt.Verify(password, nhanVien.MatKhauNV))
			{
				// Lấy vai trò từ bảng PhanQuyen
				var phanQuyen = await _context.PhanQuyen
					.Where(pq => pq.MaNV == nhanVien.MaNV)
					.Join(_context.VaiTro,
						pq => pq.MaVT,
						vt => vt.MaVT,
						(pq, vt) => new { pq, vt })
					.FirstOrDefaultAsync();

				string userRole = phanQuyen?.vt.TenVT ?? "Employee"; // Mặc định là Employee nếu không có vai trò

				// Đăng nhập thành công - Nhân viên
				Helpers.SessionHelper.SetEmployeeSession(
					HttpContext.Session,
					nhanVien.MaNV,
					nhanVien.TenNV,
					userRole,
					nhanVien.AnhNV ?? ""
				);

				TempData["SuccessMessage"] = "Đăng nhập thành công!";

				// Chuyển hướng theo vai trò
				if (userRole == "Admin")
				{
					return RedirectToAction("Index", "NhanVien"); // Trang quản lý nhân viên
				}
				else
				{
					return RedirectToAction("Index", "SanPham"); // Trang quản lý sản phẩm
				}
			}

			// Đăng nhập thất bại
			TempData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng!";
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> SignUp()
		{
			ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh");
			ViewBag.Xa = new SelectList(Enumerable.Empty<object>(), "MaXa", "TenXa");
			ViewData["MaXaSelected"] = null;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SignUp(KhachHang model, string maTinh, IFormFile? AnhFile)
		{
			// Mã khách hàng sinh bởi DB/SP
			ModelState.Remove("MaKH");

			var hasExistingImage = !string.IsNullOrEmpty(model.AnhKH);
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
			string? filePath = model.AnhKH;
			var hasFileError = ModelState.TryGetValue("AnhFile", out var fileState) && fileState.Errors.Count > 0;
			var shouldSaveFile = AnhFile != null && AnhFile.Length > 0 && !hasFileError;
			if (shouldSaveFile)
			{
				try
				{
					var fileName = Guid.NewGuid().ToString() + Path.GetExtension(AnhFile!.FileName);
					var folderPath = Path.Combine(_environment.WebRootPath, "images", "customers");
					if (!Directory.Exists(folderPath))
						Directory.CreateDirectory(folderPath);

					var savePath = Path.Combine(folderPath, fileName);
					using var stream = new FileStream(savePath, FileMode.Create);
					await AnhFile.CopyToAsync(stream);
					filePath = fileName;
					model.AnhKH = fileName; // giữ lại khi return View
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", "Lỗi khi lưu file: " + ex.Message);
				}
			}


			ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh", maTinh);

			var xaList = await _xaService.GetByIDTinh(maTinh);
			ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", model.MaXa);
			ViewData["MaXaSelected"] = model.MaXa;
			if (!ModelState.IsValid)
			{
				// Giữ lại đường dẫn ảnh đã upload để không phải chọn lại
				model.AnhKH = filePath;
				return View(model);
			}

			try
			{
				await _khachHangService.Create(model, model.AnhFile);

				TempData["SuccessMessage"] = "Thêm khách hàng thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				if (!string.IsNullOrEmpty(filePath))
				{
					try
					{
						var fileToDelete = Path.Combine(_environment.WebRootPath, "images", "customers", filePath);
						if (System.IO.File.Exists(fileToDelete))
							System.IO.File.Delete(fileToDelete);
					}
					catch { }
				}

				ModelState.AddModelError("", "Lỗi khi thêm khách hàng: " + ex.Message);
				return View(model);
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetXaByTinh(string maTinh)
		{
			var xaList = await _xaService.GetByIDTinh(maTinh);
			return Json(xaList.Select(x => new { MaXa = x.MaXa, TenXa = x.TenXa }));
		}

		[HttpGet]
		public IActionResult Logout()
		{
			if (!Helpers.SessionHelper.IsLoggedIn(HttpContext.Session))
			{
				return RedirectToAction("Index");
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult LogoutConfirmed()
		{
			Helpers.SessionHelper.ClearSession(HttpContext.Session);

			TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công!";

			return RedirectToAction("Index", "Home");
		}

		// Action hiển thị khi user không có quyền truy cập
		public IActionResult AccessDenied()
		{
			ViewBag.Message = "Bạn không có quyền truy cập trang này!";
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
