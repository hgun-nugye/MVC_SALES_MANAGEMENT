using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using System.Data;

namespace QuanLyBanHang.Services
{
	public class DonMuaHangService
	{
		private readonly AppDbContext _context;

		public DonMuaHangService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<DonMuaHang>> GetAll()
		{
			return await _context.DonMuaHang
				.FromSqlRaw("EXEC DonMuaHang_GetAll")
				.ToListAsync();
		}


		// Lấy chi tiết 1 chi tiết sản phẩm
		public async Task<CTMH?> GetDetail(string MaDMH, string MaSP)
		{
			var parameters = new[]
			{
				new SqlParameter("@MaDMH", MaDMH),
				new SqlParameter("@MaSP", MaSP),
			};

			var data = await _context.CTMHDetailDtos
				.FromSqlRaw("EXEC CTMH_GetById_Detail @MaDMH, @MaSP", parameters)
				.ToListAsync();

			var model = data.FirstOrDefault();
			if (model == null) return null;

			return new CTMH
			{
				MaDMH = model.MaDMH,
				MaSP = model.MaSP,
				SLM = model.SLM,
				DGM = model.DGM,
				TenSP = model.TenSP
			};
		}

		public async Task<List<DonMuaHangDetail>?> GetByID(string id)
		{
			var param = new SqlParameter("@MaDMH", id);
			var result = await _context.DonMuaHangDetail
				.FromSqlRaw("EXEC DonMuaHang_GetByID @MaDMH", param)
				.ToListAsync();

			return result;
		}

		public async Task Create(DonMuaHang model)
		{
			var table = new DataTable();
			table.Columns.Add("MaSP", typeof(string));
			table.Columns.Add("SLM", typeof(int));
			table.Columns.Add("DGM", typeof(decimal));

			foreach (var ct in model.CTMHs!)
				table.Rows.Add(ct.MaSP, ct.SLM, ct.DGM);

			var parameters = new[]
			{
				new SqlParameter("@NgayMH", model.NgayMH),
				new SqlParameter("@MaNCC", model.MaNCC),
				new SqlParameter("@MaNV", model.MaNV),
				new SqlParameter("@MaTTMH", (object?)model.MaTTMH ?? "CHO"),
				new SqlParameter("@ChiTiet", table)
				{
					SqlDbType = SqlDbType.Structured,
					TypeName = "dbo.CTMH_List"
				}
			};

			await _context.Database.ExecuteSqlRawAsync(
				"EXEC DonMuaHang_Insert @NgayMH, @MaNCC, @MaNV, @MaTTMH, @ChiTiet", parameters
			);
		}

		public async Task Update(DonMuaHangEditCTMH model)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					// 1. Cập nhật Header
					await _context.Database.ExecuteSqlRawAsync(
						"EXEC DonMuaHang_Update @MaDMH, @NgayMH, @MaNCC, @MaNV, @MaTTMH",
						new SqlParameter("@MaDMH", model.MaDMH),
						new SqlParameter("@NgayMH", model.NgayMH),
						new SqlParameter("@MaNCC", model.MaNCC ?? (object)DBNull.Value),
						new SqlParameter("@MaNV", model.MaNV ?? (object)DBNull.Value),
						new SqlParameter("@MaTTMH", model.MaTTMH)
					);

					// 2. Xóa sạch chi tiết cũ (Sử dụng Procedure đã sửa ở Bước 1)
					await _context.Database.ExecuteSqlRawAsync(
						"EXEC CTMH_DeleteByMaDMH @MaDMH",
						new SqlParameter("@MaDMH", model.MaDMH)
					);

					// 3. Chèn lại chi tiết mới
					if (model.ChiTiet != null)
					{
						foreach (var ct in model.ChiTiet.Where(x => !string.IsNullOrEmpty(x.MaSP)))
						{
							await _context.Database.ExecuteSqlRawAsync(
								"EXEC CTMH_Insert @MaDMH, @MaSP, @SLM, @DGM",
								new SqlParameter("@MaDMH", model.MaDMH),
								new SqlParameter("@MaSP", ct.MaSP),
								new SqlParameter("@SLM", ct.SLM),
								new SqlParameter("@DGM", ct.DGM)
							);
						}
					}

					// Nếu mọi thứ ổn thì mới lưu vĩnh viễn
					await transaction.CommitAsync();
				}
				catch (Exception ex)
				{
					// Nếu bất kỳ bước nào lỗi, Rollback ở đây là đủ
					await transaction.RollbackAsync();
					throw new Exception("Lỗi hệ thống khi cập nhật: " + ex.Message);
				}
			}
		}

		public async Task Delete(string id)
		{
			await _context.Database.ExecuteSqlRawAsync(
				"EXEC DonMuaHang_Delete @MaDMH",
				new SqlParameter("@MaDMH", id)
			);
		}

		public async Task DeleteDetail(string MaDMH, string MaSP)
		{
			await _context.Database.ExecuteSqlRawAsync(
				"EXEC CTMH_Delete @MaDMH, @MaSP",
				new SqlParameter("@MaDMH", MaDMH),
				new SqlParameter("@MaSP", MaSP)
			);
		}

		public async Task<List<DonMuaHangDetail>> Search(string? search, int? month, int? year, string? status)
		{
			var parameters = new[]
			{
				new SqlParameter("@Search", (object?)search ?? DBNull.Value),
				new SqlParameter("@Month", (object?)month ?? DBNull.Value),
				new SqlParameter("@Year", (object?)year ?? DBNull.Value)
			};

			return await _context.DonMuaHangDetail
				.FromSqlRaw("EXEC DonMuaHang_Search @Search, @Month, @Year", parameters)
				.ToListAsync();
		}

		public async Task<List<DonMuaHang>> Reset()
		{
			var data = await _context.DonMuaHang
				.FromSqlRaw("EXEC DonMuaHang_Search @Search=NULL, @Month=NULL, @Year=NULL")
				.ToListAsync();

			return data;
		}
	}
}
