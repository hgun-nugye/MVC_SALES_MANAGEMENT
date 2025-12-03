using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class DonMuaHang
	{
		[Key]
		[Display(Name = "Mã đơn mua hàng")]
		public string? MaDMH { get; set; }

		[Required(ErrorMessage = "Ngày mua hàng không được để trống")]
		[Display(Name = "Ngày mua hàng")]
		[DataType(DataType.Date)]
		public DateTime NgayMH { get; set; }

		[Display(Name = "Mã nhà cung cấp")]
		public string? MaNCC { get; set; }

		[ForeignKey("MaNCC")]
		public virtual NhaCC? NhaCC { get; set; }

		public virtual List<CTMH>? CTMHs { get; set; }

		public string? TenNCC { get; set; }

	}

	[Keyless] // vì không có khóa chính riêng
	public class DonMuaHangDetail
	{
		public string? MaDMH { get; set; }
		public DateTime NgayMH { get; set; }
		public string? MaNCC { get; set; }
		public string? TenNCC { get; set; }
		public string? MaSP { get; set; }
		public string? TenSP { get; set; }
		public int? SLM { get; set; }
		public decimal? DGM { get; set; }
	}

	public class DonMuaHangEditCTMH
	{
		public string? MaDMH { get; set; }
		public DateTime NgayMH { get; set; }
		public string? MaNCC { get; set; }

		public List<CTMH> ChiTiet { get; set; } = new();
	}

}
