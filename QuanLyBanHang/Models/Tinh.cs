using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class Tinh
	{
		[Key]
		[Required(ErrorMessage = "Mã tỉnh không được để trống")]
		[StringLength(2, ErrorMessage = "Mã tỉnh tối đa 2 ký tự")]
		[Display(Name = "Mã tỉnh")]
		public string MaTinh { get; set; } = string.Empty;

		[Required(ErrorMessage = "Tên tỉnh không được để trống")]
		[StringLength(90, ErrorMessage = "Tên tỉnh không được quá 90 ký tự")]
		[Display(Name = "Tên tỉnh")]
		public string TenTinh { get; set; } = string.Empty;

	}
}
