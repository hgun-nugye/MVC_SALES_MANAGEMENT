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
			// Tắt lazy loading để tránh tự động load navigation properties
			//optionsBuilder.UseLazyLoadingProxies(false);
		}

		// ========== TABLES (ENTITY thật) ==========
		public DbSet<Tinh> Tinh { get; set; } = null!;
		public DbSet<Xa> Xa { get; set; } = null!;
		public DbSet<Nuoc> Nuoc { get; set; } = null!;
		public DbSet<Hang> Hang { get; set; } = null!;
		public DbSet<NhaCC> NhaCC { get; set; } = null!;
		public DbSet<KhachHang> KhachHang { get; set; } = null!;
		public DbSet<NhanVien> NhanVien { get; set; } = null!;
		public DbSet<NhomSP> NhomSP { get; set; } = null!;
		public DbSet<LoaiSP> LoaiSP { get; set; } = null!;
		public DbSet<SanPham> SanPham { get; set; } = null!;
		public DbSet<DonMuaHang> DonMuaHang { get; set; } = null!;
		public DbSet<DonMuaHangDetail> DonMuaHangDetail { get; set; } = null!;
		public DbSet<CTMH> CTMH { get; set; } = null!;
		public DbSet<DonBanHang> DonBanHang { get; set; } = null!;
		public DbSet<DonBanHangDetail> DonBanHangDetail { get; set; } = null!;
		public DbSet<CTBH> CTBH { get; set; } = null!;

		// ========== DTOs (Keyless entities cho stored procedures) ==========
		public DbSet<NuocDto> NuocDto { get; set; } = null!;
		public DbSet<NhomSPDto> NhomSPDto { get; set; } = null!;
		public DbSet<LoaiSPDto> LoaiSPDtos { get; set; } = null!;
		public DbSet<SanPhamDto> SanPhamDto { get; set; } = null!;
		public DbSet<CTMHDetailDto> CTMHDetailDtos { get; set; } = null!;
		public DbSet<CTBHDetailDto> CTBHDetailDtos { get; set; } = null!;

		// ========== SQL VIEW (nếu có) ==========
		public DbSet<NhaCCDetailView> NhaCCDetailView { get; set; } = null!;
		public DbSet<KhachHangDetailView> KhachHangDetailView { get; set; } = null!;


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// ====== Cấu hình khóa chính phức hợp ======
			modelBuilder.Entity<CTMH>()
				.HasKey(ct => new { ct.MaDMH, ct.MaSP });

			modelBuilder.Entity<CTBH>()
				.HasKey(ct => new { ct.MaDBH, ct.MaSP });

			// ====== DTOs: Keyless entities (cho stored procedures) ======
			modelBuilder.Entity<NuocDto>().HasNoKey();
			modelBuilder.Entity<NhomSPDto>().HasNoKey();
			modelBuilder.Entity<LoaiSPDto>().HasNoKey();
			modelBuilder.Entity<SanPhamDto>().HasNoKey();
			modelBuilder.Entity<CTMHDetailDto>().HasNoKey();
			modelBuilder.Entity<CTBHDetailDto>().HasNoKey();

			// ====== SQL VIEW: No Key + tên view ======
			modelBuilder.Entity<NhaCCDetailView>()
				.HasNoKey()
				.ToView("NhaCCDetailView");

			modelBuilder.Entity<KhachHangDetailView>()
				.HasNoKey()
				.ToView("KhachHangDetailView");

			//// ====== DonBanHangDetail là View/ResultSet không có khóa ======
			//modelBuilder.Entity<DonBanHangDetail>(entity =>
			//{
			//	entity.HasNoKey();
			//	entity.Ignore(e => e.TenTinh);
			//	entity.Ignore(e => e.TenXa);
			//	entity.Ignore(e => e.ThanhTien);
			//});

			//// ====== DonMuaHangDetail là View/ResultSet không có khóa ======
			//modelBuilder.Entity<DonMuaHangDetail>(entity =>
			//{
			//	entity.HasNoKey();
			//	entity.Ignore(e => e.ThanhTien);
			//});

			//// ====== DonBanHang: bỏ property hiển thị ======
			//modelBuilder.Entity<DonBanHang>(entity =>
			//{
			//	entity.Ignore(e => e.TenKH);
			//});

			// ====== DonMuaHang: cấu hình foreign key và bỏ property hiển thị ======
			//modelBuilder.Entity<DonMuaHang>(entity =>
			//{
			//	entity.Ignore(e => e.TenNCC);
			//	entity.Ignore(e => e.TenNV);
				
				//// Cấu hình foreign key rõ ràng để tránh shadow property
				//// Không sử dụng navigation property, chỉ dùng foreign key
				//entity.HasOne(e => e.NhaCC)
				//	.WithMany()
				//	.HasForeignKey(e => e.MaNCC)
				//	.OnDelete(DeleteBehavior.NoAction)
				//	.IsRequired(false);
				
				//entity.HasOne(e => e.NhanVien)
				//	.WithMany()
				//	.HasForeignKey(e => e.MaNV)
				//	.OnDelete(DeleteBehavior.NoAction)
				//	.IsRequired(false);
				
				// Tắt auto-include cho navigation properties
				//entity.Navigation(e => e.NhaCC).AutoInclude = false;
				//entity.Navigation(e => e.NhanVien).AutoInclude = false;
			//});

			// ====== NhaCC: cấu hình để tránh shadow property khi query ======
			//modelBuilder.Entity<NhaCC>(entity =>
			//{
			//	entity.Ignore(e => e.TenXa);
			//	entity.Ignore(e => e.MaTinh);
			//	entity.Ignore(e => e.TenTinh);
			//	// Tắt navigation property nếu không cần
			//	//entity.Navigation(e => e.SanPhams).AutoInclude = false;
			//});
		}
	}
}
