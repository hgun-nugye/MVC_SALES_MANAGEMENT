using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("CTBH")]
	public class CTBH
	{
		[Key, Column(Order = 0)]
		public string? MaDBH { get; set; }

		[Key, Column(Order = 1)]
		public string? MaSP { get; set; }
				
		public int SLB { get; set; }
		public decimal DGB { get; set; }

		[ForeignKey("MaSP")]
		public SanPham? SanPham { get; set; }

		[ForeignKey("MaDBH")]
		public DonBanHang? DonBanHang { get; set; } = null;
		public string? TenSP { get; set; }

	}

	[Keyless]
	public class CTBHDetailDto
	{
		public string? MaDBH { get; set; }
		public string? MaSP { get; set; }
		public int SLB { get; set; }
		public decimal DGB { get; set; }
		public string? TenSP { get; set; }
	}


}
