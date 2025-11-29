using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHang.Models
{
	public class KhachHang
	{
		[Key]
		public required string MaKH { get; set; }

		[Required]
		public required string TenKH { get; set; }

		[Required]
		public required string DienThoaiKH { get; set; }

		[EmailAddress]
		public required string EmailKH { get; set; }

		[Required]
		public required string DiaChiKH { get; set; }
	}

	[Keyless]
	public class KhachHangCountDto
	{
		public int TotalRecords { get; set; }
	}
}
