using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHang.Models
{
	public class NhomSP
	{
		[Key]
		public string? MaNhom { get; set; }

		public string TenNhom { get; set; }

		public ICollection<LoaiSP>? LoaiSPs { get; set; }

	}

	[Keyless]
	public class NhomSPCountDto
	{
		public int TotalRecords { get; set; }
	}
}
