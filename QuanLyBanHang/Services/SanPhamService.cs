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

		public async Task<List<SanPhamDto>> GetAll()
		{
			return await _context.SanPhamDto
				.FromSqlRaw("EXEC SanPham_GetAll")
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<List<SanPhamDto>> Search(string? search, string? status, string? type)
		{
			var parameters = new[]
			{
				new SqlParameter("@Search", SqlDbType.NVarChar, 100) { Value = string.IsNullOrEmpty(search) ? DBNull.Value : search },
				new SqlParameter("@TrangThai", SqlDbType.NVarChar, 50) { Value = string.IsNullOrEmpty(status) ? DBNull.Value : status },
				new SqlParameter("@TenLoai", SqlDbType.VarChar, 10) { Value = string.IsNullOrEmpty(type) ? DBNull.Value : type }
			};

			return await _context.SanPhamDto
				.FromSqlRaw("EXEC SanPham_Search @Search, @TrangThai, @TenLoai", parameters)
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
				"EXEC SanPham_Insert @TenSP, @DonGia, @GiaBan, @MoTaSP, @AnhMH, @ThanhPhan, @CongDung, @HDSD, @XuatXu, @BaoQuan, @TrangThai, @SoLuongTon, @MaLoai, @MaHang",
				new SqlParameter("@TenSP", sp.TenSP),
				new SqlParameter("@DonGia", sp.DonGia),
				new SqlParameter("@GiaBan", sp.GiaBan),
				new SqlParameter("@MoTaSP", sp.MoTaSP ?? (object)DBNull.Value),
				new SqlParameter("@AnhMH", filePath ?? (object)DBNull.Value),
				new SqlParameter("@ThanhPhan", sp.ThanhPhan ?? (object)DBNull.Value),
				new SqlParameter("@CongDung", sp.CongDung ?? (object)DBNull.Value),
				new SqlParameter("@HDSD", sp.HDSD ?? (object)DBNull.Value),
				new SqlParameter("@XuatXu", sp.XuatXu ?? (object)DBNull.Value),
				new SqlParameter("@BaoQuan", sp.BaoQuan ?? (object)DBNull.Value),
				new SqlParameter("@TrangThai", sp.TrangThai),
				new SqlParameter("@SoLuongTon", sp.SoLuongTon),
				new SqlParameter("@MaLoai", sp.MaLoai),
				new SqlParameter("@MaHang", sp.MaHang)
			);
		}

		// Cập nhật sản phẩm
		public async Task Update(SanPham sp, string? filePath)
		{
			await _context.Database.ExecuteSqlRawAsync(
				"EXEC SanPham_Update @MaSP, @TenSP, @DonGia, @GiaBan, @MoTaSP, @AnhMH, @ThanhPhan, @CongDung, @HDSD, @XuatXu, @BaoQuan, @TrangThai, @SoLuongTon, @MaLoai, @MaHang",
				new SqlParameter("@MaSP", sp.MaSP),
				new SqlParameter("@TenSP", sp.TenSP),
				new SqlParameter("@DonGia", sp.DonGia),
				new SqlParameter("@GiaBan", sp.GiaBan),
				new SqlParameter("@MoTaSP", sp.MoTaSP ?? (object)DBNull.Value),
				new SqlParameter("@AnhMH", filePath ?? (object)DBNull.Value),
				new SqlParameter("@ThanhPhan", sp.ThanhPhan ?? (object)DBNull.Value),
				new SqlParameter("@CongDung", sp.CongDung ?? (object)DBNull.Value),
				new SqlParameter("@HDSD", sp.HDSD ?? (object)DBNull.Value),
				new SqlParameter("@XuatXu", sp.XuatXu ?? (object)DBNull.Value),
				new SqlParameter("@BaoQuan", sp.BaoQuan ?? (object)DBNull.Value),
				new SqlParameter("@TrangThai", sp.TrangThai),
				new SqlParameter("@SoLuongTon", sp.SoLuongTon),
				new SqlParameter("@MaLoai", sp.MaLoai),
				new SqlParameter("@MaHang", sp.MaHang)
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
