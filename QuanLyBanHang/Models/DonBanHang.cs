using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class DonBanHang
	{
		[Key]
		public string? MaDBH { get; set; }

		[DataType(DataType.Date)]
		public DateTime NgayBH { get; set; }

		public string? MaKH { get; set; }

		public string? TenKH { get; set; }


		[ForeignKey("MaKH")]
		public KhachHang? KhachHang { get; set; }

		public List<CTBH>? CTBHs { get; set; }

	}
	[Keyless] // vì không có khóa chính riêng
	public class DonBanHangDetail
	{
		public string? MaDBH { get; set; }
		public DateTime NgayBH { get; set; }
		public string? MaKH { get; set; }
		public string? TenKH { get; set; }
		public string? MaSP { get; set; }
		public string? TenSP { get; set; }
		public int? SLB { get; set; }
		public decimal? DGB { get; set; }
	}

	public class DonBanHangEditCTBH
	{
		public string? MaDBH { get; set; }
		public DateTime NgayBH { get; set; }
		public string? MaKH { get; set; }

		public List<CTBH> ChiTiet { get; set; } = new();
	}

	
}
