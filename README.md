``**CALL API TỈNH/THÀNH/XÃ VIỆT NAM**

🔹 Bước 1: Tạo các class Model để nhận dữ liệu JSON

Tạo folder Models\DiaChiModels.cs:

public class Ward
{
    public string code { get; set; }
    public string name { get; set; }
}

public class District
{
    public string code { get; set; }
    public string name { get; set; }
    public List<Ward> wards { get; set; }
}

public class Province
{
    public string code { get; set; }
    public string name { get; set; }
    public List<District> districts { get; set; }
}

🔹 Bước 2: Gọi API trong Controller

Tạo controller mới DiaChiController.cs:

using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;  // nếu dùng .NET Framework MVC

public class DiaChiController : Controller
{
    private readonly string apiUrl = "https://provinces.open-api.vn/api/?depth=3";

    public async Task<ActionResult> LoadProvinces()
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetStringAsync(apiUrl);
            var provinces = JsonConvert.DeserializeObject<List<Province>>(response);
            return Json(provinces, JsonRequestBehavior.AllowGet);
        }
    }
}


🧠 Ghi chú:

JsonConvert thuộc thư viện Newtonsoft.Json → nhớ cài qua NuGet:

Install-Package Newtonsoft.Json

🔹 Bước 3: Tạo View (VD: Views/DiaChi/Index.cshtml)
@{
    ViewBag.Title = "Chọn địa chỉ";
}

<h3>Chọn địa chỉ Việt Nam</h3>

<select id="province" class="form-select">
    <option value="">-- Chọn Tỉnh/Thành phố --</option>
</select>

<select id="district" class="form-select mt-2">
    <option value="">-- Chọn Quận/Huyện --</option>
</select>

<select id="ward" class="form-select mt-2">
    <option value="">-- Chọn Phường/Xã --</option>
</select>

@section scripts {
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script>
    let provinceData = [];

    // Gọi API lấy toàn bộ dữ liệu tỉnh/thành/huyện/xã
    $(document).ready(function () {
        $.getJSON('@Url.Action("LoadProvinces", "DiaChi")', function (data) {
            provinceData = data;
            data.forEach(p => {
                $('#province').append(`<option value="${p.code}">${p.name}</option>`);
            });
        });

        // Khi chọn tỉnh → load huyện
        $('#province').on('change', function () {
            const selected = $(this).val();
            $('#district').html('<option value="">-- Chọn Quận/Huyện --</option>');
            $('#ward').html('<option value="">-- Chọn Phường/Xã --</option>');

            const province = provinceData.find(p => p.code === selected);
            if (province) {
                province.districts.forEach(d => {
                    $('#district').append(`<option value="${d.code}">${d.name}</option>`);
                });
            }
        });

        // Khi chọn huyện → load xã
        $('#district').on('change', function () {
            const selectedDistrict = $(this).val();
            const province = provinceData.find(p => p.code === $('#province').val());
            const district = province.districts.find(d => d.code === selectedDistrict);

            $('#ward').html('<option value="">-- Chọn Phường/Xã --</option>');
            if (district) {
                district.wards.forEach(w => {
                    $('#ward').append(`<option value="${w.code}">${w.name}</option>`);
                });
            }
        });
    });
</script>
}

🧠 Mở rộng (nếu bạn muốn làm chuyên sâu)

Bạn có thể:

Lưu dữ liệu này vào SQL Server bằng cách gọi API 1 lần → parse → INSERT INTO.

Hoặc cache tạm (Redis / file JSON) để không phải gọi API mỗi lần load web.
