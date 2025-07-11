using Microsoft.EntityFrameworkCore;
using zuHause.Models; // �� Scaffold �X�Ӫ� DbContext �R�W�Ŷ�

var builder = WebApplication.CreateBuilder(args);

// ? ���U Scaffold �X�Ӫ���Ʈw�s�u
builder.Services.AddDbContext<ZuHauseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ZuhauseDb")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// �����h�]�w
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// ? �w�]�������ѡG�ɦV FurnitureController �� FurnitureHomePage
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Furniture}/{action=FurnitureHomePage}/{id?}");

app.Run();
