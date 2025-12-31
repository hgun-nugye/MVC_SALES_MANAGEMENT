using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using System.Data;

namespace QuanLyBanHang.Services
{
	public class DonBanHangService
	{
		private readonly AppDbContext _context;

		public DonBanHangService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<DonBanHangDetail>> GetAll()
		{
			return await _context.DonBanHangDetail
				.FromSqlRaw("EXEC DonBanHang_GetAll")
				.ToListAsync();
		}

		public async Task<List<DonBanHangDetail>> GetByID(string id)
		{
			var param = new SqlParameter("@MaDBH", id);

			return await _context.DonBanHangDetail
				.FromSqlRaw("EXEC DonBanHang_GetByID @MaDBH", param)
				.ToListAsync();
		}


		// Lấy chi tiết 1 chi tiết sản phẩm
		public async Task<CTBH?> GetDetail(string MaDBH, string MaSP)
		{
			var parameters = new[]
			{
				new SqlParameter("@MaDBH", MaDBH),
				new SqlParameter("@MaSP", MaSP)
			};

			var data = await _context.CTBHDetailDtos
				.FromSqlRaw("EXEC CTBH_GetByID @MaDBH, @MaSP", parameters)
				.ToListAsync();

			var model = data.FirstOrDefault();
			if (model == null) return null;

			return new CTBH
			{
				MaDBH = model.MaDBH,
				MaSP = model.MaSP,
				SLB = model.SLB,
				DGB = model.DGB,
				TenSP = model.TenSP
			};
		}
		public async Task<bool> Create(DonBanHang model)
		{
			if (model.CTBHs == null || !model.CTBHs.Any())
				return false;

			var table = new DataTable();
			table.Columns.Add("MaSP", typeof(string));
			table.Columns.Add("SLB", typeof(int));
			table.Columns.Add("DGB", typeof(decimal));

			foreach (var ct in model.CTBHs)
			{
				var spList = await _context.SanPhamDto
								.FromSqlRaw("EXEC SanPham_GetByID @MaSP", new SqlParameter("@MaSP", ct.MaSP))
								.ToListAsync();
				var sp = spList.FirstOrDefault();

				decimal giaThucTe = sp?.GiaBan ?? 0;

				table.Rows.Add(ct.MaSP, ct.SLB, giaThucTe);
			}
			string finalStatus = string.IsNullOrEmpty(model.MaTTBH) ? "CHO" : model.MaTTBH;
						
			var parameters = new[]
			{
				new SqlParameter("@NgayBH", model.NgayBH),
				new SqlParameter("@MaKH", model.MaKH),
				new SqlParameter("@DiaChiDBH", model.DiaChiDBH),
				new SqlParameter("@MaXa", model.MaXa),
				new SqlParameter("@MaTTBH", finalStatus),
				new SqlParameter("@ChiTiet", table)
				{
					SqlDbType = SqlDbType.Structured,
					TypeName = "dbo.CTBH_List"
				}
			};

			await _context.Database.ExecuteSqlRawAsync(
				"EXEC DonBanHang_Insert @NgayBH, @MaKH, @DiaChiDBH, @MaXa, @MaTTBH, @ChiTiet", parameters
			);

			return true;
		}

		public async Task Update(DonBanHang model)
		{
			var table = new DataTable();
			table.Columns.Add("MaSP", typeof(string));
			table.Columns.Add("SLB", typeof(int));
			table.Columns.Add("DGB", typeof(decimal));

			foreach (var ct in model.CTBHs)
				table.Rows.Add(ct.MaSP, ct.SLB, ct.DGB);

			var parameters = new[]
			{
				new SqlParameter("@MaDBH", model.MaDBH),
				new SqlParameter("@NgayBH", model.NgayBH),
				new SqlParameter("@MaKH", model.MaKH),
				new SqlParameter("@DiaChiDBH", model.DiaChiDBH),
				new SqlParameter("@MaXa", model.MaXa),
				new SqlParameter("@MaTTBH", model.MaTTBH),
				new SqlParameter("@ChiTiet", table)
				{
					SqlDbType = SqlDbType.Structured,
					TypeName = "dbo.CTBH_List"
				}
			};

			await _context.Database.ExecuteSqlRawAsync(
				"EXEC DonBanHang_Update @MaDBH, @NgayBH, @MaKH, @DiaChiDBH, @MaXa, @MaTTBH, @ChiTiet", parameters
			);
		}

		public async Task Delete(string id)
		{
			var param = new SqlParameter("@MaDBH", id);
			await _context.Database.ExecuteSqlRawAsync("EXEC DonBanHang_Delete @MaDBH", param);
		}

		public async Task DeleteDetail(string maDBH, string maSP)
		{
			await _context.Database.ExecuteSqlRawAsync(
				"EXEC CTBH_Delete @MaDBH, @MaSP",
				new SqlParameter("@MaDBH", maDBH),
				new SqlParameter("@MaSP", maSP)
			);
		}

		public async Task<List<DonBanHangDetailDto>> Search(string? keyword, int? month, int? year, string? MaTTBH)
		{
			var parameters = new[]
			{
				new SqlParameter("@Search", (object?)keyword ?? DBNull.Value),
				new SqlParameter("@Month", (object?)month ?? DBNull.Value),
				new SqlParameter("@Year", (object?)year ?? DBNull.Value),
				new SqlParameter("@MaTTBH", (object?)MaTTBH ?? DBNull.Value)
			};

			var data = await _context.DonBanHangDetailDto
				.FromSqlRaw("EXEC DonBanHang_Search @Search, @Month, @Year, @MaTTBH", parameters)
				.ToListAsync();

			return data;
		}

		public async Task<List<DonBanHang>> Reset()
		{
			var data = await _context.DonBanHang
				.FromSqlRaw("EXEC DonBanHang_Search @Search=NULL, @Month=NULL, @Year=NULL, @MaTTBH=NULL")
				.ToListAsync();

			return data;
		}

		public async Task ConfirmOrder(string maDBH)
		{
			await _context.Database.ExecuteSqlRawAsync(
				"EXEC DonBanHang_Confirm @MaDBH",
				new SqlParameter("@MaDBH", maDBH)
			);
		}

		public async Task CancelOrder(string maDBH)
		{
			await _context.Database.ExecuteSqlRawAsync(
				"EXEC DonBanHang_Cancel @MaDBH",
				new SqlParameter("@MaDBH", maDBH)
			);
		}
	}
}
