using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class DonMuaHangController : Controller
	{
		private readonly DonMuaHangService _dmhService;
		private readonly CTMHService _ctmhService;
		private readonly SanPhamService _spService;
		private readonly NhaCCService _nhaCCService;
		private readonly NhanVienService _nhanVienService;
		private readonly AppDbContext _context;

		public DonMuaHangController(
			DonMuaHangService service,
			CTMHService ctmhService,
			SanPhamService spService,
			NhaCCService nhaCCService,
			NhanVienService nhanVienService,
			AppDbContext context)
		{
			_dmhService = service;
			_ctmhService = ctmhService;
			_spService = spService;
			_nhaCCService = nhaCCService;
			_nhanVienService = nhanVienService;
			_context = context;
		}

		public async Task<IActionResult> Index(string? search, int? month, int? year)
		{
			ViewBag.Search = search;
			ViewBag.Month = month;
			ViewBag.Year = year;

			var model = await _dmhService.Search(search, month, year);

			return View(model);
		}

		public async Task<IActionResult> Details(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var result = await _dmhService.GetByID(id);
			return View(result);
		}

		public async Task<IActionResult> Create()
		{
			// Sử dụng service thay vì query trực tiếp từ context để tránh shadow property
			var nhaCCList = await _nhaCCService.GetAll();
			var nhanVienList = await _nhanVienService.GetAll();
			var sanPhamList = await _spService.GetAll();

			ViewBag.MaNCC = new SelectList(_context.NhaCC.ToList(), "MaNCC", "TenNCC");
			ViewBag.MaNV = new SelectList(_context.NhanVien.ToList(), "MaNV", "TenNV");
			ViewBag.MaSP = new SelectList(_context.SanPham.ToList(), "MaSP", "TenSP");

			var model = new DonMuaHang
			{
				NgayMH = DateTime.Today,
				CTMHs = new List<CTMH> { new CTMH() }
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(DonMuaHang model)
		{
			// Bỏ qua validate MaDMH vì sẽ được sinh ở tầng DB / SP
			ModelState.Remove("MaDMH");
			if (model.CTMHs != null)
			{
				for (int i = 0; i < model.CTMHs.Count; i++)
				{
					ModelState.Remove($"CTMHs[{i}].MaDMH");
				}
			}

			if (!ModelState.IsValid || model.CTMHs == null || !model.CTMHs.Any())
			{
				TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin đơn mua hàng và chi tiết sản phẩm.";
				// Đảm bảo có ít nhất một dòng chi tiết để view binding không lỗi
				model.CTMHs ??= new List<CTMH>();
				if (!model.CTMHs.Any())
					model.CTMHs.Add(new CTMH());

				await LoadDropdowns(model);
				return View(model);
			}

			try
			{
				await _dmhService.Create(model);
				TempData["SuccessMessage"] = "Thêm đơn mua hàng thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			await LoadDropdowns(model);
			return View(model);
		}

		public async Task<IActionResult> Edit(string id)
		{
			if (id == null) return NotFound();

			var rows = await _dmhService.GetByID(id);
			if (rows == null || !rows.Any()) return NotFound();

			// Tách header từ dòng đầu tiên
			var header = rows.First();

			// Lấy chi tiết
			var details = rows.Select(x => new CTMH
			{
				MaDMH = x.MaDMH!,
				MaSP = x.MaSP!,
				SLM = x.SLM ?? 0,
				DGM = x.DGM ?? 0,
				TenSP = x.TenSP
			}).ToList();

			var ct = new DonMuaHangEditCTMH
			{
				MaDMH = header.MaDMH!,
				NgayMH = header.NgayMH,
				MaNCC = header.MaNCC!,
				MaNV = header.MaNV!,
				ChiTiet = details
			};

			var nhaCCList = await _nhaCCService.GetAll();
			var nhanVienList = await _nhanVienService.GetAll();

			ViewBag.MaNCC = new SelectList(nhaCCList, "MaNCC", "TenNCC", ct.MaNCC);
			ViewBag.MaNV = new SelectList(nhanVienList, "MaNV", "TenNV", ct.MaNV);
			return View(ct);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(DonMuaHangEditCTMH model)
		{
			if (!ModelState.IsValid) return View(model);
			try
			{

				await _dmhService.Update(model);

				TempData["SuccessMessage"] = "Cập nhật đơn mua hàng thành công!";
				return RedirectToAction(nameof(Details), new { id = model.MaDMH });
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			var nhaCCList = await _nhaCCService.GetAll();
			var nhanVienList = await _nhanVienService.GetAll();

			ViewBag.MaNCC = new SelectList(nhaCCList, "MaNCC", "TenNCC", model.MaNCC);
			ViewBag.MaNV = new SelectList(nhanVienList, "MaNV", "TenNV", model.MaNV);
			return View(model);

		}

		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var data = await _dmhService.GetByID(id);
			if (data == null || !data.Any()) return NotFound();

			return View(data);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				await _dmhService.Delete(id);
				TempData["SuccessMessage"] = "Xóa đơn mua hàng thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}


		[HttpGet]
		public async Task<IActionResult> DeleteDetail(string MaDMH, string maSP)
		{
			var model = await _dmhService.GetDetail(MaDMH, maSP);
			if (model == null) return NotFound();
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteDetailConfirmed(string MaDMH, string maSP)
		{
			try
			{
				await _dmhService.DeleteDetail(MaDMH, maSP);
				TempData["SuccessMessage"] = "Xóa chi tiết sản phẩm thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction("Details", new { id = MaDMH });
		}


		[HttpGet]
		public async Task<IActionResult> Search(string? search, int? month, int? year)
		{
			var data = await _dmhService.Search(search, month, year);
			return PartialView("DonMuaHangTable", data);
		}

		public async Task<IActionResult> Clear()
		{
			var data = await _dmhService.Reset();
			return PartialView("DonMuaHangTable", data);
		}

		private async Task LoadDropdowns(DonMuaHang model)
		{
			// Đảm bảo danh sách chi tiết luôn có ít nhất 1 phần tử để view không bị null/index lỗi
			model.CTMHs ??= new List<CTMH>();
			if (!model.CTMHs.Any())
				model.CTMHs.Add(new CTMH());

			var nhaCCList = await _nhaCCService.GetAll();
			var nhanVienList = await _nhanVienService.GetAll();
			var sanPhamList = await _spService.GetAll();

			ViewBag.MaNCC = new SelectList(nhaCCList, "MaNCC", "TenNCC", model.MaNCC);
			ViewBag.MaNV = new SelectList(nhanVienList, "MaNV", "TenNV", model.MaNV);
			ViewBag.MaSP = new SelectList(sanPhamList, "MaSP", "TenSP");
			//ViewBag.MaSP = new SelectList(_context.SanPham.ToList(), "MaSP", "TenSP");

			// Gán dropdown cho từng dòng CTMH
			for (int i = 0; i < model.CTMHs.Count; i++)
			{
				ViewData[$"MaSP_{i}"] = new SelectList(
					sanPhamList,
					"MaSP",
					"TenSP",
					model.CTMHs[i].MaSP);
			}
		}

	}
}
