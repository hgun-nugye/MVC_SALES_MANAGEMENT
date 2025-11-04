using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHang.Models
{
	public class NhomSP
	{
		[Key]
		[Display(Name = "Mã nhóm sản phẩm")]
		public required string MaNhom { get; set; }

		[Required, StringLength(100)]
		[Display(Name = "Tên nhóm sản phẩm")]
		public required string TenNhom { get; set; }

		public ICollection<LoaiSP>? LoaiSPs { get; set; }

	}
}
