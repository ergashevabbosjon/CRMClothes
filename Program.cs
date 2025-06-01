using ClothingWholesaleAPI.Data;
using ClothingWholesaleAPI.Repositories;
using ClothingWholesaleAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace ClothingWholesaleAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();

            // Configure database with connection string from environment variable or config
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                ?? builder.Configuration.GetConnectionString("DefaultConnection");

            // PostgreSQL connection string conversion (Render.com format)
            if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("postgres://"))
            {
                connectionString = ConvertPostgresUrl(connectionString);
            }

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure();
                    npgsqlOptions.CommandTimeout(30);
                }));

            // Register repositories and services
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            // Configure Swagger only for development
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new() { Title = "Clothing Wholesale API", Version = "v1" });
                });
            }

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            // Render.com provides HTTPS automatically, so HTTPS redirection is optional
            // app.UseHttpsRedirection();

            app.UseCors("AllowAll");
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthorization();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            // Database migration and seeding
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var logger = services.GetRequiredService<ILogger<Program>>();

                    logger.LogInformation("Starting database migration...");

                    // Run migrations automatically in production
                    context.Database.Migrate();

                    logger.LogInformation("Database migration completed successfully.");

                    // Seed only in development
                    if (app.Environment.IsDevelopment())
                    {
                        logger.LogInformation("Seeding database...");
                        DbInitializer.Initialize(context);
                        logger.LogInformation("Database seeding completed.");
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while setting up the database.");
                    throw; // Rethrow to prevent the app from starting with a broken database
                }
            }

            // Use PORT environment variable if provided by Render
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            app.Urls.Add($"http://0.0.0.0:{port}");

            app.Run();
        }

        // Convert Render.com PostgreSQL URL format to .NET connection string
        private static string ConvertPostgresUrl(string postgresUrl)
        {
            try
            {
                var uri = new Uri(postgresUrl);
                var connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.Trim('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true;";
                return connectionString;
            }
            catch (Exception)
            {
                // If conversion fails, return the original string
                return postgresUrl;
            }
        }
    }
}