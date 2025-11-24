using Microsoft.AspNetCore.Mvc;
using QuanLyBanHang.Models;
using Newtonsoft.Json;

namespace QuanLyBanHang.Controllers
{
	public class AddressController : Controller
	{
		private readonly string apiUrlProvince = "https://provinces.open-api.vn/api/?depth=1";
		private readonly string apiUrlAddress = "https://provinces.open-api.vn/api/?depth=3";

		public async Task<ActionResult> LoadProvinces()
		{
			using (var client = new HttpClient())
			{
				var response = await client.GetStringAsync(apiUrlProvince);
				var provinces = JsonConvert.DeserializeObject<List<Province>>(response);
				return Json(provinces);
			}
		}

		public async Task<ActionResult> LoadAddress()
		{
			using (var client = new HttpClient())
			{
				var response = await client.GetStringAsync(apiUrlAddress);
				var provinces = JsonConvert.DeserializeObject<List<Province>>(response);
				return Json(provinces);
			}
		}
		public IActionResult Index()
		{
			return View();
		}
	}
}
