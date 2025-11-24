using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class LoaiSP
	{
		[Key]
		[StringLength(10)]
		public string? MaLoai { get; set; } = null!;

		[Required]
		[StringLength(50)]
		public string? TenLoai { get; set; } = null!;

		// Khóa ngoại đến NhomSP
		[Required]
		[StringLength(10)]
		public string? MaNhom { get; set; } = null!;

		public string? TenNhom { get; set; }

		[ForeignKey(nameof(MaNhom))]
		public NhomSP? NhomSP { get; set; } = null;
		public ICollection<SanPham>? SanPhams { get; set; }

	}

	[Keyless]
	public class LoaiSPCountDto
	{
		public int TotalRecords { get; set; }
	}
}
