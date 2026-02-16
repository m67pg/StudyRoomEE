using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StudyRoomEE;
using StudyRoomEE.Components;
using StudyRoomEE.Models;
using System;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// builder.Build() の前に追加
// 1. 設定情報(Configuration)を取得
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=StudyRoomDB;Trusted_Connection=True;MultipleActiveResultSets=true";

// 2. 取得した接続文字列を使ってDbContextを設定
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(connectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            // 一過性のエラーが発生した際に自動で再試行する
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,           // 最大5回リトライ
                maxRetryDelay: TimeSpan.FromSeconds(10), // リトライ間の最大待ち時間
                errorNumbersToAdd: null);   // 特定のエラー番号を追加したい場合はここに
        }));

// 認証・認可のサービス登録
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// 認可（Authorize属性）を有効にする
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// ログイン時にStudentIdをClaimとして登録する設定（簡易版）
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AdditionalUserClaimsPrincipalFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
}); 

app.Run();

public class AdditionalUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
{
    public AdditionalUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor) { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        // StudentIdをClaimに追加
        identity.AddClaim(new Claim("StudentId", user.StudentId.ToString()));
        return identity;
    }
}
