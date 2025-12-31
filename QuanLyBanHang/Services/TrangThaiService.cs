using QuanLyBanHang.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace QuanLyBanHang.Services
{
	public class TrangThaiService
	{
		private readonly AppDbContext _context;

		public TrangThaiService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<TrangThai>> GetAll()
		{
			return await _context.TrangThai
				.FromSqlRaw("EXEC TrangThai_GetAll")
				.ToListAsync();
		}

		public async Task<TrangThai?> GetById(string id)
		{
			return (await _context.TrangThai
					.FromSqlInterpolated($"EXEC TrangThai_GetByID @MaTT = {id}")
					.ToListAsync())
				.FirstOrDefault();
		}

		public async Task Create(TrangThai model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC TrangThai_Insert 
                    @TenTT = {model.TenTT}");
		}

		public async Task Update(TrangThai model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC TrangThai_Update 
                    @MaTT = {model.MaTT},
                    @TenTT = {model.TenTT}");
		}

		public async Task Delete(string id)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC TrangThai_Delete @MaTT = {id}");
		}

		public async Task<List<TrangThai>> Search(string? search)
		{
			var parameter = new SqlParameter("@Search", (object?)search ?? DBNull.Value);
			return await _context.TrangThai
				.FromSqlRaw("EXEC TrangThai_Search @Search", parameter)
				.ToListAsync();
		}
	}
}

