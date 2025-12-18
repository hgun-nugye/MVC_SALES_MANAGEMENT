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

        public HomeController(
            ILogger<HomeController> logger,
            SanPhamService sanPhamService,
            KhachHangService khachHangService,
            NhanVienService nhanVienService,
            XaService xaService,
            TinhService tinhService,
            AppDbContext context)
        {
            _logger = logger;
            _sanPhamService = sanPhamService;
            _khachHangService = khachHangService;
            _nhanVienService = nhanVienService;
            _xaService = xaService;
            _tinhService = tinhService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách sản phẩm để hiển thị dạng card
            var sanPhams = await _sanPhamService.GetAll();
            return View(sanPhams);
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Nếu đã đăng nhập, chuyển về trang chủ
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
            // Load dropdown cho Xã và Tỉnh
            var tinhList = await _tinhService.GetAll();
            ViewBag.MaTinh = new SelectList(tinhList, "MaTinh", "TenTinh");
            ViewBag.MaXa = new SelectList(new List<Xa>(), "MaXa", "TenXa");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(KhachHang khachHang, IFormFile? AnhFile)
        {
            // Bỏ qua validation cho MaKH vì nó được tự động generate
            ModelState.Remove("MaKH");
            ModelState.Remove("AnhFile");

            // Validate file upload
            if (AnhFile == null || AnhFile.Length == 0)
            {
                ModelState.AddModelError("AnhFile", "Vui lòng chọn ảnh đại diện!");
            }
            else if (AnhFile.Length > 5 * 1024 * 1024) // 5MB
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

            // Kiểm tra tên đăng nhập đã tồn tại chưa
            if (!string.IsNullOrEmpty(khachHang.TenDNKH))
            {
                var existingKH = await _context.KhachHang
                    .FirstOrDefaultAsync(kh => kh.TenDNKH == khachHang.TenDNKH);
                if (existingKH != null)
                {
                    ModelState.AddModelError("TenDNKH", "Tên đăng nhập đã tồn tại!");
                }
            }

            if (!ModelState.IsValid)
            {
                // Load lại dropdown
                var tinhList = await _tinhService.GetAll();
                ViewBag.MaTinh = new SelectList(tinhList, "MaTinh", "TenTinh", khachHang.MaXa);
                
                // Lấy mã tỉnh từ Xã nếu có
                short maTinh = 0;
                if (khachHang.MaXa.HasValue)
                {
                    maTinh = await _xaService.GetByIDWithTinh(khachHang.MaXa.Value);
                }
                
                if (maTinh > 0)
                {
                    var xaList = await _xaService.GetByIDTinh(maTinh);
                    ViewBag.MaXa = new SelectList(xaList, "MaXa", "TenXa", khachHang.MaXa);
                }
                else
                {
                    ViewBag.MaXa = new SelectList(new List<Xa>(), "MaXa", "TenXa");
                }

                return View(khachHang);
            }

            try
            {
                if (AnhFile == null || AnhFile.Length == 0)
                {
                    ModelState.AddModelError("AnhFile", "Vui lòng chọn ảnh đại diện!");
                    var tinhList = await _tinhService.GetAll();
                    ViewBag.MaTinh = new SelectList(tinhList, "MaTinh", "TenTinh");
                    ViewBag.MaXa = new SelectList(new List<Xa>(), "MaXa", "TenXa");
                    return View(khachHang);
                }

                await _khachHangService.Create(khachHang, AnhFile);
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi đăng ký: " + ex.Message;
                var tinhList = await _tinhService.GetAll();
                ViewBag.MaTinh = new SelectList(tinhList, "MaTinh", "TenTinh");
                ViewBag.MaXa = new SelectList(new List<Xa>(), "MaXa", "TenXa");
                return View(khachHang);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetXaByTinh(short maTinh)
        {
            var xaList = await _xaService.GetByIDTinh(maTinh);
            return Json(xaList.Select(x => new { MaXa = x.MaXa, TenXa = x.TenXa }));
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Đăng xuất thành công!";
            return RedirectToAction("Index");
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
