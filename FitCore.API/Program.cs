using FitCore.API.Middlewares;
using FitCore.DAL.Data;
using FitCore.DAL.Data.Contexts;
using FitCore.DAL.Interfaces;
using FitCore.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FitCore.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Connection String & DbContext
            builder.Services.AddDbContext<FitCoreDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 2. Unit of Work 
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddHttpContextAccessor();
            // Add services to the container.
            builder.Services.AddControllers();

            // OpenAPI & Swagger Configuration
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseMiddleware<GlobalExceptionMiddleware>();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); 
            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            
            app.MapControllers();

            #region data seeding
            // Data Seeding للـ Roles الأساسية
            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    try
            //    {
            //        var context = services.GetRequiredService<FitCoreDbContext>();
            //        await ContextSeed.SeedRolesAsync(context);
            //    }
            //    catch (Exception ex)
            //    {
            //        var logger = services.GetRequiredService<ILogger<Program>>();
            //        logger.LogError(ex, "حدث خطأ أثناء حقن البيانات التلقائية (Data Seeding)");
            //    }
            //}
            #endregion
            app.Run();
        }
    }
}