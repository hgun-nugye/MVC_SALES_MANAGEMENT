using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHang.Models
{
	public class NhaCC
	{
		[Key]
		public string? MaNCC { get; set; }

		[Required, StringLength(100)]
		public string? TenNCC { get; set; }

		[Required, StringLength(15)]
		public string? DienThoaiNCC { get; set; }

		[EmailAddress]
		public string? EmailNCC { get; set; }

		[Required, StringLength(255)]
		public string? DiaChiNCC { get; set; }

		public ICollection<SanPham>? SanPhams { get; set; }

	}

	[Keyless]
	public class NhaCCCountDto
	{
		public int TotalRecords { get; set; }
	}
}
