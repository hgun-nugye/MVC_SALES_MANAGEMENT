using QuanLyBanHang.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace QuanLyBanHang.Services
{
	public class NuocService
	{
		private readonly AppDbContext _context;

		public NuocService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<Nuoc>> Search(string? search)
		{
			var parameter = new SqlParameter("@Search", (object?)search ?? DBNull.Value);

			return await _context.Nuoc
				.FromSqlRaw("EXEC Nuoc_Search @Search", parameter)
				.ToListAsync();
		}

		// Lấy danh sách tất cả Nước
		public async Task<List<Nuoc>> GetAll()
		{
			return await _context.Nuoc
				.FromSqlRaw("EXEC Nuoc_GetAll")
				.ToListAsync();
		}

		// Lấy Nước theo ID
		public async Task<Nuoc?> GetByID(string id)
		{
			return (await _context.Nuoc
				.FromSqlInterpolated($"EXEC Nuoc_GetByID @MaNuoc = {id}")
				.ToListAsync())
				.FirstOrDefault();
		}

		// Thêm Nước
		public async Task Create(Nuoc model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC Nuoc_Insert 
                    @TenNuoc = {model.TenNuoc}");
		}

		// Cập nhật Nước
		public async Task Update(Nuoc model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC Nuoc_Update 
                    @MaNuoc = {model.MaNuoc},
                    @TenNuoc = {model.TenNuoc}");
		}

		// Xóa Nước
		public async Task Delete(string id)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC Nuoc_Delete @MaNuoc = {id}");
		}
	}
}

