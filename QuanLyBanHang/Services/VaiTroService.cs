using QuanLyBanHang.Models;
using Microsoft.EntityFrameworkCore;

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

		// Search using LINQ since there's no Search procedure
		public async Task<List<VaiTro>> Search(string? search)
		{
			var all = await GetAll();
			if (string.IsNullOrWhiteSpace(search))
				return all;

			search = search.ToLower();
			return all.Where(v => 
				v.MaVT.ToLower().Contains(search) || 
				v.TenVT.ToLower().Contains(search)
			).ToList();
		}
	}
}

