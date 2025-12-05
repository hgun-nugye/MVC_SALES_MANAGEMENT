using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.Services
{
	public class GianHangService
	{
		private readonly AppDbContext _context;

		public GianHangService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<GianHang>> GetAll()
		{
			return await _context.GianHang
				.FromSqlRaw("EXEC GianHang_GetAll")
				.ToListAsync();
		}
		public async Task<GianHang?> GetById(string id)
		{
			return (await _context.GianHang
				.FromSqlInterpolated($"EXEC GianHang_GetByID @MaGH = {id}")
				.ToListAsync())
				.FirstOrDefault();
		}

		public async Task Insert(GianHang model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC GianHang_Insert 
                    @TenGH = {model.TenGH},
                    @MoTaGH = {model.MoTaGH},
                    @DienThoaiGH = {model.DienThoaiGH},
                    @EmailGH = {model.EmailGH},
                    @DiaChiGH = {model.DiaChiGH},
					@MaXa = {model.MaXa}
            ");
		}

		public async Task Update(GianHang model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC GianHang_Update 
                    @MaGH = {model.MaGH},
                    @TenGH = {model.TenGH},
                    @MoTaGH = {model.MoTaGH},
                    @DienThoaiGH = {model.DienThoaiGH},
                    @EmailGH = {model.EmailGH},
                    @DiaChiGH = {model.DiaChiGH},
					@MaXa = {model.MaXa}
            ");
		}

		public async Task Delete(string id)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync(
				$@"EXEC GianHang_Delete @MaGH = {id}");
		}

		public async Task<List<GianHang>> Search(string? keyword, short? tinh)
		{
			var parameters = new[]
			{
				new SqlParameter("@Search", (object?)keyword ?? DBNull.Value),
				new SqlParameter("@MaTinh", (object?)tinh ?? DBNull.Value)
			};

			return await _context.GianHang
				.FromSqlRaw("EXEC GianHang_Search @Search, @MaTinh", parameters)
				.ToListAsync();
		}
	}
}
