using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.Services
{
	public class TrangThaiBHService
	{
		private readonly AppDbContext _context;

		public TrangThaiBHService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<TrangThaiBH>> Search(string? search)
		{
			var parameter = new SqlParameter("@Search", (object?)search ?? DBNull.Value);

			return await _context.TrangThaiBH
				.FromSqlRaw("EXEC TrangThaiBH_Search @Search", parameter)
				.ToListAsync();
		}

		public async Task<List<TrangThaiBH>> GetAll()
		{
			return await _context.TrangThaiBH
				.FromSqlRaw("EXEC TrangThaiBH_GetAll")
				.ToListAsync();
		}

		public async Task<TrangThaiBH?> GetById(string id)
		{
			return (await _context.TrangThaiBH
					.FromSqlInterpolated($"EXEC TrangThaiBH_GetByID @MaTTBH = {id}")
					.ToListAsync())
				.FirstOrDefault();
		}

		public async Task Create(TrangThaiBH model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC TrangThaiBH_Insert 
                    @MaTTBH = {model.MaTTBH},
                    @TenTTBH = {model.TenTTBH}");
		}

		public async Task Update(TrangThaiBH model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC TrangThaiBH_Update 
                    @MaTTBH = {model.MaTTBH},
                    @TenTTBH = {model.TenTTBH}");
		}

		public async Task Delete(string id)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC TrangThaiBH_Delete @MaTTBH = {id}");
		}
	}
}

