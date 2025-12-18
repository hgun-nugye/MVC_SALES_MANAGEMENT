using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.Services
{
	public class TrangThaiMHService
	{
		private readonly AppDbContext _context;

		public TrangThaiMHService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<TrangThaiMH>> Search(string? search)
		{
			var parameter = new SqlParameter("@Search", (object?)search ?? DBNull.Value);

			return await _context.TrangThaiMH
				.FromSqlRaw("EXEC TrangThaiMH_Search @Search", parameter)
				.ToListAsync();
		}

		public async Task<List<TrangThaiMH>> GetAll()
		{
			return await _context.TrangThaiMH
				.FromSqlRaw("EXEC TrangThaiMH_GetAll")
				.ToListAsync();
		}

		public async Task<TrangThaiMH?> GetById(string id)
		{
			return (await _context.TrangThaiMH
					.FromSqlInterpolated($"EXEC TrangThaiMH_GetByID @MaTTMH = {id}")
					.ToListAsync())
				.FirstOrDefault();
		}

		public async Task Create(TrangThaiMH model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC TrangThaiMH_Insert 
                    @MaTTMH = {model.MaTTMH},
                    @TenTTMH = {model.TenTTMH}");
		}

		public async Task Update(TrangThaiMH model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC TrangThaiMH_Update 
                    @MaTTMH = {model.MaTTMH},
                    @TenTTMH = {model.TenTTMH}");
		}

		public async Task Delete(string id)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC TrangThaiMH_Delete @MaTTMH = {id}");
		}
	}
}

