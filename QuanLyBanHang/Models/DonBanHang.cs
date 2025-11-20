using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class DonBanHang
	{
		[Key]
		[StringLength(11)]
		[Display(Name = "Mã đơn bán hàng")]
		public string? MaDBH { get; set; }

		[Required(ErrorMessage = "Ngày bán hàng không được để trống")]
		[Display(Name = "Ngày bán hàng")]
		[DataType(DataType.Date)]
		public DateTime NgayBH { get; set; }

		[StringLength(10)]
		[Display(Name = "Mã khách hàng")]
		public string? MaKH { get; set; }

		[StringLength(50)]
		[Display(Name = "Tên khách hàng")]
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
		public string MaDBH { get; set; }
		public DateTime NgayBH { get; set; }
		public string MaKH { get; set; }

		public List<DonBanHangDetail> ChiTiet { get; set; } = new();
	}

}
