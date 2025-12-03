using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("CTMH")]
	public class CTMH
	{
		[Key, Column(Order = 0)]
		public string? MaDMH { get; set; }

		[Key, Column(Order = 1)]
		public string? MaSP { get; set; }

		public int SLM { get; set; }
		public decimal DGM { get; set; }

		[ForeignKey("MaDMH")]
		public virtual DonMuaHang? DonMuaHang { get; set; }

		[ForeignKey("MaSP")]
		public virtual SanPham? SanPham { get; set; }

		public string? TenSP { get; set; }
	}
	[Keyless]
	public class CTMHDetailDto
	{
		public string? MaDMH { get; set; }
		public string? MaSP { get; set; }
		public int SLM { get; set; }
		public decimal DGM { get; set; }
		public string? TenSP { get; set; }
	}
}
