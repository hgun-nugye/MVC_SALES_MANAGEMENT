using QuanLyBanHang.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace QuanLyBanHang.Services
{
	public class PhanQuyenService
	{
		private readonly AppDbContext _context;

		public PhanQuyenService(AppDbContext context)
		{
			_context = context;
		}

		private List<PhanQuyen> MapToPhanQuyen(List<PhanQuyenDto> dtos)
		{
			return dtos.Select(d => new PhanQuyen
			{
				MaVT = d.MaVT,
				MaNV = d.MaNV,
				VaiTro = new VaiTro { MaVT = d.MaVT, TenVT = d.TenVT },
				NhanVien = new NhanVien { MaNV = d.MaNV, TenNV = d.TenNV }
			}).ToList();
		}

		public async Task<List<PhanQuyen>> GetAll()
		{
			var dtos = await _context.PhanQuyenDto
				.FromSqlRaw("EXEC PhanQuyen_GetAll")
				.ToListAsync();
			return MapToPhanQuyen(dtos);
		}

		public async Task<List<PhanQuyen>> GetByNhanVien(string maNV)
		{
			var dtos = await _context.PhanQuyenDto
				.FromSqlRaw("EXEC PhanQuyen_GetByNhanVien @MaNV", new SqlParameter("@MaNV", maNV))
				.ToListAsync();
			return MapToPhanQuyen(dtos);
		}

		public async Task<PhanQuyen?> GetById(string maVT, string maNV)
		{
			var parameters = new[] {
				new SqlParameter("@MaVT", maVT),
				new SqlParameter("@MaNV", maNV)
			};
			var dtos = await _context.PhanQuyenDto
				.FromSqlRaw("EXEC PhanQuyen_GetByID @MaVT, @MaNV", parameters)
				.ToListAsync();
			return MapToPhanQuyen(dtos).FirstOrDefault();
		}

		public async Task Create(PhanQuyen model)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC PhanQuyen_Insert 
                    @MaVT = {model.MaVT},
                    @MaNV = {model.MaNV}");
		}

		public async Task Update(string maNV, string oldMaVT, string newMaVT)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC PhanQuyen_Update 
                    @MaNV = {maNV},
                    @OldMaVT = {oldMaVT},
                    @NewMaVT = {newMaVT}");
		}

		public async Task Delete(string maVT, string maNV)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC PhanQuyen_Delete 
                    @MaVT = {maVT},
                    @MaNV = {maNV}");
		}

		public async Task<List<PhanQuyen>> Search(string? search)
		{
			var dtos = await _context.PhanQuyenDto
				.FromSqlRaw("EXEC PhanQuyen_Search @Search", new SqlParameter("@Search", (object?)search ?? DBNull.Value))
				.ToListAsync();
			return MapToPhanQuyen(dtos);
		}
	}
}

