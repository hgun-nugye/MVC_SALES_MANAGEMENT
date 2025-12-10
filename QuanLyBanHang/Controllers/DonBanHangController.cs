using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

		//public async Task<IActionResult> Create()
		//{
		//	ViewBag.MaKH = new SelectList(_context.KhachHang, "MaKH", "TenKH");
		//	ViewBag.MaSP = new SelectList(await _spService.GetAll(), "MaSP", "TenSP");

		//	var model = new DonBanHang
		//	{
		//		NgayBH = DateTime.Today,
		//		CTBHs = new List<CTBH>
		//		{
		//			new CTBH()
		//		}
		//	};

		//	return View(model);
		//}

		public IActionResult Create()
		{
			var sp = _context.SanPham
				.Select(x => new SelectListItem
				{
					Value = x.MaSP ?? "",
					Text = x.TenSP ?? ""
				})
				.ToList();

			ViewBag.MaSP = sp ?? new List<SelectListItem>();

			var kh = _context.KhachHang
				.Select(x => new SelectListItem
				{
					Value = x.MaKH ?? "",
					Text = x.TenKH ?? ""
				})
				.ToList();

			ViewBag.MaKH = kh ?? new List<SelectListItem>();

			return View(new DonBanHang());
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(DonBanHang model)
		{
			try
			{
				// XÓA DÒNG TRỐNG
				model.CTBHs = model.CTBHs
					.Where(x => !string.IsNullOrEmpty(x.MaSP))
					.ToList();

				// GÁN GIÁ TRỊ HỢP LỆ
				foreach (var ct in model.CTBHs)
				{
					ct.SLB ??= 1;
					ct.DGB ??= 0;
				}

				// Kiểm tra tồn kho
				var selectedIds = model.CTBHs
					.Select(x => x.MaSP!)
					.Distinct()
					.ToList();

				var stockLookup = selectedIds.Any()
					? await _context.SanPham
						.Where(x => selectedIds.Contains(x.MaSP))
						.ToDictionaryAsync(x => x.MaSP, x => x.SoLuongTon)
					: new Dictionary<string, int>();

				for (int i = 0; i < model.CTBHs.Count; i++)
				{
					var ct = model.CTBHs[i];
					if (!string.IsNullOrEmpty(ct.MaSP) && stockLookup.TryGetValue(ct.MaSP, out var ton))
					{
						var slb = ct.SLB ?? 0;
						if (slb > ton)
						{
							ModelState.AddModelError($"CTBHs[{i}].SLB", $"Số lượng bán ({slb}) vượt tồn kho ({ton}).");
						}
					}
				}

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
			try
			{
				// Gán giá trị mặc định
				foreach (var ct in model.ChiTiet)
				{
					ct.SLB ??= 1;
					ct.DGB ??= 0;
				}

				// Kiểm tra tồn kho
				var selectedIds = model.ChiTiet
					.Select(x => x.MaSP!)
					.Distinct()
					.ToList();

				var stockLookup = selectedIds.Any()
					? await _context.SanPham
						.Where(x => selectedIds.Contains(x.MaSP))
						.ToDictionaryAsync(x => x.MaSP, x => x.SoLuongTon)
					: new Dictionary<string, int>();

				for (int i = 0; i < model.ChiTiet.Count; i++)
				{
					var ct = model.ChiTiet[i];
					if (!string.IsNullOrEmpty(ct.MaSP) && stockLookup.TryGetValue(ct.MaSP, out var ton))
					{
						var slb = ct.SLB ?? 0;
						if (slb > ton)
						{
							ModelState.AddModelError($"ChiTiet[{i}].SLB", $"Số lượng bán ({slb}) vượt tồn kho ({ton}).");
						}
					}
				}

				if (!ModelState.IsValid)
				{
					ViewBag.MaKH = new SelectList(_context.KhachHang, "MaKH", "TenKH", model.MaKH);
					return View(model);
				}

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


