using QuanLyBanHang.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace QuanLyBanHang.Services
{
	public class NhanVienService
	{
		private readonly AppDbContext _context;

		public NhanVienService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<NhanVien>> Search(string? search)
		{
			var parameter = new SqlParameter("@Search", (object?)search ?? DBNull.Value);

			return await _context.NhanVien
				.FromSqlRaw("EXEC NhanVien_Search @Search", parameter)
				.ToListAsync();
		}

		// Lấy danh sách tất cả Nhân Viên
		public async Task<List<NhanVien>> GetAll()
		{
			return await _context.NhanVien
				.FromSqlRaw("EXEC NhanVien_GetAll")
				.ToListAsync();
		}

		// Lấy Nhân Viên theo ID
		public async Task<NhanVien?> GetByID(string id)
		{
			return (await _context.NhanVien
				.FromSqlInterpolated($"EXEC NhanVien_GetByID @MaNV = {id}")
				.ToListAsync())
				.FirstOrDefault();
		}

		// Thêm Nhân Viên
		public async Task Create(NhanVien model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC NhanVien_Insert 
                    @TenNV = {model.TenNV},
                    @VaiTro = {model.VaiTro},
                    @TenDNNV = {model.TenDNNV},
                    @MatKhauNV = {model.MatKhauNV}");
		}

		// Cập nhật Nhân Viên
		public async Task Update(NhanVien model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC NhanVien_Update 
                    @MaNV = {model.MaNV},
                    @TenNV = {model.TenNV},
                    @VaiTro = {model.VaiTro},
                    @TenDNNV = {model.TenDNNV},
                    @MatKhauNV = {model.MatKhauNV}");
		}

		// Xóa Nhân Viên
		public async Task Delete(string id)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC NhanVien_Delete @MaNV = {id}");
		}
	}
}

