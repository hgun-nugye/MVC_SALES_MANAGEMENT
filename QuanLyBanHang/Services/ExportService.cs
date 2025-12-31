using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace QuanLyBanHang.Services
{
	public class ExportService
	{
		public byte[] ExportDashboardToExcel(
			DashboardStats stats,
			List<MonthlyRevenueData> monthlyRevenue,
			List<OrderDetailReport> orders,
			List<ImportOrderDetailReport> importOrders,
			List<ProductRevenueReport> productRevenue,
			List<TopProductData> topProducts,
			List<TopProductData> slowProducts)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

			using (var package = new ExcelPackage())
			{
				// Sheet 1: Tổng quan
				CreateOverviewSheet(package.Workbook.Worksheets.Add("Tổng quan"), stats);

				// Sheet 2: Doanh thu theo tháng
				CreateMonthlyRevenueSheet(package.Workbook.Worksheets.Add("Doanh thu tháng"), monthlyRevenue);

				// Sheet 3: Chi tiết hóa đơn bán
				CreateOrderDetailsSheet(package.Workbook.Worksheets.Add("Chi tiết hóa đơn bán hàng"), orders);

				// Sheet 4: Chi tiết hóa đơn nhập
				CreateImportOrderDetailsSheet(package.Workbook.Worksheets.Add("Chi tiết hóa đơn nhập hàng"), importOrders);

				// Sheet 5: Doanh thu theo sản phẩm
				CreateProductRevenueSheet(package.Workbook.Worksheets.Add("Doanh thu sản phẩm"), productRevenue);

				// Sheet 6: Sản phẩm bán chạy
				CreateTopProductsSheet(package.Workbook.Worksheets.Add("Sản phẩm bán chạy"), topProducts);

				// Sheet 7: Sản phẩm ít bán
				CreateSlowProductsSheet(package.Workbook.Worksheets.Add("Sản phẩm ít bán"), slowProducts);

				return package.GetAsByteArray();
			}
		}

		private void CreateOverviewSheet(ExcelWorksheet ws, DashboardStats stats)
		{
			// Title
			ws.Cells["A1"].Value = "BÁO CÁO TỔNG QUAN HỆ THỐNG";
			ws.Cells["A1:B1"].Merge = true;
			ws.Cells["A1"].Style.Font.Size = 16;
			ws.Cells["A1"].Style.Font.Bold = true;
			ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(76, 175, 80));
			ws.Cells["A1"].Style.Font.Color.SetColor(Color.White);

			ws.Cells["A2"].Value = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}";
			ws.Cells["A2:B2"].Merge = true;
			ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

			// Headers
			int row = 4;
			ws.Cells[row, 1].Value = "Chỉ số";
			ws.Cells[row, 2].Value = "Giá trị";
			ws.Cells[row, 1, row, 2].Style.Font.Bold = true;
			ws.Cells[row, 1, row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells[row, 1, row, 2].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

			// Data
			row++;
			ws.Cells[row, 1].Value = "Tổng số sản phẩm";
			ws.Cells[row, 2].Value = stats.TotalProducts;
			row++;
			ws.Cells[row, 1].Value = "Tổng số khách hàng";
			ws.Cells[row, 2].Value = stats.TotalCustomers;
			row++;
			ws.Cells[row, 1].Value = "Tổng số nhân viên";
			ws.Cells[row, 2].Value = stats.TotalEmployees;
			row++;
			ws.Cells[row, 1].Value = "Tổng đơn bán hàng";
			ws.Cells[row, 2].Value = stats.TotalOrders;
			row++;
			ws.Cells[row, 1].Value = "Tổng đơn mua hàng";
			ws.Cells[row, 2].Value = stats.TotalPurchaseOrders;
			row++;
			ws.Cells[row, 1].Value = "Doanh thu tháng này (VNĐ)";
			ws.Cells[row, 2].Value = stats.MonthlyRevenue;
			ws.Cells[row, 2].Style.Numberformat.Format = "#,##0";
			row++;
			ws.Cells[row, 1].Value = "Doanh thu năm nay (VNĐ)";
			ws.Cells[row, 2].Value = stats.YearlyRevenue;
			ws.Cells[row, 2].Style.Numberformat.Format = "#,##0";
			row++;
			ws.Cells[row, 1].Value = "Đơn hàng tháng này";
			ws.Cells[row, 2].Value = stats.MonthlyOrders;

			ws.Cells.AutoFitColumns();
		}

		private void CreateMonthlyRevenueSheet(ExcelWorksheet ws, List<MonthlyRevenueData> data)
		{
			// Title
			ws.Cells["A1"].Value = "DOANH THU THEO THÁNG";
			ws.Cells["A1:D1"].Merge = true;
			ws.Cells["A1"].Style.Font.Size = 14;
			ws.Cells["A1"].Style.Font.Bold = true;
			ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(33, 150, 243));
			ws.Cells["A1"].Style.Font.Color.SetColor(Color.White);

			// Headers
			int row = 3;
			ws.Cells[row, 1].Value = "Tháng";
			ws.Cells[row, 2].Value = "Tên tháng";
			ws.Cells[row, 3].Value = "Số đơn hàng";
			ws.Cells[row, 4].Value = "Doanh thu (VNĐ)";
			ws.Cells[row, 1, row, 4].Style.Font.Bold = true;
			ws.Cells[row, 1, row, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells[row, 1, row, 4].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

			// Data
			row++;
			foreach (var item in data)
			{
				ws.Cells[row, 1].Value = item.Month;
				ws.Cells[row, 2].Value = item.MonthName;
				ws.Cells[row, 3].Value = item.OrderCount;
				ws.Cells[row, 4].Value = item.Revenue;
				ws.Cells[row, 4].Style.Numberformat.Format = "#,##0";
				row++;
			}

			// Total
			ws.Cells[row, 1].Value = "TỔNG";
			ws.Cells[row, 1, row, 2].Merge = true;
			ws.Cells[row, 3].Formula = $"SUM(C4:C{row - 1})";
			ws.Cells[row, 4].Formula = $"SUM(D4:D{row - 1})";
			ws.Cells[row, 1, row, 4].Style.Font.Bold = true;
			ws.Cells[row, 4].Style.Numberformat.Format = "#,##0";

			ws.Cells.AutoFitColumns();
		}

		private void CreateOrderDetailsSheet(ExcelWorksheet ws, List<OrderDetailReport> orders)
		{
			// Title
			ws.Cells["A1"].Value = "CHI TIẾT HÓA ĐƠN BÁN HÀNG";
			ws.Cells["A1:G1"].Merge = true;
			ws.Cells["A1"].Style.Font.Size = 14;
			ws.Cells["A1"].Style.Font.Bold = true;
			ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(76, 175, 80));
			ws.Cells["A1"].Style.Font.Color.SetColor(Color.White);

			// Headers
			int row = 3;
			ws.Cells[row, 1].Value = "Mã đơn hàng";
			ws.Cells[row, 2].Value = "Ngày bán";
			ws.Cells[row, 3].Value = "Khách hàng";
			ws.Cells[row, 4].Value = "Địa chỉ";
			ws.Cells[row, 5].Value = "Trạng thái";
			ws.Cells[row, 6].Value = "Số SP";
			ws.Cells[row, 7].Value = "Tổng tiền (VNĐ)";
			ws.Cells[row, 1, row, 7].Style.Font.Bold = true;
			ws.Cells[row, 1, row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells[row, 1, row, 7].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);

			// Data
			row++;
			foreach (var order in orders)
			{
				ws.Cells[row, 1].Value = order.MaDBH;
				ws.Cells[row, 2].Value = order.NgayBH.ToString("dd/MM/yyyy");
				ws.Cells[row, 3].Value = order.TenKH;
				ws.Cells[row, 4].Value = order.DiaChiDBH;
				ws.Cells[row, 5].Value = order.TrangThai;
				ws.Cells[row, 6].Value = order.SoLuongSP;
				ws.Cells[row, 7].Value = order.TongTien;
				ws.Cells[row, 7].Style.Numberformat.Format = "#,##0";
				row++;
			}

			// Total
			if (orders.Any())
			{
				ws.Cells[row, 1].Value = "TỔNG CỘNG";
				ws.Cells[row, 1, row, 6].Merge = true;
				ws.Cells[row, 7].Formula = $"SUM(G4:G{row - 1})";
				ws.Cells[row, 1, row, 7].Style.Font.Bold = true;
				ws.Cells[row, 7].Style.Numberformat.Format = "#,##0";
			}

			ws.Cells.AutoFitColumns();
		}

		private void CreateImportOrderDetailsSheet(	ExcelWorksheet ws, List<ImportOrderDetailReport> orders)
		{
			ws.Cells["A1"].Value = "CHI TIẾT HÓA ĐƠN NHẬP HÀNG";
			ws.Cells["A1:E1"].Merge = true;
			ws.Cells["A1"].Style.Font.Size = 14;
			ws.Cells["A1"].Style.Font.Bold = true;
			ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(244, 67, 54));
			ws.Cells["A1"].Style.Font.Color.SetColor(Color.White);

			int row = 3;
			ws.Cells[row, 1].Value = "Mã đơn nhập";
			ws.Cells[row, 2].Value = "Ngày nhập";
			ws.Cells[row, 3].Value = "Nhà cung cấp";
			ws.Cells[row, 4].Value = "Số lượng SP";
			ws.Cells[row, 5].Value = "Tổng tiền (VNĐ)";
			ws.Cells[row, 1, row, 5].Style.Font.Bold = true;
			ws.Cells[row, 1, row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells[row, 1, row, 5].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 205, 210));

			row++;
			foreach (var item in orders)
			{
				ws.Cells[row, 1].Value = item.MaDMH;
				ws.Cells[row, 2].Value = item.NgayMH;
				ws.Cells[row, 2].Style.Numberformat.Format = "dd/MM/yyyy";
				ws.Cells[row, 3].Value = item.TenNCC;
				ws.Cells[row, 4].Value = item.SoLuongSP;
				ws.Cells[row, 5].Value = item.TongTien;
				ws.Cells[row, 5].Style.Numberformat.Format = "#,##0";
				row++;
			}

			if (orders.Any())
			{
				ws.Cells[row, 1].Value = "TỔNG CỘNG";
				ws.Cells[row, 1, row, 4].Merge = true;
				ws.Cells[row, 5].Formula = $"SUM(E4:E{row - 1})";
				ws.Cells[row, 1, row, 5].Style.Font.Bold = true;
				ws.Cells[row, 5].Style.Numberformat.Format = "#,##0";
			}

			ws.Cells.AutoFitColumns();
		}

		private void CreateProductRevenueSheet(ExcelWorksheet ws, List<ProductRevenueReport> products)
		{
			// Title
			ws.Cells["A1"].Value = "DOANH THU THEO SẢN PHẨM";
			ws.Cells["A1:F1"].Merge = true;
			ws.Cells["A1"].Style.Font.Size = 14;
			ws.Cells["A1"].Style.Font.Bold = true;
			ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 152, 0));
			ws.Cells["A1"].Style.Font.Color.SetColor(Color.White);

			// Headers
			int row = 3;
			ws.Cells[row, 1].Value = "Mã SP";
			ws.Cells[row, 2].Value = "Tên sản phẩm";
			ws.Cells[row, 3].Value = "Giá bán";
			ws.Cells[row, 4].Value = "Số lượng bán";
			ws.Cells[row, 5].Value = "Số đơn hàng";
			ws.Cells[row, 6].Value = "Doanh thu (VNĐ)";
			ws.Cells[row, 1, row, 6].Style.Font.Bold = true;
			ws.Cells[row, 1, row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells[row, 1, row, 6].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 224, 178));

			// Data
			row++;
			foreach (var product in products)
			{
				ws.Cells[row, 1].Value = product.MaSP;
				ws.Cells[row, 2].Value = product.TenSP;
				ws.Cells[row, 3].Value = product.GiaBan;
				ws.Cells[row, 3].Style.Numberformat.Format = "#,##0";
				ws.Cells[row, 4].Value = product.SoLuongBan;
				ws.Cells[row, 5].Value = product.SoDonHang;
				ws.Cells[row, 6].Value = product.DoanhThu;
				ws.Cells[row, 6].Style.Numberformat.Format = "#,##0";
				row++;
			}

			// Total
			if (products.Any())
			{
				ws.Cells[row, 1].Value = "TỔNG CỘNG";
				ws.Cells[row, 1, row, 5].Merge = true;
				ws.Cells[row, 6].Formula = $"SUM(F4:F{row - 1})";
				ws.Cells[row, 1, row, 6].Style.Font.Bold = true;
				ws.Cells[row, 6].Style.Numberformat.Format = "#,##0";
			}

			ws.Cells.AutoFitColumns();
		}

		private void CreateTopProductsSheet(ExcelWorksheet ws, List<TopProductData> products)
		{
			// Title
			ws.Cells["A1"].Value = "TOP SẢN PHẨM BÁN CHẠY";
			ws.Cells["A1:E1"].Merge = true;
			ws.Cells["A1"].Style.Font.Size = 14;
			ws.Cells["A1"].Style.Font.Bold = true;
			ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(244, 67, 54));
			ws.Cells["A1"].Style.Font.Color.SetColor(Color.White);

			// Headers
			int row = 3;
			ws.Cells[row, 1].Value = "STT";
			ws.Cells[row, 2].Value = "Mã SP";
			ws.Cells[row, 3].Value = "Tên sản phẩm";
			ws.Cells[row, 4].Value = "Đã bán";
			ws.Cells[row, 5].Value = "Doanh thu (VNĐ)";
			ws.Cells[row, 1, row, 5].Style.Font.Bold = true;
			ws.Cells[row, 1, row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells[row, 1, row, 5].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 205, 210));

			// Data
			row++;
			int stt = 1;
			foreach (var product in products)
			{
				ws.Cells[row, 1].Value = stt++;
				ws.Cells[row, 2].Value = product.MaSP;
				ws.Cells[row, 3].Value = product.TenSP;
				ws.Cells[row, 4].Value = product.TotalQuantitySold;
				ws.Cells[row, 5].Value = product.TotalRevenue;
				ws.Cells[row, 5].Style.Numberformat.Format = "#,##0";
				row++;
			}

			ws.Cells.AutoFitColumns();
		}

		private void CreateSlowProductsSheet(ExcelWorksheet ws, List<TopProductData> products)
		{
			// Title
			ws.Cells["A1"].Value = "SẢN PHẨM ÍT BÁN CHẠY";
			ws.Cells["A1:E1"].Merge = true;
			ws.Cells["A1"].Style.Font.Size = 14;
			ws.Cells["A1"].Style.Font.Bold = true;
			ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 152, 0));
			ws.Cells["A1"].Style.Font.Color.SetColor(Color.White);

			// Headers
			int row = 3;
			ws.Cells[row, 1].Value = "STT";
			ws.Cells[row, 2].Value = "Mã SP";
			ws.Cells[row, 3].Value = "Tên sản phẩm";
			ws.Cells[row, 4].Value = "Đã bán";
			ws.Cells[row, 5].Value = "Doanh thu (VNĐ)";
			ws.Cells[row, 1, row, 5].Style.Font.Bold = true;
			ws.Cells[row, 1, row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells[row, 1, row, 5].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 224, 178));

			// Data
			row++;
			int stt = 1;
			foreach (var product in products)
			{
				ws.Cells[row, 1].Value = stt++;
				ws.Cells[row, 2].Value = product.MaSP;
				ws.Cells[row, 3].Value = product.TenSP;
				ws.Cells[row, 4].Value = product.TotalQuantitySold;
				ws.Cells[row, 5].Value = product.TotalRevenue;
				ws.Cells[row, 5].Style.Numberformat.Format = "#,##0";
				row++;
			}

			ws.Cells.AutoFitColumns();
		}
	}
}
