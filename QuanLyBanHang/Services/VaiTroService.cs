using QuanLyBanHang.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace QuanLyBanHang.Services
{
	public class VaiTroService
	{
		private readonly AppDbContext _context;

		public VaiTroService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<VaiTro>> GetAll()
		{
			return await _context.VaiTro
				.FromSqlRaw("EXEC VaiTro_GetAll")
				.ToListAsync();
		}

		public async Task<VaiTro?> GetById(string id)
		{
			return (await _context.VaiTro
					.FromSqlInterpolated($"EXEC VaiTro_GetByID @MaVT = {id}")
					.ToListAsync())
				.FirstOrDefault();
		}

		public async Task Create(VaiTro model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC VaiTro_Insert 
                    @MaVT = {model.MaVT},
                    @TenVT = {model.TenVT}");
		}

		public async Task Update(VaiTro model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC VaiTro_Update 
                    @MaVT = {model.MaVT},
                    @TenVT = {model.TenVT}");
		}

		public async Task Delete(string id)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC VaiTro_Delete @MaVT = {id}");
		}

		public async Task<List<VaiTro>> Search(string? search)
		{
			var parameter = new SqlParameter("@Search", (object?)search ?? DBNull.Value);
			return await _context.VaiTro
				.FromSqlRaw("EXEC VaiTro_Search @Search", parameter)
				.ToListAsync();
		}
	}
}

