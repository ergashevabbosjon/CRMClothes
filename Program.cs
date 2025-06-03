using ClothingWholesaleAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// ----------------------
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var context = services.GetRequiredService<ApplicationDbContext>();
//        context.Database.Migrate(); // Barcha migratsiyalarni qo'llaydi
//        DbInitializer.Initialize(context); // Dastlabki ma'lumotlarni yuklaydi
//    }
//    catch (Exception ex)
//    {
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "An error occurred while migrating or initializing the database.");
//        // Xatolikni qayta tashlash yoki dasturni to'xtatish kerak bo'lishi mumkin
//        // throw; 
//    }
//}

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Development paytida DbInitializer ni ishga tushirishimiz mumkin
    // using (var scope = app.Services.CreateScope())
    // {
    //     var services = scope.ServiceProvider;
    //     try
    //     {
    //         var context = services.GetRequiredService<ApplicationDbContext>();
    //         // Agar baza mavjud bo'lmasa yaratadi va migratsiyalarni qo'llaydi
    //         context.Database.Migrate(); 
    //         // DbInitializer.Initialize(context); // Keyingi qadamda yaratamiz
    //     }
    //     catch (Exception ex)
    //     {
    //         var logger = services.GetRequiredService<ILogger<Program>>();
    //         logger.LogError(ex, "An error occurred while migrating or initializing the database.");
    //     }
    // }
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
