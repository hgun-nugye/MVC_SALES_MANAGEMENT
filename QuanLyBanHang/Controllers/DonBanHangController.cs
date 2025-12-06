using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;
using System.Threading.Tasks;

namespace QuanLyBanHang.Controllers
{
	public class DonBanHangController : Controller
	{
		private readonly DonBanHangService _dbhService;
		private readonly CTBHService _ctbhService;
		private readonly SanPhamService _spService;
		private readonly AppDbContext _context;

		public DonBanHangController(DonBanHangService service, CTBHService ctbhService, SanPhamService spService, AppDbContext context)
		{
			_dbhService = service;
			_ctbhService = ctbhService;
			_spService = spService;
			_context = context;
		}

		public async Task<IActionResult> Index(string? search, int? month, int? year)
		{
			ViewBag.Search = search;
			ViewBag.Month = month;
			ViewBag.Year = year;

			var model = await _dbhService.Search(search, month, year);
			return View(model);
		}


		public async Task<IActionResult> Details(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var result = await _dbhService.GetByID(id);
			return View(result);
		}

		public async Task<IActionResult> Create()
		{
			ViewBag.MaKH = new SelectList(_context.KhachHang, "MaKH", "TenKH");
			ViewBag.MaSP = new SelectList(await _spService.GetAll(), "MaSP", "TenSP");

			var model = new DonBanHang
			{
				NgayBH = DateTime.Today,
				CTBHs = new List<CTBH>
				{
					new CTBH()
				}
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(DonBanHang model)
		{
			try
			{
				if (ModelState.IsValid && model.CTBHs != null && model.CTBHs.Any())
				{
					await _dbhService.Create(model);

					TempData["SuccessMessage"] = $"Thêm đơn bán hàng thành công!";
					return RedirectToAction(nameof(Index));
				}

				TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin đơn hàng và chi tiết sản phẩm.";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi khi thêm đơn bán hàng: " + ex.Message;
			}

			ViewBag.MaKH = new SelectList(_context.KhachHang, "MaKH", "TenKH", model.MaKH);
			ViewBag.MaSP = new SelectList(await _spService.GetAll(), "MaSP", "TenSP");
			return View(model);
		}
		public async Task<IActionResult> Edit(string id)
		{
			if (id == null) return NotFound();

			var rows = await _dbhService.GetByID(id);
			if (rows == null || !rows.Any()) return NotFound();

			// Tách header từ dòng đầu tiên
			var header = rows.First();

			// Lấy chi tiết
			var details = rows.Select(x => new CTBH
			{
				MaDBH = x.MaDBH!,
				MaSP = x.MaSP!,
				SLB = x.SLB ?? 0,
				DGB = x.DGB ?? 0,
				TenSP = x.TenSP
			}).ToList();

			var ct = new DonBanHangEditCTBH
			{
				MaDBH = header.MaDBH!,
				NgayBH = header.NgayBH,
				MaKH = header.MaKH!,
				ChiTiet = details
			};

			ViewBag.MaKH = new SelectList(_context.KhachHang, "MaKH", "TenKH", ct.MaKH);

			return View(ct);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(DonBanHangEditCTBH model)
		{
			if (!ModelState.IsValid) return View(model);
			try
			{

				await _dbhService.Update(model);

				TempData["SuccessMessage"] = "Cập nhật đơn bán hàng thành công!";
				return RedirectToAction(nameof(Details), new { id = model.MaDBH });
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			ViewBag.MaKH = new SelectList(_context.KhachHang, "MaKH", "TenKH", model.MaKH);
			return View(model);

		}

		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();


			var dbh = await _dbhService.GetByID(id);
			if (dbh == null || !dbh.Any()) return NotFound();

			return View(dbh);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				await _dbhService.Delete(id);
				TempData["SuccessMessage"] = "Xóa đơn bán hàng thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> DeleteDetail(string maDBH, string maSP)
		{
			var model = await _dbhService.GetDetail(maDBH, maSP);
			if (model == null) return NotFound();
			return View(model);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteDetailConfirmed(string maDBH, string maSP)
		{
			try
			{
				await _dbhService.DeleteDetail(maDBH, maSP);

				TempData["SuccessMessage"] = "Xóa chi tiết sản phẩm thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi khi xóa: " + ex.Message;
			}

			return RedirectToAction("Details", new { id = maDBH });
		}

	}
}


