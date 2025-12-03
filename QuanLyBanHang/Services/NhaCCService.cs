using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.Services
{
	public class NhaCCService
	{
		private readonly AppDbContext _context;

		public NhaCCService(AppDbContext context)
		{
			_context = context;
		}

		// Lấy danh sách + phân trang
		public async Task<List<NhaCC>> GetList(string? search, string? province)
		{
			var parameters = new[]
			{
				new SqlParameter("@Search", (object?)search ?? DBNull.Value),
				new SqlParameter("@MaTinh", string.IsNullOrEmpty(province) ? DBNull.Value : province)
			};

			return await _context.NhaCC
				.FromSqlRaw("EXEC NhaCC_Search @Search, @MaTinh", parameters)
				.ToListAsync();
		}

		public async Task<List<NhaCC>> GetAll()
		{
			return await _context.NhaCC
				.FromSqlRaw("EXEC NhaCC_GetAll")
				.ToListAsync();
		}
		public async Task<NhaCC?> GetById(string id)
		{
			return (await _context.NhaCC
				.FromSqlInterpolated($"EXEC NhaCC_GetByID @MaNCC = {id}")
				.ToListAsync())
				.FirstOrDefault();
		}

		public async Task Create(NhaCC model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC NhaCC_Insert 
                    @TenNCC = {model.TenNCC}, 
                    @DienThoaiNCC = {model.DienThoaiNCC}, 
                    @EmailNCC = {model.EmailNCC}, 
                    @DiaChiNCC = {model.DiaChiNCC},
					@MaXa = {model.MaXa}
            ");
		}

		public async Task Update(NhaCC model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC NhaCC_Update 
                    @MaNCC = {model.MaNCC},
                    @TenNCC = {model.TenNCC}, 
                    @DienThoaiNCC = {model.DienThoaiNCC}, 
                    @EmailNCC = {model.EmailNCC}, 
                    @DiaChiNCC = {model.DiaChiNCC},
					@MaXa = {model.MaXa}
            ");
		}

		public async Task Delete(string id)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"EXEC NhaCC_Delete @MaNCC = {id}");
		}

		public async Task<List<NhaCC>> Search(string keyword, string? province)
		{
			var parameters = new[]
			{
				new SqlParameter("@Search", (object?)keyword ?? DBNull.Value),
				new SqlParameter("@MaTinh", (object?)province ?? DBNull.Value)
			};

			return await _context.NhaCC
				.FromSqlRaw("EXEC NhaCC_Search @Search, @MaTinh", parameters)
				.ToListAsync();
		}

		public async Task<List<NhaCC>> Clear()
		{
			return await _context.NhaCC
				.FromSqlRaw("EXEC NhaCC_Search @Search=NULL, @MaTinh=NULL")
				.ToListAsync();
		}

	}
}
