using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace ClothingWholesaleAPI.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                Console.WriteLine("🚀 [DB INIT] Migration boshlanmoqda...");
                context.Database.Migrate(); // mavjud migrationlarni bazaga qo‘llaydi
                Console.WriteLine("✅ [DB INIT] Migrationlar muvaffaqiyatli bajarildi.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Migration vaqtida xatolik yuz berdi.");
                throw;
            }
        }
    }
}
