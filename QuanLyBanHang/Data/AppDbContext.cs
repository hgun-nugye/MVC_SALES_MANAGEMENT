using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.Services
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		// ======== KHAI BÁO CÁC BẢNG ========
		public DbSet<Tinh> Tinh { get; set; } = null!;

		public DbSet<Xa> Xa { get; set; } = null!;
		public DbSet<Nuoc> Nuoc { get; set; } = null!;
		public DbSet<NuocDto> NuocDto { get; set; } = null!;
		public DbSet<Hang> Hang { get; set; } = null!;

		public DbSet<NhaCC> NhaCC { get; set; } = null!;
		public DbSet<NhaCCDetailView> NhaCCDetailView { get; set; } = null!;

		public DbSet<KhachHang> KhachHang { get; set; } = null!;
		public DbSet<KhachHangDetailView> KhachHangDetailView { get; set; }

		public DbSet<NhanVien> NhanVien { get; set; } = null!;

		public DbSet<NhomSP> NhomSP { get; set; } = null!;

		public DbSet<LoaiSP> LoaiSP { get; set; } = null!;
		public DbSet<LoaiSPDto> LoaiSPDtos { get; set; }

		public DbSet<SanPham> SanPham { get; set; } = null!;
		public DbSet<SanPhamDto> SanPhamDto { get; set; }

		public DbSet<DonMuaHang> DonMuaHang { get; set; } = null!;
		public DbSet<DonMuaHangDetail> DonMuaHangDetail { get; set; } = null!;

		public DbSet<CTMH> CTMH { get; set; } = null!;
		public DbSet<CTMHDetailDto> CTMHDetailDtos { get; set; }

		public DbSet<DonBanHang> DonBanHang { get; set; } = null!;
		public DbSet<DonBanHangDetail> DonBanHangDetail { get; set; } = null!;

		public DbSet<CTBH> CTBH { get; set; } = null!;
		public DbSet<CTBHDetailDto> CTBHDetailDtos { get; set; }

		//Kiểm tra tồn tại 
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CTMH>().HasKey(ct => new { ct.MaDMH, ct.MaSP });
			modelBuilder.Entity<CTBH>().HasKey(ct => new { ct.MaDBH, ct.MaSP });
		}
	}
}
