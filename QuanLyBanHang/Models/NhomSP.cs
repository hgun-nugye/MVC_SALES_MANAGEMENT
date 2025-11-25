using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHang.Models
{
	public class NhomSP
	{
		[Key]
		[Display(Name = "Mã nhóm sản phẩm")]
		public string? MaNhom { get; set; }

		[Required, StringLength(100)]
		[Display(Name = "Tên nhóm sản phẩm")]
		public string? TenNhom { get; set; }

		public ICollection<LoaiSP>? LoaiSPs { get; set; }

	}

	[Keyless]
	public class NhomSPCountDto
	{
		public int TotalRecords { get; set; }
	}
}
