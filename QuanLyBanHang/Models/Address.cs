namespace QuanLyBanHang.Models
{
	public class Ward
	{
		public required string code { get; set; }
		public required string name { get; set; }
	}

	public class District
	{
		public required string code { get; set; }
		public required string name { get; set; }
		public List<Ward> wards { get; set; } = new List<Ward>();
	}

	public class Province
	{
		public required string code { get; set; }
		public required string name { get; set; }
		public List<District> districts { get; set; } = new List<District>();
	}
}
