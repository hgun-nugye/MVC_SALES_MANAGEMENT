using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.Services
{
	public class XaService
	{
		private readonly AppDbContext _context;

		public XaService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<XaDTO>> Search(string? search, string? tinh)
		{
			var parameters = new[]
			{
				new SqlParameter("@Search", (object?)search ?? DBNull.Value),
				new SqlParameter("@MaTinh", (object?)tinh ?? DBNull.Value)
			};

			return await _context.XaDTO
				.FromSqlRaw("EXEC Xa_Search @Search, @MaTinh", parameters)
				.ToListAsync();
		}


		public async Task<List<Xa>> GetAll()
		{
			return await _context.Xa
				.FromSqlRaw("EXEC Xa_GetAll")
				.ToListAsync();
		}

		//READ - Danh sách Xã
		public async Task<List<Xa>> GetAllWithTinh()
		{
			return await _context.Xa.FromSqlRaw("EXEC Xa_GetAllWithTinh").ToListAsync();
		}

		// DETAILS - Xem chi tiết
		public async Task<Xa?> GetByID(string id)
		{
			var dto = (await _context.XaDTO.FromSqlInterpolated($"EXEC Xa_GetByIDWithTinh @MaXa = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (dto == null) return null;

			return new Xa
			{
				MaXa = dto.MaXa ?? "",
				TenXa = dto.TenXa ?? "",
				MaTinh = dto.MaTinh ?? "",
				TenTinh = dto.TenTinh
			};
		}

		// CREATE
		public async Task Create(Xa model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
						EXEC Xa_Insert 
							@TenXa = {model.TenXa},
							@MaTinh = {model.MaTinh}
					");

			// Clear EF cache
			_context.ChangeTracker.Clear();
		}

		// EDIT
		public async Task Update(Xa model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
						EXEC Xa_Update
						@MaXa ={model.MaXa}, 
						@TenXa = {model.TenXa},
						@MaTinh= {model.MaTinh}");
		}


		// DELETE 
		public async Task Delete(string id)
		{
			var xa = await GetByID(id);
			if (xa == null)
				throw new KeyNotFoundException("Xã không tồn tại!");

			await _context.Database.ExecuteSqlInterpolatedAsync($@"EXEC Xa_Delete @MaXa = {id}");
		}

		// Lấy danh sách Xã theo Tỉnh
		public async Task<List<Xa>> GetByIDTinh(string maTinh)
		{
			return await _context.Xa
				.FromSqlInterpolated($"EXEC Xa_GetByIDTinh @MaTinh={maTinh}")
				.ToListAsync();
		}

		// Lấy thông tin Tỉnh từ MaXa
		public async Task<string?> GetMaTinhByXa(string maXa)
		{
			var xa = (await _context.Xa
				.FromSqlInterpolated($"EXEC Xa_GetByIDWithTinh @MaXa={maXa}")
				.ToListAsync())
				.FirstOrDefault();

			return xa?.MaTinh ?? "";
		}
	}
}
