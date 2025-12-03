using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectedDb")));
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<CTMHService>();
builder.Services.AddScoped<CTBHService>();
builder.Services.AddScoped<DonMuaHangService>();
builder.Services.AddScoped<DonBanHangService>();
builder.Services.AddScoped<GianHangService>();
builder.Services.AddScoped<KhachHangService>();
builder.Services.AddScoped<LoaiSPService>();
builder.Services.AddScoped<NhaCCService>();
builder.Services.AddScoped<NhomSPService>();
builder.Services.AddScoped<SanPhamService>();
builder.Services.AddScoped<TinhService>();
builder.Services.AddScoped<XaService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
