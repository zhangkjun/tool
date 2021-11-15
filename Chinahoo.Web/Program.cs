using Chinahoo.Extensions.Context;
using FineUICore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.WebEncoders;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
services.Configure<WebEncoderOptions>(options =>
{
    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
});
services.AddDistributedMemoryCache();
services.AddSession();
services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// 配置请求参数限制
services.Configure<FormOptions>(x =>
{
    x.ValueCountLimit = 1024;   // 请求参数的个数限制（默认值：1024）
    x.ValueLengthLimit = 4194304;   // 单个请求参数值的长度限制（默认值：4194304 = 1024 * 1024 * 4）
});

// FineUI 服务
services.AddFineUI(builder.Configuration);

services.AddControllersWithViews().AddMvcOptions(options =>
{
    // 自定义模型绑定（Newtonsoft.Json）
    options.ModelBinderProviders.Insert(0, new JsonModelBinderProvider());
}).AddNewtonsoftJson();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthorization();

// FineUI 中间件（确保 UseFineUI 位于 UseEndpoints 的前面）
app.UseFineUI();
app.UseStaticHttpContext();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "area",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
