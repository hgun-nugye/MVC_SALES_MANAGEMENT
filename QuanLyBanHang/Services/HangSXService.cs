using QuanLyBanHang.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace QuanLyBanHang.Services
{
	public class HangSXService
	{
		private readonly AppDbContext _context;

		public HangSXService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<HangSX>> Search(string? search)
		{
			var parameter = new SqlParameter("@Search", (object?)search ?? DBNull.Value);

			return await _context.Hang
				.FromSqlRaw("EXEC HangSX_Search @Search", parameter)
				.ToListAsync();
		}

		// Lấy danh sách tất cả Hãng
		public async Task<List<HangSX>> GetAll()
		{
			return await _context.Hang
				.FromSqlRaw("EXEC HangSX_GetAll")
				.ToListAsync();
		}

		// Lấy Hãng theo ID
		public async Task<HangSX?> GetByID(string id)
		{
			return (await _context.Hang
				.FromSqlInterpolated($"EXEC HangSX_GetByID @MaHangSX = {id}")
				.ToListAsync())
				.FirstOrDefault();
		}

		// Thêm Hãng
		public async Task Create(HangSX model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC HangSX_Insert 
                    @TenHangSX = {model.TenHangSX},
                    @MaNuoc = {model.MaNuoc}");
		}

		// Cập nhật Hãng
		public async Task Update(HangSX model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC HangSX_Update 
                    @MaHangSX = {model.MaHangSX},
                    @TenHangSX = {model.TenHangSX},
                    @MaNuoc = {model.MaNuoc}");
		}

		// Xóa Hãng
		public async Task Delete(string id)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC HangSX_Delete @MaHangSX = {id}");
		}
	}
}

