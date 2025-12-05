using QuanLyBanHang.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace QuanLyBanHang.Services
{
	public class TinhService
	{
		private readonly AppDbContext _context;

		public TinhService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<Tinh>> Search(string? search)
		{
			var parameter = new SqlParameter("@Search", (object?)search ?? DBNull.Value);

			return await _context.Tinh
				.FromSqlRaw("EXEC Tinh_Search @Search", parameter)
				.ToListAsync();
		}

		// Lấy danh sách tất cả Tỉnh
		public async Task<List<Tinh>> GetAll()
		{
			return await _context.Tinh
				.FromSqlRaw("EXEC Tinh_GetAll")
				.ToListAsync();
		}

		// Lấy Tỉnh theo ID
		public async Task<Tinh?> GetByID(string id)
		{
			return (await _context.Tinh
				.FromSqlInterpolated($"EXEC Tinh_GetByID @MaTinh = {id}")
				.ToListAsync())
				.FirstOrDefault();
		}

		// Thêm Tỉnh
		public async Task Create(Tinh model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC Tinh_Insert 
                    @TenTinh = {model.TenTinh}");
		}

		// Cập nhật Tỉnh
		public async Task Update(Tinh model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC Tinh_Update 
                    @MaTinh = {model.MaTinh},
                    @TenTinh = {model.TenTinh}");
		}

		// Xóa Tỉnh
		public async Task Delete(string id)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC Tinh_Delete @MaTinh = {id}");
		}
	}
}
