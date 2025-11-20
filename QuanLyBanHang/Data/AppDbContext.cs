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
		public DbSet<NhaCC> NhaCC { get; set; } = null!;
		public DbSet<KhachHang> KhachHang { get; set; } = null!;
		public DbSet<GianHang> GianHang { get; set; } = null!;

		public DbSet<NhomSP> NhomSP { get; set; } = null!;
		public DbSet<LoaiSP> LoaiSP { get; set; } = null!;
		public DbSet<SanPham> SanPham { get; set; } = null!;
		public DbSet<DonMuaHang> DonMuaHang { get; set; } = null!;
		public DbSet<CTMH> CTMH { get; set; } = null!;
		public DbSet<DonBanHang> DonBanHang { get; set; } = null!;
		public DbSet<CTBH> CTBH { get; set; } = null!;
		public DbSet<KhuyenMai> KhuyenMai { get; set; } = null!;
		public DbSet<DanhGia> DanhGia { get; set; } = null!;
		public DbSet<TaiKhoan> TaiKhoan { get; set; } = null!;

		public DbSet<DonMuaHangDetail> DonMuaHangDetail { get; set; } = null!;
		public DbSet<DonBanHangDetail> DonBanHangDetail { get; set; } = null!;
		public DbSet<CTBHDetailDto> CTBHDetailDtos { get; set; }
		public DbSet<CTMHDetailDto> CTMHDetailDtos { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// ======== Khóa chính ========
			modelBuilder.Entity<NhaCC>().HasKey(n => n.MaNCC);
			modelBuilder.Entity<KhachHang>().HasKey(kh => kh.MaKH);
			modelBuilder.Entity<GianHang>().HasKey(g => g.MaGH);
			modelBuilder.Entity<NhomSP>().HasKey(nsp => nsp.MaNhom);
			modelBuilder.Entity<LoaiSP>().HasKey(lsp => lsp.MaLoai);
			modelBuilder.Entity<SanPham>().HasKey(sp => sp.MaSP);
			modelBuilder.Entity<DonMuaHang>().HasKey(d => d.MaDMH);
			modelBuilder.Entity<DonBanHang>().HasKey(d => d.MaDBH);
			modelBuilder.Entity<CTMH>().HasKey(ct => new { ct.MaDMH, ct.MaSP });
			modelBuilder.Entity<CTBH>().HasKey(ct => new { ct.MaDBH, ct.MaSP });
			modelBuilder.Entity<KhuyenMai>().HasKey(km => km.MaKM);
			modelBuilder.Entity<DanhGia>().HasKey(dg => dg.MaDG);
			modelBuilder.Entity<TaiKhoan>().HasKey(tk => tk.TenUser);

			modelBuilder.Entity<CTBHDetailDto>().HasNoKey();   // DTO không có khóa
			modelBuilder.Entity<CTMHDetailDto>().HasNoKey();   // DTO không có khóa


			// ======== Quan hệ: LoaiSP → NhomSP ========
			modelBuilder.Entity<LoaiSP>()
				.HasOne(lsp => lsp.NhomSP)
				.WithMany(nsp => nsp.LoaiSPs)
				.HasForeignKey(lsp => lsp.MaNhom)
				.OnDelete(DeleteBehavior.Cascade);

			// ======== Quan hệ: SanPham → LoaiSP ========
			modelBuilder.Entity<SanPham>()
				.HasOne(sp => sp.LoaiSP)
				.WithMany(lsp => lsp.SanPhams)
				.HasForeignKey(sp => sp.MaLoai)
				.OnDelete(DeleteBehavior.Cascade);

			// ======== Quan hệ: SanPham → NhaCC ========
			modelBuilder.Entity<SanPham>()
				.HasOne(sp => sp.NhaCC)
				.WithMany(ncc => ncc.SanPhams)
				.HasForeignKey(sp => sp.MaNCC)
				.OnDelete(DeleteBehavior.Cascade);

			// ======== Khóa ngoại: SanPham → GianHang ========
			modelBuilder.Entity<SanPham>()
				.HasOne(sp => sp.GianHang)
				.WithMany(g => g.DsSanPham)
				.HasForeignKey(sp => sp.MaGH)
				.OnDelete(DeleteBehavior.Cascade);

			// ======== Quan hệ: Đơn Mua Hàng → NhaCC ========
			modelBuilder.Entity<DonMuaHang>()
				.HasOne(d => d.NhaCC)
				.WithMany()
				.HasForeignKey(d => d.MaNCC)
				.OnDelete(DeleteBehavior.Cascade);

			// ======== Quan hệ: Đơn Bán Hàng → Khách Hàng ========
			modelBuilder.Entity<DonBanHang>()
				.HasOne(d => d.KhachHang)
				.WithMany()
				.HasForeignKey(d => d.MaKH)
				.OnDelete(DeleteBehavior.Cascade);

			// ======== Quan hệ: CTMH → DonMuaHang & SanPham ========
			modelBuilder.Entity<CTMH>()
				.HasOne(ct => ct.DonMuaHang)
				.WithMany()
				.HasForeignKey(ct => ct.MaDMH)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<CTMH>()
				.HasOne(ct => ct.SanPham)
				.WithMany()
				.HasForeignKey(ct => ct.MaSP)
				.OnDelete(DeleteBehavior.Cascade);

			 //======== Quan hệ: CTBH → DonBanHang & SanPham ========
			modelBuilder.Entity<CTBH>()
				.HasOne(ct => ct.DonBanHang)
				.WithMany()
				.HasForeignKey(ct => ct.MaDBH)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<CTBH>()
				.HasOne(ct => ct.SanPham)
				.WithMany()
				.HasForeignKey(ct => ct.MaSP)
				.OnDelete(DeleteBehavior.Cascade);

			// ======== Quan hệ: DanhGia → SanPham & KhachHang ========
			modelBuilder.Entity<DanhGia>()
				.HasOne(dg => dg.SanPham)
				.WithMany()
				.HasForeignKey(dg => dg.MaSP)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<DanhGia>()
				.HasOne(dg => dg.KhachHang)
				.WithMany()
				.HasForeignKey(dg => dg.MaKH)
				.OnDelete(DeleteBehavior.Cascade);

			// ======== Quan hệ: TaiKhoan → KhachHang (nếu có) ========
			modelBuilder.Entity<TaiKhoan>()
				.HasOne(tk => tk.KhachHang)
				.WithMany()
				.HasForeignKey(tk => tk.MaKH)
				.OnDelete(DeleteBehavior.SetNull);
		}
	}
}
