using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class LoaiSP
	{
		[Key]
		[Display(Name = "Mã loại sản phẩm")]
		public required string MaLoai { get; set; }

		[Required, StringLength(50)]
		[Display(Name = "Tên loại sản phẩm")]
		public required	string TenLoai { get; set; }

		[Required]
		[Display(Name = "Mã nhóm sản phẩm")]
		public required	string MaNhom { get; set; }

		[ForeignKey("MaNhom")]
		[Display(Name = "Nhóm sản phẩm")]
		public NhomSP? NhomSP { get; set; }
		[Display(Name = "Sản phẩm")]
		public ICollection<SanPham>? SanPhams { get; set; }

	}
}
