using Microsoft.AspNetCore.Mvc;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace QuanLyBanHang.Controllers
{
    public class CartController : Controller
    {
        private readonly SanPhamService _spService;
        private readonly DonBanHangService _dbhService;
        private readonly AppDbContext _context;
        private readonly KhachHangService _khService;
        private readonly TinhService _tinhService;
        private const string CART_SESSION_KEY = "UserCart";

        public CartController(
            SanPhamService spService, 
            DonBanHangService dbhService, 
            AppDbContext context, 
            KhachHangService khService,
            TinhService tinhService)
        {
            _spService = spService;
            _dbhService = dbhService;
            _context = context;
            _khService = khService;
            _tinhService = tinhService;
        }

        private string? GetUserId() => HttpContext.Session.GetString("UserId");

		private List<CartItem> GetCartFromSession()
		{
			var json = HttpContext.Session.GetString(CART_SESSION_KEY);
			return string.IsNullOrEmpty(json)
				? new List<CartItem>()
				: JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
		}

		private void SaveCartToSession(List<CartItem> cart)
		{
			var json = JsonSerializer.Serialize(cart);
			HttpContext.Session.SetString(CART_SESSION_KEY, json);
		}

        public async Task<IActionResult> Index()
        {
            var cart = GetCartFromSession();
            // Cập nhật số lượng tồn mới nhất từ DB
            foreach (var item in cart)
            {
                var sp = await _spService.GetById(item.MaSP);
                if (sp != null)
                {
                    item.SoLuongTon = sp.SoLuongTon ?? 0;
                }
            }
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(string maSP, int soLuong = 1)
        {
            var sp = await _spService.GetById(maSP);
            if (sp == null) return NotFound();

            if (sp.MaTT == "TT4" || sp.TenTT == "Ngưng bán")
            {
                TempData["ErrorMessage"] = "Sản phẩm này đã ngừng kinh doanh.";
                return RedirectToAction("Details", "SanPham", new { id = maSP });
            }

            var cart = GetCartFromSession();
			var item = cart.FirstOrDefault(c => c.MaSP == maSP);
            
            int currentQtyInCart = item?.SoLuong ?? 0;
            if (currentQtyInCart + soLuong > (sp.SoLuongTon ?? 0))
            {
                TempData["ErrorMessage"] = $"Không đủ hàng tồn kho. Hiện còn: {sp.SoLuongTon}";
                return RedirectToAction("Details", "SanPham", new { id = maSP });
            }

			if (item != null)
			{
				item.SoLuong += soLuong;
			}
			else
			{
				cart.Add(new CartItem
				{
					MaSP = sp.MaSP,
					TenSP = sp.TenSP,
					AnhMH = sp.AnhMH,
					GiaBan = sp.GiaBan ?? 0,
					SoLuong = soLuong,
                    SoLuongTon = sp.SoLuongTon ?? 0,
					IsSelected = true
				});
			}

			SaveCartToSession(cart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(string maSP, int soLuong)
        {
            var sp = await _spService.GetById(maSP);
            if (sp == null) return NotFound();

            if (soLuong > (sp.SoLuongTon ?? 0)) {
                TempData["ErrorMessage"] = $"Số lượng vượt quá tồn kho (Còn: {sp.SoLuongTon})";
                return RedirectToAction(nameof(Index));
            }

			var cart = GetCartFromSession();
			var item = cart.FirstOrDefault(c => c.MaSP == maSP);
			if (item != null)
			{
				item.SoLuong = soLuong;
                item.SoLuongTon = sp.SoLuongTon ?? 0;
				if (item.SoLuong <= 0) cart.Remove(item);
				SaveCartToSession(cart);
			}
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult RemoveFromCart(string maSP)
        {
			var cart = GetCartFromSession();
			var item = cart.FirstOrDefault(c => c.MaSP == maSP);
			if (item != null)
			{
				cart.Remove(item);
				SaveCartToSession(cart);
			}
            
            TempData["SuccessMessage"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ToggleSelect(string maSP)
        {
			var cart = GetCartFromSession();
			var item = cart.FirstOrDefault(c => c.MaSP == maSP);
			if (item != null)
			{
				item.IsSelected = !item.IsSelected;
				SaveCartToSession(cart);
			}
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ToggleSelectAll(bool selectAll)
        {
			var cart = GetCartFromSession();
			foreach (var item in cart)
			{
				item.IsSelected = selectAll;
			}
			SaveCartToSession(cart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ClearSelected()
        {
			var cart = GetCartFromSession();
			cart.RemoveAll(c => c.IsSelected);
			SaveCartToSession(cart);
            
            TempData["SuccessMessage"] = "Đã xóa các mục được chọn.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Home");

            var cart = GetCartFromSession();
            var selectedItems = cart.Where(i => i.IsSelected).ToList();
            
            if (!selectedItems.Any()) 
            {
                TempData["ErrorMessage"] = "Bạn chưa chọn sản phẩm nào để thanh toán.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra tồn kho một lần nữa trước khi vào trang checkout
            foreach (var item in selectedItems) {
                var sp = await _spService.GetById(item.MaSP);
                if (sp == null || item.SoLuong > (sp.SoLuongTon ?? 0)) {
                    TempData["ErrorMessage"] = $"Sản phẩm {item.TenSP} không đủ số lượng tồn kho (Còn {sp?.SoLuongTon ?? 0}). Vui lòng điều chỉnh lại.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var khDetail = await _khService.GetByIDWithXa(userId);
            if (khDetail != null)
            {
                var kh = new KhachHang {
                    MaKH = khDetail.MaKH ?? "",
                    TenKH = khDetail.TenKH ?? "",
                    DienThoaiKH = khDetail.DienThoaiKH ?? "",
                    DiaChiKH = khDetail.DiaChiKH,
                    TenXa = khDetail.TenXa,
                    TenTinh = khDetail.TenTinh,
                    MaXa = khDetail.MaXa ?? ""
                };
                ViewBag.Customer = kh;
            }
            
            ViewBag.TinhList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _tinhService.GetAll(), "MaTinh", "TenTinh");

            return View(selectedItems); // View uses List<CartItem> model
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string diaChiGiaoHang, string maXa)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Home");

            var cart = GetCartFromSession();
            var selectedItems = cart.Where(i => i.IsSelected).ToList();

            if (!selectedItems.Any()) return RedirectToAction(nameof(Index));

            // Kiểm tra tồn kho lần cuối cùng trước khi lưu vào DB
            foreach (var item in selectedItems) {
                var sp = await _spService.GetById(item.MaSP);
                if (sp == null || item.SoLuong > (sp.SoLuongTon ?? 0)) {
                    TempData["ErrorMessage"] = $"Sản phẩm {item.TenSP} vừa mới hết hàng hoặc không đủ số lượng. Vui lòng kiểm tra lại.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var donHang = new DonBanHang
            {
                NgayBH = DateTime.Now,
                MaKH = userId,
                DiaChiDBH = diaChiGiaoHang,
                MaXa = maXa,
                MaTTBH = "CHO", // Chờ xác nhận
                CTBHs = selectedItems.Select(i => new CTBH
                {
                    MaSP = i.MaSP,
                    SLB = i.SoLuong,
                    DGB = i.GiaBan
                }).ToList()
            };

            var success = await _dbhService.Create(donHang);
            if (success)
            {
                // Xóa các mục đã select trong Session
                cart.RemoveAll(c => c.IsSelected);
				SaveCartToSession(cart);
                
                TempData["SuccessMessage"] = "Bạn đã đặt hàng thành công! Đơn hàng đang chờ xử lý.";
                return RedirectToAction("Index", "DonBanHang"); 
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra khi đặt hàng. Vui lòng thử lại.";
            return RedirectToAction(nameof(Checkout));
        }
    }
}
