using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using QuanLyBanHang.Services;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.Controllers
{
	public class DonMuaHangController : Controller
	{
		private readonly AppDbContext _context;

		public DonMuaHangController(AppDbContext context)
		{
			_context = context;
		}

		// ============ INDEX / READ ============
		public async Task<IActionResult> Index(int? month, int? year)
		{
			ViewBag.Month = month;
			ViewBag.Year = year;

			var parameters = new[]
			{
				new SqlParameter("@Month", month ?? (object)DBNull.Value),
				new SqlParameter("@Year", year ?? (object)DBNull.Value)
			};

			var data = await _context.DonMuaHang
				.FromSqlRaw("EXEC DonMuaHang_Filter @Month, @Year", parameters)
				.ToListAsync();

			return View(data);
		}

		// ============ DETAILS ============
		public async Task<IActionResult> Details(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var param = new SqlParameter("@MaDMH", id);
			var result = await _context.DonMuaHangDetail
				.FromSqlRaw("EXEC DonMuaHang_GetById_Detail @MaDMH", param)
				.ToListAsync();

			return View(result);
		}

		// ============ CREATE (GET) ============
		public IActionResult Create()
		{
			ViewBag.MaNCC = new SelectList(_context.NhaCC, "MaNCC", "TenNCC");
			ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP");

			var model = new DonMuaHang
			{
				NgayMH = DateTime.Today,
				CTMHs = new List<CTMH> { new CTMH() }
			};

			return View(model);
		}

		// ============ CREATE (POST) ============
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(DonMuaHang model)
		{
			try
			{
				if (ModelState.IsValid && model.CTMHs != null && model.CTMHs.Any())
				{
					var table = new DataTable();
					table.Columns.Add("MaSP", typeof(string));
					table.Columns.Add("SLM", typeof(int));
					table.Columns.Add("DGM", typeof(decimal));

					foreach (var ct in model.CTMHs)
						table.Rows.Add(ct.MaSP, ct.SLM, ct.DGM);

					var parameters = new[]
					{
						new SqlParameter("@NgayMH", model.NgayMH),
						new SqlParameter("@MaNCC", model.MaNCC),
						new SqlParameter("@ChiTiet", table)
						{
							SqlDbType = SqlDbType.Structured,
							TypeName = "dbo.CTMH_List"
						}
					};

					await _context.Database.ExecuteSqlRawAsync(
						"EXEC DonMuaHang_Insert @NgayMH, @MaNCC, @ChiTiet", parameters
					);

					TempData["SuccessMessage"] = "Thêm đơn mua hàng thành công!";
					return RedirectToAction(nameof(Index));
				}

				TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin đơn hàng và chi tiết sản phẩm.";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi khi thêm đơn mua hàng: " + ex.Message;
			}

			ViewBag.MaNCC = new SelectList(_context.NhaCC, "MaNCC", "TenNCC", model.MaNCC);
			ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP");
			return View(model);
		}

		// ============ EDIT (GET) ============
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var param = new SqlParameter("@MaDMH", id);
			var data = await _context.Set<DonMuaHangDetail>()
				.FromSqlRaw("EXEC DonMuaHang_GetById_Detail @MaDMH", param)
				.ToListAsync();

			if (!data.Any()) return NotFound();

			var ct = new DonMuaHangEditCTMH
			{
				MaDMH = data[0].MaDMH!,
				NgayMH = data[0].NgayMH,
				MaNCC = data[0].MaNCC!,
				ChiTiet = data
			};

			ViewBag.MaNCC = new SelectList(_context.NhaCC, "MaNCC", "TenNCC", ct.MaNCC);
			return View(ct);
		}

		// ============ EDIT (POST) ============
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(DonMuaHangEditCTMH model)
		{
			if (!ModelState.IsValid) return View(model);

			try
			{
				// Update header
				await _context.Database.ExecuteSqlRawAsync(
					"EXEC DonMuaHang_Update @MaDMH, @NgayMH, @MaNCC",
					new SqlParameter("@MaDMH", model.MaDMH),
					new SqlParameter("@NgayMH", model.NgayMH),
					new SqlParameter("@MaNCC", model.MaNCC)
				);

				// Update each product
				foreach (var ct in model.ChiTiet)
				{
					await _context.Database.ExecuteSqlRawAsync(
						"EXEC CTMH_Update @MaDMH, @MaSP, @SLM, @DGM",
						new SqlParameter("@MaDMH", model.MaDMH),
						new SqlParameter("@MaSP", ct.MaSP),
						new SqlParameter("@SLM", ct.SLM),
						new SqlParameter("@DGM", ct.DGM)
					);
				}

				TempData["SuccessMessage"] = "Cập nhật đơn mua hàng thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			ViewBag.MaNCC = new SelectList(_context.NhaCC, "MaNCC", "TenNCC", model.MaNCC);
			return View(model);
		}

		// ============ DELETE (GET) ============
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var param = new SqlParameter("@MaDMH", id);
			var data = await _context.DonMuaHang
				.FromSqlRaw("EXEC DonMuaHang_GetById_Detail @MaDMH", param)
				.ToListAsync();

			var dmh = data.FirstOrDefault();
			if (dmh == null) return NotFound();

			return View(dmh);
		}

		// ============ DELETE (POST) ============
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				var param = new SqlParameter("@MaDMH", id);
				await _context.Database.ExecuteSqlRawAsync("EXEC DonMuaHang_Delete @MaDMH", param);

				TempData["SuccessMessage"] = "Xóa đơn mua hàng thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}

		// ============ DELETE DETAIL (GET) ============
		[HttpGet]
		public async Task<IActionResult> DeleteDetail(string MaDMH, string maSP)
		{
			if (string.IsNullOrEmpty(MaDMH) || string.IsNullOrEmpty(maSP))
				return NotFound();

			var parameters = new[]
			{
				new SqlParameter("@MaDMH", MaDMH),
				new SqlParameter("@MaSP", maSP)
			};

			var data = await _context.CTMHDetailDtos
				.FromSqlRaw("EXEC CTMH_GetById_Detail @MaDMH, @MaSP", parameters)
				.ToListAsync();

			var model = data.FirstOrDefault();
			if (model == null) return NotFound();

			var ctmh = new CTMH
			{
				MaDMH = model.MaDMH,
				MaSP = model.MaSP,
				SLM = model.SLM,
				DGM = model.DGM,
				TenSP = model.TenSP
			};

			return View("DeleteDetail", ctmh);
		}

		// ============ DELETE DETAIL (POST) ============
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteDetailConfirmed(string MaDMH, string maSP)
		{
			try
			{
				await _context.Database.ExecuteSqlRawAsync(
					"EXEC CTMH_Delete @MaDMH, @MaSP",
					new SqlParameter("@MaDMH", MaDMH),
					new SqlParameter("@MaSP", maSP)
				);

				TempData["SuccessMessage"] = "Xóa chi tiết sản phẩm thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi khi xóa: " + ex.Message;
			}

			return RedirectToAction("Details", new { id = MaDMH });
		}

		// ============ SEARCH ============
		[HttpGet]
		public async Task<IActionResult> Search(string keyword)
		{
			var param = new SqlParameter("@Search", keyword ?? (object)DBNull.Value);

			var data = await _context.DonMuaHang
				.FromSqlRaw("EXEC DonMuaHang_Search @Search", param)
				.ToListAsync();

			return PartialView("DonMuaHangTable", data);
		}
	}
}
