using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class Tinh
	{
		[Key]
		[Display(Name = "Mã tỉnh")]
		public short MaTinh { get; set; }

		[Required(ErrorMessage = "Tên tỉnh không được để trống")]
		[StringLength(90, ErrorMessage = "Tên tỉnh không được quá 90 ký tự")]
		[Display(Name = "Tên tỉnh")]
		public string TenTinh { get; set; } = string.Empty;

	}
}
