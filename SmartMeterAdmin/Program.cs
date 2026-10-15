using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SmartMeterAdmin.Infrastructure.Data;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Middleware;
using SmartMeterAdmin.Services.AuditLog;
using SmartMeterAdmin.Services.Dapper_ORM;
using SmartMeterAdmin.Services.DT;
using SmartMeterAdmin.Services.DTCSurvey;
using SmartMeterAdmin.Services.Feeder;
using SmartMeterAdmin.Services.HTCSurvey;
using SmartMeterAdmin.Services.Master;
using SmartMeterAdmin.Services.Meter;
using SmartMeterAdmin.Services.Production;
using SmartMeterAdmin.Services.Store;
using SmartMeterAdmin.Services.Survey;
using SmartMeterAdmin.Services.User;
using SmartmeterAdmin.Hangfire.SyncHesMeter;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    // Global audit log - har action ka log automatic (SkipAuditLog attribute se skip ho sakta hai)
    options.Filters.Add<AuditLogActionFilter>();
}).AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "smartmeter",
        Version = "v1",
    });

    // Ensure uniqueness of schema IDs
    options.CustomSchemaIds(type => type.FullName);

    // Exclude certain types from schema generation
    options.IgnoreObsoleteProperties();
});

//For Background Jobs
builder.Services.AddHangfire((sp, config) =>
{
    var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("DevConnection");
    config.UseSqlServerStorage(connectionString);
});
builder.Services.AddHangfireServer();
builder.Services.AddTransient<HesMeter>(); // MeterController sync push inject ke liye (Hangfire background job bhi isi class use karta hai)

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<IUser, Userservice>();
builder.Services.AddTransient<IMeter, MeterServices>();
builder.Services.AddTransient<IJsonResponseFromat, JsonResponseFromat>();
builder.Services.AddTransient<IMaster, MasterService>();
builder.Services.AddTransient<IBasicUtility, BasicUtility>();
builder.Services.AddScoped<IExportUtility, ExportUtility>();
builder.Services.AddScoped<IDapper, Dapper_ORM>();
builder.Services.AddScoped<IAuditLogger, AuditLogger>();
builder.Services.AddScoped<IProduction, ProductionService>();
builder.Services.AddScoped<IHTC, HTCService>();
builder.Services.AddScoped<IDT, DTService>();
builder.Services.AddScoped<IFeeder, FeederService>();
builder.Services.AddHttpClient<IStore, StoreService>();
builder.Services.AddScoped<IDTC, DTCService>();
builder.Services.AddScoped<ISurvey, SurveyService>();



// Configure database context
builder.Services.AddDbContext<SmartMeterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection"),
    providerOptions => providerOptions.EnableRetryOnFailure()));

// Configure CORS
string[] urls = builder.Configuration.GetSection("AllowSpecificOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigins",
        builder => builder.WithOrigins(urls)
            .AllowAnyMethod()
            .AllowAnyHeader());
});


// Configure JWT authentication
var key = Encoding.UTF8.GetBytes(builder.Configuration["ApplicationSettings:JWT_Secret"].ToString());

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// Configure application settings
builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
builder.Services.Configure<HesBaseConfig>(builder.Configuration.GetSection("HESBaseConfig"));


var app = builder.Build();

// Audit log: request start time record (duration calculate karne ke liye)
app.Use(async (context, next) =>
{
    context.Items["AuditLog_StartTime"] = DateTimeOffset.UtcNow;
    await next();
});

var supportedCultures = new[] { new System.Globalization.CultureInfo("en-IN") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-IN"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseCors("AllowOrigins");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSwagger();
app.UseSwaggerUI();

// Exception handling
app.UseExceptionHandler("/error");

app.UseStaticFiles();    //Serve files from wwwroot

// Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();
app.UseHangfireDashboard("/smartmeter/job-dashboard", new DashboardOptions
{
    DashboardTitle = "Smartmeter Background Jobs",
    DarkModeEnabled = false,
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter
        {
            User="Superadmin",
            Pass="P@ssword123"
        }
    }
});
// Add custom middleware
app.UseMiddleware<RequestAccessMiddleware>();
app.Run();
