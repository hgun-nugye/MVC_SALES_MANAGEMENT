using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

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

            // Kiểm tra trong bảng KhachHang
            var khachHang = await _context.KhachHang
                .FirstOrDefaultAsync(kh => kh.TenDNKH == username && kh.MatKhauKH == password);

            if (khachHang != null)
            {
                // Đăng nhập thành công - Khách hàng
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("IsCustomer", "true");
                HttpContext.Session.SetString("UserId", khachHang.MaKH);
                HttpContext.Session.SetString("UserName", khachHang.TenKH);
                HttpContext.Session.SetString("UserType", "Customer");

                TempData["SuccessMessage"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "SanPham");
            }

            // Kiểm tra trong bảng NhanVien
            var nhanVien = await _context.NhanVien
                .FirstOrDefaultAsync(nv => nv.TenDNNV == username && nv.MatKhauNV == password);

            if (nhanVien != null)
            {
                // Đăng nhập thành công - Nhân viên
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("IsCustomer", "false");
                HttpContext.Session.SetString("UserId", nhanVien.MaNV);
                HttpContext.Session.SetString("UserName", nhanVien.TenNV);
                HttpContext.Session.SetString("UserType", "Employee");

                TempData["SuccessMessage"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "SanPham");
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
		public async Task<IActionResult> SignUp(KhachHang model, short maTinh, IFormFile? AnhFile)
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
        public async Task<IActionResult> GetXaByTinh(short maTinh)
        {
            var xaList = await _xaService.GetByIDTinh(maTinh);
            return Json(xaList.Select(x => new { MaXa = x.MaXa, TenXa = x.TenXa }));
        }

		[HttpGet]
		public IActionResult Logout()
		{
			if (HttpContext.Session.GetString("IsLoggedIn") != "true")
			{
				return RedirectToAction("Index");
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult LogoutConfirmed()
		{
			HttpContext.Session.Clear();

			TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công!";

			return RedirectToAction("Index", "Home");
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
