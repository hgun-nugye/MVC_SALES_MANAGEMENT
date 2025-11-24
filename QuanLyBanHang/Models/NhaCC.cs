using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHang.Models
{
	public class NhaCC
	{
		[Key]
		[Display(Name = "Mã nhà cung cấp")]
		public required string MaNCC { get; set; }

		[Required, StringLength(100)]
		[Display(Name = "Tên nhà cung cấp")]
		public required string TenNCC { get; set; }

		[Required, StringLength(15)]
		[Display(Name = "Điện thoại")]
		public required string DienThoaiNCC { get; set; }

		[EmailAddress]
		[Display(Name = "Email")]
		public string? EmailNCC { get; set; }

		[Required, StringLength(255)]
		[Display(Name = "Địa chỉ")]
		public required string DiaChiNCC { get; set; }

		public ICollection<SanPham>? SanPhams { get; set; }

	}

	[Keyless]
	public class NhaCCCountDto
	{
		public int TotalRecords { get; set; }
	}
}
