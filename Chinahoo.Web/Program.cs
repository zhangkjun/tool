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
// ���������������
services.Configure<FormOptions>(x =>
{
    x.ValueCountLimit = 1024;   // ��������ĸ������ƣ�Ĭ��ֵ��1024��
    x.ValueLengthLimit = 4194304;   // �����������ֵ�ĳ������ƣ�Ĭ��ֵ��4194304 = 1024 * 1024 * 4��
});

// FineUI ����
services.AddFineUI(builder.Configuration);

services.AddControllersWithViews().AddMvcOptions(options =>
{
    // �Զ���ģ�Ͱ󶨣�Newtonsoft.Json��
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

// FineUI �м����ȷ�� UseFineUI λ�� UseEndpoints ��ǰ�棩
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
