using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.Services
{
	public class CTBHService
	{
		private readonly AppDbContext _context;

		public CTBHService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<CTBHDetailDto>> GetAll()
		{
			return await _context.CTBHDetailDtos
				.FromSqlRaw("EXEC CTBH_GetAll")
				.ToListAsync();
		}

		public async Task<CTBH?> GetByIDDBH(string maDBH)
		{
			var parameters = new[]
			{
				new SqlParameter("@MaDBH", maDBH)
			};

			var data = await _context.CTBH
				.FromSqlRaw("EXEC CTBH_GetByID @MaDBH", parameters)
				.ToListAsync();

			return data.FirstOrDefault();
		}

		public async Task<CTBH?> GetByID(string maDBH, string maSP)
		{
			var parameters = new[]
			{
				new SqlParameter("@MaDBH", maDBH),
				new SqlParameter("@MaSP", maSP)
			};

			var data = await _context.CTBH
				.FromSqlRaw("EXEC CTBH_GetByID @MaDBH, @MaSP", parameters)
				.ToListAsync();

			return data.FirstOrDefault();
		}

		public async Task<CTBHDetailDto?> GetDetail(string maDBH, string maSP)
		{
			var parameters = new[]
			{
				new SqlParameter("@MaDBH", maDBH),
				new SqlParameter("@MaSP", maSP)
			};

			var data = await _context.CTBHDetailDtos
				.FromSqlRaw("EXEC CTBH_GetByID @MaDBH, @MaSP", parameters)
				.ToListAsync();

			return data.FirstOrDefault();
		}

		public async Task Update(CTBH model)
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
		}

		public async Task Delete(string maDBH, string maSP)
		{
			var parameters = new[]
			{
				new SqlParameter("@MaDBH", maDBH),
				new SqlParameter("@MaSP", maSP)
			};

			await _context.Database.ExecuteSqlRawAsync(
				"EXEC CTBH_Delete @MaDBH, @MaSP", parameters);
		}
	}
}
