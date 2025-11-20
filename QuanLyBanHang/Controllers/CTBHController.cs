
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class CTBHController : Controller
	{
		private readonly AppDbContext _context;

		public CTBHController(AppDbContext context)
		{
			_context = context;
		}

		// ============ READ ALL ============
		public async Task<IActionResult> Index()
		{
			var list = await _context.Set<CTBHDetailDto>()
			.FromSqlRaw("EXEC CTBH_GetAll_Detail")
			.ToListAsync();

			return View(list);

		}

		// ============ DETAILS ============
		public async Task<IActionResult> Details(string maDBH, string maSP)
		{
			if (string.IsNullOrEmpty(maDBH) || string.IsNullOrEmpty(maSP))
				return NotFound();

			var parameters = new[]
			{
				new SqlParameter("@MaDBH", maDBH),
				new SqlParameter("@MaSP", maSP)
			};

			var data = await _context.Set<CTBHDetailDto>()
				.FromSqlRaw("EXEC CTBH_GetById_Detail @MaDBH, @MaSP", parameters)
				.ToListAsync();

			return View(data);
		}

		// ============ EDIT (GET) ============
		public async Task<IActionResult> Edit(string maDBH, string maSP)
		{
			if (string.IsNullOrEmpty(maDBH) || string.IsNullOrEmpty(maSP))
				return NotFound();

			var parameters = new[]
			{
				new SqlParameter("@MaDBH", maDBH),
				new SqlParameter("@MaSP", maSP)
			};

			var data = await _context.CTBH
				.FromSqlRaw("EXEC CTBH_GetById_Detail @MaDBH, @MaSP", parameters)
				.ToListAsync();

			var ctbh = data.FirstOrDefault();
			if (ctbh == null) return NotFound();

			ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP", ctbh.MaSP);
			return View(ctbh);
		}

		// ============ EDIT (POST) ============
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(CTBH model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var parameters = new[]
					{
						new SqlParameter("@MaDBH", model.MaDBH),
						new SqlParameter("@MaSP", model.MaSP),
						new SqlParameter("@SLB", model.SLB),
						new SqlParameter("@DGB", model.DGB)
					};

					await _context.Database.ExecuteSqlRawAsync(
						"EXEC CTBH_Update @MaDBH, @MaSP, @SLB, @DGB", parameters);

					TempData["SuccessMessage"] = "Cập nhật chi tiết bán hàng thành công!";
					return RedirectToAction("CTBH_Admin");
				}
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP", model.MaSP);
			return View(model);
		}

		// ============ DELETE (GET) ============
		public async Task<IActionResult> Delete(string maDBH, string maSP)
		{
			if (string.IsNullOrEmpty(maDBH) || string.IsNullOrEmpty(maSP))
				return NotFound();

			var parameters = new[]
			{
				new SqlParameter("@MaDBH", maDBH),
				new SqlParameter("@MaSP", maSP)
			};

			var data = await _context.CTBH
				.FromSqlRaw("EXEC CTBH_GetById_Detail @MaDBH, @MaSP", parameters)
				.ToListAsync();

			var ctbh = data.FirstOrDefault();
			if (ctbh == null) return NotFound();

			return View(ctbh);
		}

		// ============ DELETE (POST) ============
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string maDBH, string maSP)
		{
			try
			{
				var parameters = new[]
				{
					new SqlParameter("@MaDBH", maDBH),
					new SqlParameter("@MaSP", maSP)
				};

				await _context.Database.ExecuteSqlRawAsync(
					"EXEC CTBH_Delete @MaDBH, @MaSP", parameters);

				TempData["SuccessMessage"] = "Xóa chi tiết bán hàng thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction("CTBH_Admin");
		}
	}
}
