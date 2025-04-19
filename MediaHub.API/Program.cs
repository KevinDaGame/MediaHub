using System.IO.Abstractions;
using Auth0.AspNetCore.Authentication;
using MediaHub.API.Auth;
using MediaHub.API.Components;
using MediaHub.API.Service;
using MediaHub.DAL;
using MediaHub.DAL.Model;
using MediaHub.DAL.Repository;
using MediaHub.DAL.Services;
using MediaHub.DAL.Services.MediaPath;
using MediaHub.DAL.Services.Thumbnail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

string rootPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "media";
string thumbnailPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "/thumbnails";

string mysqlConnection = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCors",
        builder =>
        {
            builder.WithOrigins("*")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddAuthentication(options =>
{

    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
   
}).AddJwtBearer(options =>
{
    options.Authority = "https://dev-038ffj4bu8og0aq6.eu.auth0.com/";
    options.Audience = "https://mediahub.kevda.dev";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("read:media", policy => policy.Requirements.Add(new 
        HasScopeRequirement("read:media", "https://dev-038ffj4bu8og0aq6.eu.auth0.com/")));
});

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

builder.Services.AddDbContext<MediaHubDBContext>(options =>
{
    options.UseMySql(mysqlConnection, ServerVersion.AutoDetect(mysqlConnection));
});

using(var context = new MediaHubDBContext(new DbContextOptionsBuilder<MediaHubDBContext>().UseMySql(mysqlConnection, ServerVersion.AutoDetect(mysqlConnection)).Options))
{
    context.Database.EnsureCreated();
}

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//swagger with auth
builder.Services.AddSwaggerGen( );
AddServices(builder.Services);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

app.MapControllers();


app.MapGet("/Account/Login", async (HttpContext httpContext, string returnUrl = "/") =>
{
    var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
        .WithRedirectUri(returnUrl)
        .Build();

    await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});

app.MapGet("/Account/Logout", async (HttpContext httpContext) =>
{
    var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
        .WithRedirectUri("/")
        .Build();

    await httpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});

app.UseCors("AllowCors");

app.Run();
return;

void AddServices(IServiceCollection services)
{
    services.AddLogging();
    services.AddTransient<RootPathService, RootPathService>(_ => new RootPathService((AbsolutePath)rootPath));
    services.AddTransient<ThumbnailPathService, ThumbnailPathService>(_ => new ThumbnailPathService((AbsolutePath)thumbnailPath));
    services.AddTransient<IMediaService, MediaService>();
    services.AddTransient<IMediaThumbnailService, MediaThumbnailService>();
    services.AddTransient<ThumbnailContext, ThumbnailContext>();
    services.AddTransient<MediaRepository, MediaRepository>();
    services.AddTransient<IMediaDiscoveryService, MediaDiscoveryService>();
    
    services.AddSingleton<MediaFileSystemWatcherHostedService>();
    services.AddSingleton<IFileSystem, FileSystem>();
    
    services.AddHostedService<ThumbnailHostedService>();
    services.AddHostedService<MediaHostedService>();
    services.AddHostedService<MediaFileSystemWatcherHostedService>();
}
