using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("LoaiSP")]
	public class LoaiSP
	{
		[Key]
		[Required(ErrorMessage = "Mã loại sản phẩm không được để trống.")]
		[StringLength(10, ErrorMessage = "Mã loại sản phẩm tối đa 10 ký tự.")]
		[Display(Name = "Mã Loại Sản Phẩm")]
		public string MaLoai { get; set; } = string.Empty;

		[Required(ErrorMessage = "Tên loại sản phẩm không được để trống.")]
		[StringLength(50, ErrorMessage = "Tên loại sản phẩm tối đa 50 ký tự.")]
		[Display(Name = "Tên Loại Sản Phẩm")]
		public string TenLoai { get; set; } = string.Empty;

		// Khóa ngoại đến NhomSP
		[Required(ErrorMessage = "Mã nhóm sản phẩm không được để trống.")]
		[StringLength(10, ErrorMessage = "Mã nhóm sản phẩm tối đa 10 ký tự.")]
		[Display(Name = "Mã Nhóm Sản Phẩm")]
		public string MaNhom { get; set; } = string.Empty;

		[NotMapped]
		[Display(Name = "Tên Nhóm Sản Phẩm")]
		public string? TenNhom { get; set; }

		//[ForeignKey(nameof(MaNhom))]
		//[Display(Name = "Nhóm Sản Phẩm")]
		//public NhomSP? NhomSP { get; set; }

		//[Display(Name = "Danh Sách Sản Phẩm")]
		//public ICollection<SanPham>? SanPhams { get; set; }
	}

	[Keyless]
	public class LoaiSPDto
	{
		[Display(Name = "Mã Loại Sản Phẩm")]
		public string? MaLoai { get; set; }

		[Display(Name = "Tên Loại Sản Phẩm")]
		public string? TenLoai { get; set; }

		[Display(Name = "Mã Nhóm Sản Phẩm")]
		public string? MaNhom { get; set; }

		[Display(Name = "Tên Nhóm Sản Phẩm")]
		public string? TenNhom { get; set; }
	}
}
