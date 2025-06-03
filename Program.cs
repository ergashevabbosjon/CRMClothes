using ClothingWholesaleAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// ----------------------


builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Migrationni avtomatik qo‘llash
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        DbInitializer.Initialize(context, logger);
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ [Startup] Migrationda xatolik: " + ex.Message);
    }
}
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRMClothes API v1");
    c.RoutePrefix = "swagger"; // bu https://crmclothes.onrender.com/swagger bo'lishi uchun kerak
});
// Test o'zgartirish


app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
