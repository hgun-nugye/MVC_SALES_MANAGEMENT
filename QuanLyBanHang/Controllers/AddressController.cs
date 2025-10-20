using Microsoft.AspNetCore.Mvc;
using QuanLyBanHang.Models;
using Newtonsoft.Json;

namespace QuanLyBanHang.Controllers
{
	public class AddressController : Controller
	{
		private readonly string apiUrl = "https://provinces.open-api.vn/api/?depth=3";

		public async Task<ActionResult> LoadProvinces()
		{
			using (var client = new HttpClient())
			{
				var response = await client.GetStringAsync(apiUrl);
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
