using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.Services
{
	public class LoaiSPService
	{
		private readonly AppDbContext _context;

		public LoaiSPService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<LoaiSPDto>> Search(string? search, string? group)
		{
			var parameters = new[]
			{
				new SqlParameter("@Search", (object?)search ?? DBNull.Value),
				new SqlParameter("@MaNhom", (object?)group ?? DBNull.Value)
			};

			return await _context.LoaiSPDtos
				.FromSqlRaw("EXEC LoaiSP_Search @Search, @MaNhom", parameters)
				.ToListAsync();
		}

		public async Task<List<LoaiSPDto>> GetAll()
		{
			return await _context.LoaiSPDtos
				.FromSqlRaw("EXEC LoaiSP_GetAll")
				.ToListAsync();
		}

		// Lấy 1 loại theo ID
		public async Task<LoaiSPDto?> GetById(string id)
		{
			return (await _context.LoaiSPDtos
				.FromSqlInterpolated($"EXEC LoaiSP_GetByID @MaLoai = {id}")
				.ToListAsync())
				.FirstOrDefault();
		}

		// Thêm mới
		public async Task Insert(LoaiSP model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC LoaiSP_Insert 
                    @MaLoai = {model.MaLoai},
                    @TenLoai = {model.TenLoai},
                    @MaNhom = {model.MaNhom}
            ");
		}

		// Cập nhật
		public async Task Update(LoaiSPDto model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC LoaiSP_Update 
                    @MaLoai = {model.MaLoai},
                    @TenLoai = {model.TenLoai},
                    @MaNhom = {model.MaNhom}
            ");
		}

		// Xóa
		public async Task Delete(string id)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC LoaiSP_Delete @MaLoai = {id}
            ");
		}

		// Lấy danh sách nhóm SP để lọc + dropdown
		public async Task<List<NhomSP>> GetGroups()
		{
			return await _context.NhomSP
				.FromSqlRaw("EXEC NhomSP_GetAll")
				.ToListAsync();
		}
	}
}
