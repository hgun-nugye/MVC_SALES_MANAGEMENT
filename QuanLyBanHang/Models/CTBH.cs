using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("CTBH")]
	public class CTBH
	{
		[Key, Column(Order = 0)]
		[Display(Name = "Mã đơn bán hàng")]
		public string? MaDBH { get; set; }

		[Key, Column(Order = 1)]
		[Display(Name = "Mã sản phẩm")]
		public string? MaSP { get; set; }

		[Required(ErrorMessage = "Số lượng bán không được để trống")]
		[Display(Name = "Số lượng bán")]
		[Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
		public int SLB { get; set; }

		[Required(ErrorMessage = "Đơn giá bán không được để trống")]
		[Display(Name = "Đơn giá bán")]
		[Column(TypeName = "money")]
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
		public string MaDBH { get; set; }
		public string MaSP { get; set; }
		public int SLB { get; set; }
		public decimal DGB { get; set; }
		public string TenSP { get; set; }
	}


}
