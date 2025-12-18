using QuanLyBanHang.Models;
using Microsoft.EntityFrameworkCore;

namespace QuanLyBanHang.Services
{
	public class PhanQuyenService
	{
		private readonly AppDbContext _context;

		public PhanQuyenService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<PhanQuyen>> GetAll()
		{
			return await _context.PhanQuyen
				.Include(pq => pq.VaiTro)
				.Include(pq => pq.NhanVien)
				.ToListAsync();
		}

		public async Task<List<PhanQuyen>> GetByNhanVien(string maNV)
		{
			return await _context.PhanQuyen
				.Include(pq => pq.VaiTro)
				.Include(pq => pq.NhanVien)
				.Where(pq => pq.MaNV == maNV)
				.ToListAsync();
		}

		public async Task<PhanQuyen?> GetById(string maVT, string maNV)
		{
			return await _context.PhanQuyen
				.Include(pq => pq.VaiTro)
				.Include(pq => pq.NhanVien)
				.FirstOrDefaultAsync(pq => pq.MaVT == maVT && pq.MaNV == maNV);
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

		// Search using LINQ
		public async Task<List<PhanQuyen>> Search(string? search)
		{
			var all = await GetAll();
			if (string.IsNullOrWhiteSpace(search))
				return all;

			search = search.ToLower();
			return all.Where(pq => 
				pq.MaVT.ToLower().Contains(search) || 
				pq.MaNV.ToLower().Contains(search) ||
				(pq.VaiTro != null && pq.VaiTro.TenVT.ToLower().Contains(search)) ||
				(pq.NhanVien != null && pq.NhanVien.TenNV.ToLower().Contains(search))
			).ToList();
		}
	}
}

