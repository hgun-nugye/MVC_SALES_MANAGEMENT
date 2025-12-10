using QuanLyBanHang.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace QuanLyBanHang.Services
{
	public class HangService
	{
		private readonly AppDbContext _context;

		public HangService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<Hang>> Search(string? search)
		{
			var parameter = new SqlParameter("@Search", (object?)search ?? DBNull.Value);

			return await _context.Hang
				.FromSqlRaw("EXEC Hang_Search @Search", parameter)
				.ToListAsync();
		}

		// Lấy danh sách tất cả Hãng
		public async Task<List<Hang>> GetAll()
		{
			return await _context.Hang
				.FromSqlRaw("EXEC Hang_GetAll")
				.ToListAsync();
		}

		// Lấy Hãng theo ID
		public async Task<Hang?> GetByID(string id)
		{
			return (await _context.Hang
				.FromSqlInterpolated($"EXEC Hang_GetByID @MaHang = {id}")
				.ToListAsync())
				.FirstOrDefault();
		}

		// Thêm Hãng
		public async Task Create(Hang model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC Hang_Insert 
                    @TenHang = {model.TenHang},
                    @MaNuoc = {model.MaNuoc}");
		}

		// Cập nhật Hãng
		public async Task Update(Hang model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC Hang_Update 
                    @MaHang = {model.MaHang},
                    @TenHang = {model.TenHang},
                    @MaNuoc = {model.MaNuoc}");
		}

		// Xóa Hãng
		public async Task Delete(string id)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC Hang_Delete @MaHang = {id}");
		}
	}
}

