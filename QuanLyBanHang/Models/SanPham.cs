using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class SanPham
	{
		[Key]
		public string? MaSP { get; set; }

		public string? TenSP { get; set; }

		[DataType(DataType.Currency)]
		public decimal? DonGia { get; set; }

		[DataType(DataType.Currency)]
		public decimal? GiaBan { get; set; }
	
		public string? MoTaSP { get; set; }

		public string? AnhMH { get; set; }
		public string? ThanhPhan { get; set; }
		public string? CongDung { get; set; }
		public string? HDSD { get; set; }
		public string? XuatXu { get; set; }
		public string? BaoQuan { get; set; }

		public string? TrangThai { get; set; }

		public int SoLuongTon { get; set; }

		public string? MaLoai { get; set; }

		public string? MaNCC { get; set; }

		public string? MaGH { get; set; } = null!;

		[ForeignKey("MaLoai")]
		public LoaiSP? LoaiSP { get; set; }

		[ForeignKey("MaNCC")]
		public NhaCC? NhaCC { get; set; }

		[ForeignKey(nameof(MaGH))]
		public GianHang? GianHang { get; set; }

		public virtual ICollection<CTMH>? CTMHs { get; set; }
		public virtual ICollection<CTBH>? CTBHs { get; set; }

		[NotMapped]
		public string? TenGH { get; set; }
		[NotMapped]
		public string? TenLoai { get; set; }
		[NotMapped]
		public string? TenNCC { get; set; }

		[NotMapped]
		public IFormFile? AnhFile { get; set; }
	}

	[Keyless]
	public class SanPhamDto
	{
		public string? MaSP { get; set; }
		public string? TenSP { get; set; }
		public decimal? DonGia { get; set; }
		public decimal? GiaBan { get; set; }
		public string? MoTaSP { get; set; }
		public string? AnhMH { get; set; }
		public string? TrangThai { get; set; }
		public int? SoLuongTon { get; set; }
		public string? MaLoai { get; set; }
		public string? MaGH { get; set; }
		public string? MaNCC { get; set; }

		public string? TenLoai { get; set; }
		public string? TenGH { get; set; }
		public string? TenNCC { get; set; }
	}

}
