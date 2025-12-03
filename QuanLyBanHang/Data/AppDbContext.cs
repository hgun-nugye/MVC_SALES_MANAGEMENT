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
		public DbSet<TinhService> TinhService { get; set; } = null!;

		public DbSet<Xa> Xa { get; set; } = null!;
		public DbSet<XaService> XaService { get; set; } = null!;

		public DbSet<NhaCC> NhaCC { get; set; } = null!;
		public DbSet<NhaCCService> NhaCCService { get; set; } = null!;

		public DbSet<KhachHang> KhachHang { get; set; } = null!;
		public DbSet<KhachHangService> KhachHangService { get; set; } = null!;
		public DbSet<KhachHangDetailView> KhachHangDetailView { get; set; }

		public DbSet<GianHang> GianHang { get; set; } = null!;
		public DbSet<GianHangService> GianHangService { get; set; } = null!;

		public DbSet<NhomSP> NhomSP { get; set; } = null!;
		public DbSet<NhomSPService> NhomSPService { get; set; } = null!;

		public DbSet<LoaiSP> LoaiSP { get; set; } = null!;
		public DbSet<LoaiSPService> LoaiSPService { get; set; } = null!;
		public DbSet<LoaiSPDto> LoaiSPDtos { get; set; }

		public DbSet<SanPham> SanPham { get; set; } = null!;
		public DbSet<SanPhamService> SanPhamService { get; set; } = null!;
		public DbSet<SanPhamDto> SanPhamDto { get; set; }

		public DbSet<DonMuaHang> DonMuaHang { get; set; } = null!;
		public DbSet<DonMuaHangDetail> DonMuaHangDetail { get; set; } = null!;
		public DbSet<DonMuaHangService> DonMuaHangService { get; set; } = null!;

		public DbSet<CTMH> CTMH { get; set; } = null!;
		public DbSet<CTMHService> CTMHService { get; set; } = null!;
		public DbSet<CTMHDetailDto> CTMHDetailDtos { get; set; }

		public DbSet<DonBanHang> DonBanHang { get; set; } = null!;
		public DbSet<DonBanHangService> DonBanHangService { get; set; } = null!;
		public DbSet<DonBanHangDetail> DonBanHangDetail { get; set; } = null!;

		public DbSet<CTBH> CTBH { get; set; } = null!;
		public DbSet<CTBHService> CTBHService { get; set; } = null!;
		public DbSet<CTBHDetailDto> CTBHDetailDtos { get; set; }

		//Kiểm tra tồn tại 
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

		}
	}
}
