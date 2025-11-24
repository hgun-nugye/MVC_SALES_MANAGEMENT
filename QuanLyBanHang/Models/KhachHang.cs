using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHang.Models
{
	public class KhachHang
	{
		[Key]
		[Display(Name = "Mã khách hàng")]
		public required string MaKH { get; set; }

		[Required, StringLength(50)]
		[Display(Name = "Tên khách hàng")]
		public required string TenKH { get; set; }

		[Required, StringLength(10)]
		[Display(Name = "Điện thoại")]
		public required string DienThoaiKH { get; set; }

		[EmailAddress]
		[Display(Name = "Email")]
		public required string EmailKH { get; set; }

		[Required, StringLength(255)]
		[Display(Name = "Địa chỉ")]
		public required string DiaChiKH { get; set; }
	}

	[Keyless]
	public class KhachHangCountDto
	{
		public int TotalRecords { get; set; }
	}
}
