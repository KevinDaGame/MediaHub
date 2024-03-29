using MediaHub.API.Auth;
using MediaHub.DAL.FS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

string rootPath = Directory.GetCurrentDirectory() + "/media";


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
AddServices(builder.Services);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseCors("AllowCors");

app.Run();
return;

void AddServices(IServiceCollection services)
{
    services.AddTransient<IMediaService, MediaService>(provider => new MediaService(rootPath));
}