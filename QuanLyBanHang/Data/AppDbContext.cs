using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.Services
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
		}

		// ========== TABLES (ENTITY thật) ==========
		public DbSet<Tinh> Tinh { get; set; } = null!;
		public DbSet<Xa> Xa { get; set; } = null!;
		public DbSet<Nuoc> Nuoc { get; set; } = null!;
		public DbSet<HangSX> Hang { get; set; } = null!;
		public DbSet<NhaCC> NhaCC { get; set; } = null!;
		public DbSet<KhachHang> KhachHang { get; set; } = null!;
		public DbSet<VaiTro> VaiTro { get; set; } = null!;
		public DbSet<NhanVien> NhanVien { get; set; } = null!;
		public DbSet<PhanQuyen> PhanQuyen { get; set; } = null!;
		public DbSet<NhomSP> NhomSP { get; set; } = null!;
		public DbSet<LoaiSP> LoaiSP { get; set; } = null!;
		public DbSet<TrangThai> TrangThai { get; set; } = null!;
		public DbSet<TrangThaiBH> TrangThaiBH { get; set; } = null!;
		public DbSet<TrangThaiMH> TrangThaiMH { get; set; } = null!;
		public DbSet<SanPham> SanPham { get; set; } = null!;
		public DbSet<DonMuaHang> DonMuaHang { get; set; } = null!;
		public DbSet<DonMuaHangDetail> DonMuaHangDetail { get; set; } = null!;
		public DbSet<CTMH> CTMH { get; set; } = null!;
		public DbSet<DonBanHang> DonBanHang { get; set; } = null!;
		public DbSet<DonBanHangDetail> DonBanHangDetail { get; set; } = null!;
		public DbSet<CTBH> CTBH { get; set; } = null!;

		public DbSet<NuocDto> NuocDto { get; set; } = null!;
		public DbSet<NhomSPDto> NhomSPDto { get; set; } = null!;
		public DbSet<LoaiSPDto> LoaiSPDtos { get; set; } = null!;
		public DbSet<SanPhamDto> SanPhamDto { get; set; } = null!;
		public DbSet<CTMHDetailDto> CTMHDetailDtos { get; set; } = null!;
		public DbSet<CTBHDetailDto> CTBHDetailDtos { get; set; } = null!;
		public DbSet<DonBanHangDetailDto> DonBanHangDetailDto { get; set; } = null!;
		public DbSet<XaDTO> XaDTO { get; set; } = null!;
		public DbSet<PhanQuyenDto> PhanQuyenDto { get; set; } = null!;
		public DbSet<DashboardStats> DashboardStats { get; set; } = null!;
		public DbSet<MonthlyRevenueData> MonthlyRevenueData { get; set; } = null!;
		public DbSet<TopProductData> TopProductData { get; set; } = null!;
		public DbSet<OrderDetailReport> OrderDetailReport { get; set; } = null!;
		public DbSet<ImportOrderDetailReport> ImportOrderDetailReport { get; set; } = null!;
		public DbSet<ProductRevenueReport> ProductRevenueReport { get; set; } = null!;
		


		// ========== SQL VIEW ==========
		public DbSet<NhaCCDetailView> NhaCCDetailView { get; set; } = null!;
		public DbSet<KhachHangDetailView> KhachHangDetailView { get; set; } = null!;
		public DbSet<NhanVienDetailView> NhanVienDetailView { get; set; } = null!;


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// ====== Cấu hình khóa chính phức hợp ======
			modelBuilder.Entity<CTMH>()
				.HasKey(ct => new { ct.MaDMH, ct.MaSP });

			modelBuilder.Entity<CTBH>()
				.HasKey(ct => new { ct.MaDBH, ct.MaSP });

			modelBuilder.Entity<PhanQuyen>()
				.HasKey(pq => new { pq.MaVT, pq.MaNV });

			// ====== DTOs: Keyless entities (cho stored procedures) ======
			modelBuilder.Entity<NuocDto>().HasNoKey();
			modelBuilder.Entity<NhomSPDto>().HasNoKey();
			modelBuilder.Entity<LoaiSPDto>().HasNoKey();
			modelBuilder.Entity<SanPhamDto>().HasNoKey();
			modelBuilder.Entity<CTMHDetailDto>().HasNoKey();
			modelBuilder.Entity<CTBHDetailDto>().HasNoKey();
			modelBuilder.Entity<PhanQuyenDto>().HasNoKey();
			modelBuilder.Entity<DashboardStats>().HasNoKey();
			modelBuilder.Entity<MonthlyRevenueData>().HasNoKey();
			modelBuilder.Entity<TopProductData>().HasNoKey();
			modelBuilder.Entity<OrderDetailReport>().HasNoKey();
			modelBuilder.Entity<ImportOrderDetailReport>().HasNoKey();
			modelBuilder.Entity<ProductRevenueReport>().HasNoKey();



		}
	}
}
