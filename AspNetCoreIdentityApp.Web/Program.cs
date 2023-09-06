using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.EntityFrameworkCore;
using AspNetCoreIdentityApp.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityApp.Core.OptionsModels;
using AspNetCoreIdentityApp.Service.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication;
using AspNetCoreIdentityApp.Web.ClaimProviders;
using AspNetCoreIdentityApp.Web.Requirements;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IMemberService, MemberService>();



builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ViolenceRequirementHandler>();
//Add services to the container.
builder.Services.AddControllersWithViews();
//ClaimProviders þehir için ayrý bir cookie ekledik (cookieler claimlerden oluþur)
builder.Services.AddScoped<IClaimsTransformation,UserClaimProvider>();
//Claimlerimizdeki dinamik iþlermleri gerçekleþtirirken yetkilendirme gibi policy kullanýyoruz
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AntalyaPolicy", policy =>
    {
        policy.RequireClaim("city", "Antalya","Afyon");
    });

    options.AddPolicy("ExchangePolicy", policy =>
    {
        policy.AddRequirements(new ExchangeExpireRequirement());
    });
    options.AddPolicy("ViolencePolicy", policy =>
    {
        policy.AddRequirements(new ViolenceRequirement()
        {
            TresholdAge = 18
        });
    });
});

//Database için
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"), options =>
    {
        options.MigrationsAssembly("AspNetCoreIdentityApp.Repository");
    });
});

//Email'e mesaj yollamak için gerekli eklemeler
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

//Fiziksel dosya pathini seçiyoruz resim kýsmý
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
//Özel ex tension ekleme
builder.Services.AddIdentityWithExtension();

//Cookie ayarlarý
builder.Services.ConfigureApplicationCookie(options =>
{

    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "UdemyAppCookie";

    options.LoginPath = new PathString("/Home/SignIn");
    options.LogoutPath = new PathString("/Member/LogOut");
    options.AccessDeniedPath = new PathString("/Member/AccessDenied");

    options.ExpireTimeSpan = TimeSpan.FromDays(60);

    options.SlidingExpiration = true;

});


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


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
