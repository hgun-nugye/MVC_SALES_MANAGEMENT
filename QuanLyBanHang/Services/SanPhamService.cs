using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using System.Data;

namespace QuanLyBanHang.Services
{
	public class SanPhamService
	{
		private readonly AppDbContext _context;

		public SanPhamService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<SanPhamDto>> GetAll(string? search, string? status, string? type)
		{
			var parameters = new[]
			{
				new SqlParameter("@Search", SqlDbType.NVarChar, 100) { Value = string.IsNullOrEmpty(search) ? DBNull.Value : search },
				new SqlParameter("@TrangThai", SqlDbType.NVarChar, 50) { Value = string.IsNullOrEmpty(status) ? DBNull.Value : status },
				new SqlParameter("@TenLoai", SqlDbType.VarChar, 10) { Value = string.IsNullOrEmpty(type) ? DBNull.Value : type }
			};

			return await _context.SanPhamDto
				.FromSqlRaw("EXEC SanPham_Search @Search, @TrangThai, @TenLoai, NULL, NULL", parameters)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<SanPhamDto?> GetById(string id)
		{
			return (await _context.SanPhamDto
				.FromSqlRaw("EXEC SanPham_GetByID @MaSP", new SqlParameter("@MaSP", id))
				.ToListAsync())
				.FirstOrDefault();
		}

		public async Task Create(SanPham sp, string? filePath)
		{
			await _context.Database.ExecuteSqlRawAsync(
				"EXEC SanPham_Insert @TenSP, @DonGia, @GiaBan, @MoTaSP, @AnhMH, @ThanhPhan, @CongDung, @HDSD, @XuatXu, @BaoQuan, @TrangThai, @SoLuongTon, @MaLoai, @MaNCC, @MaGH",
				new SqlParameter("@TenSP", sp.TenSP),
				new SqlParameter("@DonGia", sp.DonGia),
				new SqlParameter("@GiaBan", sp.GiaBan),
				new SqlParameter("@MoTaSP", sp.MoTaSP),
				new SqlParameter("@AnhMH", filePath ?? (object)DBNull.Value),
				new SqlParameter("@ThanhPhan", sp.ThanhPhan),
				new SqlParameter("@CongDung", sp.CongDung),
				new SqlParameter("@HDSD", sp.HDSD),
				new SqlParameter("@XuatXu", sp.XuatXu),
				new SqlParameter("@BaoQuan", sp.BaoQuan),
				new SqlParameter("@TrangThai", sp.TrangThai),
				new SqlParameter("@SoLuongTon", sp.SoLuongTon),
				new SqlParameter("@MaLoai", sp.MaLoai ?? (object)DBNull.Value),
				new SqlParameter("@MaNCC", sp.MaNCC ?? (object)DBNull.Value),
				new SqlParameter("@MaGH", sp.MaGH ?? (object)DBNull.Value)
			);
		}

		// Cập nhật sản phẩm
		public async Task Update(SanPham sp, string? filePath)
		{
			await _context.Database.ExecuteSqlRawAsync(
				"EXEC SanPham_Update @MaSP, @TenSP, @DonGia, @GiaBan, @MoTaSP, @AnhMH, @ThanhPhan, @CongDung, @HDSD, @XuatXu, @BaoQuan, @TrangThai, @SoLuongTon, @MaLoai, @MaNCC, @MaGH",
				new SqlParameter("@MaSP", sp.MaSP),
				new SqlParameter("@TenSP", sp.TenSP),
				new SqlParameter("@DonGia", sp.DonGia),
				new SqlParameter("@GiaBan", sp.GiaBan),
				new SqlParameter("@MoTaSP", sp.MoTaSP),
				new SqlParameter("@AnhMH", filePath ?? (object)DBNull.Value),
				new SqlParameter("@ThanhPhan", sp.ThanhPhan),
				new SqlParameter("@CongDung", sp.CongDung),
				new SqlParameter("@HDSD", sp.HDSD),
				new SqlParameter("@XuatXu", sp.XuatXu),
				new SqlParameter("@BaoQuan", sp.BaoQuan),
				new SqlParameter("@TrangThai", sp.TrangThai),
				new SqlParameter("@SoLuongTon", sp.SoLuongTon),
				new SqlParameter("@MaLoai", sp.MaLoai ?? (object)DBNull.Value),
				new SqlParameter("@MaNCC", sp.MaNCC ?? (object)DBNull.Value),
				new SqlParameter("@MaGH", sp.MaGH ?? (object)DBNull.Value)
			);
		}

		// Xóa sản phẩm
		public async Task Delete(string id)
		{
			await _context.Database.ExecuteSqlRawAsync(
				"EXEC SanPham_Delete @MaSP",
				new SqlParameter("@MaSP", id)
			);
		}
	}
}
