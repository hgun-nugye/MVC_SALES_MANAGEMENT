using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.Services
{
	public class CTMHService
	{
		private readonly AppDbContext _context;

		public CTMHService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<CTMHDetailDto>> GetAll()
		{
			return await _context.CTMHDetailDtos
				.FromSqlRaw("EXEC CTMH_GetAll")
				.ToListAsync();
		}

		public async Task<CTMH?> GetByID(string maDMH, string maSP)
		{
			var parameters = new[]
			{
				new SqlParameter("@MaDMH", maDMH),
				new SqlParameter("@MaSP", maSP)
			};

			var data = await _context.CTMH
				.FromSqlRaw("EXEC CTMH_GetByID @MaDMH, @MaSP", parameters)
				.ToListAsync();

			return data.FirstOrDefault();
		}

		public async Task<CTMHDetailDto?> GetDetail(string maDMH, string maSP)
		{
			var parameters = new[]
			{
				new SqlParameter("@MaDMH", maDMH),
				new SqlParameter("@MaSP", maSP)
			};

			var data = await _context.CTMHDetailDtos
				.FromSqlRaw("EXEC CTMH_GetByID @MaDMH, @MaSP", parameters)
				.ToListAsync();

			return data.FirstOrDefault();
		}

		public async Task Update(CTMH model)
		{
			var parameters = new[]
			{
				new SqlParameter("@MaDMH", model.MaDMH),
				new SqlParameter("@MaSP", model.MaSP),
				new SqlParameter("@SLM", model.SLM),
				new SqlParameter("@DGM", model.DGM)
			};

			await _context.Database.ExecuteSqlRawAsync(
				"EXEC CTMH_Update @MaDMH, @MaSP, @SLM, @DGM", parameters);
		}

		public async Task Delete(string maDMH, string maSP)
		{
			var parameters = new[]
			{
				new SqlParameter("@MaDMH", maDMH),
				new SqlParameter("@MaSP", maSP)
			};

			await _context.Database.ExecuteSqlRawAsync(
				"EXEC CTMH_Delete @MaDMH, @MaSP", parameters);
		}
	}
}
