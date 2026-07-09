using FitCore.API.Middlewares;
using FitCore.BLL.Interfaces.AuditLogs;
using FitCore.BLL.Interfaces.Classes;
using FitCore.BLL.Interfaces.Notifications;
using FitCore.BLL.Interfaces.PrivateSessions;
using FitCore.BLL.Interfaces.Profile;
using FitCore.BLL.Interfaces.Trainers;
using FitCore.BLL.Interfaces.Trainers;
using FitCore.BLL.Services.AuditLogs;
using FitCore.BLL.Services.Classes;
using FitCore.BLL.Services.Notifications;
using FitCore.BLL.Services.PrivateSessions;
using FitCore.BLL.Services.Profile;
using FitCore.BLL.Services.Trainers;
using FitCore.BLL.Services.Trainers;
using FitCore.DAL.Data;
using FitCore.DAL.Data.Contexts;
using FitCore.DAL.Interfaces;
using Hangfire;
using Microsoft.EntityFrameworkCore;
namespace FitCore.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Connection String & DbContext
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            Console.WriteLine("=================================");
            Console.WriteLine(connectionString ?? "NULL");
            Console.WriteLine("=================================");

            builder.Services.AddDbContext<FitCoreDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 2. Unit of Work 
            //builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAuditLogsService, AuditLogService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IClassService, ClassService>();
            builder.Services.AddScoped<ITrainerService, TrainerService>();
            builder.Services.AddScoped<IPrivateSessionService, PrivateSessionService>();
            builder.Services.AddHttpContextAccessor(); 


            #region Added Hangfire

            builder.Services.AddHangfire(config => config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer() //when create job use simple service name not full name with version and Public Key Token
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHangfireServer();
            #endregion

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // OpenAPI & Swagger Configuration
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseMiddleware<GlobalExceptionMiddleware>();

            using (var scope = app.Services.CreateScope())
            {
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                recurringJobManager.AddOrUpdate(
                    "check-MemberShip-expiration",
                    () => notificationService.MemberExpiryNotification(),
                    Cron.Daily(1)
                );
                recurringJobManager.AddOrUpdate(
                    "check-Low-Stock",
                    () => notificationService.LowStockNotification(),
                    Cron.Daily(3)
                );
                recurringJobManager.AddOrUpdate(
                    "check-Near-Expiry-Products",
                    () => notificationService.ExpiryProductsNotification(),
                    Cron.Daily(5)
                );
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHangfireDashboard("/hangfire");
            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");

            app.UseAuthentication(); 
            app.UseAuthorization();            
            
            app.MapControllers();

            var shouldSeed = builder.Configuration.GetValue<bool>("SeedData");

            if (shouldSeed)
            {
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var context = services.GetRequiredService<FitCoreDbContext>();

                        await ContextSeed.SeedAllAsync(context);
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "حصلت مشكلة أثناء تنزيل الداتا الافتراضية (Seeding).");
                    }
                }
            }

            app.Run();
        }
    }
}