using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("TrangThaiBH")]
	public class TrangThaiBH
	{
		[Key]
		[Required(ErrorMessage = "Mã trạng thái không được để trống.")]
		[StringLength(3, ErrorMessage = "Mã trạng thái tối đa 3 ký tự.")]
		[Display(Name = "Mã Trạng Thái")]
		public string MaTTBH { get; set; } = string.Empty;

		[Required(ErrorMessage = "Tên trạng thái không được để trống.")]
		[StringLength(50, ErrorMessage = "Tên trạng thái tối đa 50 ký tự.")]
		[Display(Name = "Tên Trạng Thái")]
		public string TenTTBH { get; set; } = string.Empty;
	}
}

