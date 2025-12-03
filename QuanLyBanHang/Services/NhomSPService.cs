using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.Services
{
	public class NhomSPService
	{
		private readonly AppDbContext _context;

		public NhomSPService(AppDbContext context)
		{
			_context = context;
		}

		// Lấy tất cả nhóm sản phẩm
		public async Task<List<NhomSP>> GetAll()
		{
			return await _context.NhomSP
				.FromSqlRaw("EXEC Nhom_GetAll")
				.ToListAsync();
		}

		// Lấy theo ID
		public async Task<NhomSP?> GetById(string id)
		{
			return (await _context.NhomSP
				.FromSqlInterpolated($"EXEC Nhom_GetByID @MaNhom = {id}")
				.ToListAsync())
				.FirstOrDefault();
		}

		// Thêm
		public async Task Insert(NhomSP model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC Nhom_Insert 
                    @TenNhom = {model.TenNhom}
            ");
		}

		// Cập nhật
		public async Task Update(NhomSP model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC Nhom_Update 
                    @MaNhom = {model.MaNhom},
                    @TenNhom = {model.TenNhom}
            ");
		}

		// Xóa
		public async Task Delete(string id)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC Nhom_Delete @MaNhom = {id}
            ");
		}
	}
}
