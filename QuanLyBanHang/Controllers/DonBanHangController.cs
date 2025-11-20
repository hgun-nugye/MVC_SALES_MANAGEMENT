using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using QuanLyBanHang.Services;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.Controllers
{
	public class DonBanHangController : Controller
	{
		private readonly AppDbContext _context;

		public DonBanHangController(AppDbContext context)
		{
			_context = context;
		}

		
		// ============ READ ============
		public async Task<IActionResult> Index()
		{
			var data = await _context.DonBanHang
				.FromSqlRaw("EXEC DonBanHang_GetAll")
				.ToListAsync();

			return View(data);
		}

		// ============ DETAILS ============
		public async Task<IActionResult> Details(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var param = new SqlParameter("@MaDBH", id);
			var result = await _context.DonBanHangDetail
				.FromSqlRaw("EXEC DonBanHang_GetById_Detail @MaDBH", param)
				.ToListAsync();
			
			return View(result);
		}

		// ===================== CREATE (GET) =====================
		public IActionResult Create()
		{
			ViewBag.MaKH = new SelectList(_context.KhachHang, "MaKH", "TenKH");
			ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP");

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

		// ===================== CREATE (POST) =====================
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(DonBanHang model)
		{
			try
			{
				if (ModelState.IsValid && model.CTBHs != null && model.CTBHs.Any())
				{
					var table = new DataTable();
					table.Columns.Add("MaSP", typeof(string));
					table.Columns.Add("SLB", typeof(int));
					table.Columns.Add("DGB", typeof(decimal));

					foreach (var ct in model.CTBHs)
					{
						table.Rows.Add(ct.MaSP, ct.SLB, ct.DGB);
					}

					var parameters = new[]
					{
						new SqlParameter("@NgayBH", model.NgayBH),
						new SqlParameter("@MaKH", model.MaKH),
						new SqlParameter("@ChiTiet", table)
						{
							SqlDbType = SqlDbType.Structured,
							TypeName = "dbo.CTBH_List"
						}
					};
					await _context.Database.ExecuteSqlRawAsync("EXEC DonBanHang_Insert @NgayBH, @MaKH, @ChiTiet", parameters);

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
			ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP");
			return View(model);
		}

		// ============ EDIT (GET) ============
		public async Task<IActionResult> Edit(string id)
		{
			if (id == null) return NotFound();

			var param = new SqlParameter("@MaDBH", id);
			var data = await _context.Set<DonBanHangDetail>()
				.FromSqlRaw("EXEC DonBanHang_GetById_Detail @MaDBH", param)
				.ToListAsync();

			if (!data.Any()) return NotFound();

			var ct = new DonBanHangEditCTBH
			{
				MaDBH = data[0].MaDBH!,
				NgayBH = data[0].NgayBH,
				MaKH = data[0].MaKH!,
				ChiTiet = data
			};

			ViewBag.MaKH = new SelectList(_context.KhachHang, "MaKH", "TenKH", ct.MaKH);

			return View(ct);
		}
				
		// ============ EDIT (POST) ============
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(DonBanHangEditCTBH model)
		{
			if (!ModelState.IsValid) return View(model);
			try
			{

				// Update header
				await _context.Database.ExecuteSqlRawAsync(
					"EXEC DonBanHang_Update  @MaDBH,@NgayBH, @MaKH",
					new SqlParameter("@MaDBH", model.MaDBH),
					new SqlParameter("@NgayBH", model.NgayBH),
					new SqlParameter("@MaKH", model.MaKH)
				);

				// Update each product
				foreach (var ct in model.ChiTiet)
				{
					await _context.Database.ExecuteSqlRawAsync(
						"EXEC CTBH_Update @MaDBH, @MaSP, @SLB, @DGB",
						new SqlParameter("@MaDBH", model.MaDBH),
						new SqlParameter("@MaSP", ct.MaSP),
						new SqlParameter("@SLB", ct.SLB),
						new SqlParameter("@DGB", ct.DGB)
					);
				}

				TempData["SuccessMessage"] = "Cập nhật đơn bán hàng thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			ViewBag.MaKH = new SelectList(_context.KhachHang, "MaKH", "TenKH", model.MaKH);
			return View(model);

		}
				
		// ============ DELETE (GET) ============
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var param = new SqlParameter("@MaDBH", id);
			var data = await _context.DonBanHang
				.FromSqlRaw("EXEC DonBanHang_GetById_Detail @MaDBH", param)
				.ToListAsync();

			var dbh = data.FirstOrDefault();
			if (dbh == null) return NotFound();

			return View(dbh);
		}

		// ============ DELETE (POST) ============
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				var param = new SqlParameter("@MaDBH", id);
				await _context.Database.ExecuteSqlRawAsync("EXEC DonBanHang_Delete @MaDBH", param);

				TempData["SuccessMessage"] = "Xóa đơn bán hàng thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}
	}
}


