using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClothingWholesaleAPI.Data;
using ClothingWholesaleAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingWholesaleAPI.Controllers
{
    [Route("api/[controller]")] // Marshrut: /api/Orders
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            if (_context.Orders == null)
            {
                return NotFound("Orders table is not found.");
            }
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound("Orders table is not found.");
            }
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Orders' is null.");
            }
            // Model validatsiyasini tekshirish (agar xohlasangiz)
            // if (!ModelState.IsValid)
            // {
            //     return BadRequest(ModelState);
            // }

            order.OrderDate = DateTime.UtcNow; // Buyurtma sanasini avtomatik belgilash
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // YANGI METOD: Bir nechta buyurtmani (bulk) qo'shish uchun
        [HttpPost("bulk")] // Marshrut: /api/Orders/bulk
        public async Task<ActionResult<IEnumerable<Order>>> PostBulkOrders([FromBody] List<Order> orders)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Orders' is null.");
            }

            if (orders == null || !orders.Any())
            {
                return BadRequest("Order list cannot be null or empty.");
            }

            var createdOrders = new List<Order>();

            foreach (var order in orders)
            {
                 //Har bir buyurtma uchun kerakli validatsiya yoki qiymatlarni belgilash
                 //Masalan, agar klient OrderDate yubormasa, serverda belgilash:
                 if (order.OrderDate == default(DateTime))
                 {
                     order.OrderDate = DateTime.UtcNow;
                 }
                //Yoki sizning modelingizda[Required] atributlari bo'lsa, ModelState.IsValid tekshiruvi
            }

            // Barcha buyurtmalarni bir martada DbContext ga qo'shish ancha samarali
            _context.Orders.AddRange(orders);

            try
            {
                await _context.SaveChangesAsync(); // Barcha o'zgarishlarni bir martada saqlash

                // Qo'shilgan buyurtmalarni (ID'lari bilan) qaytarish
                // AddRange dan keyin order obyektlarining Id'lari avtomatik to'ldirilgan bo'ladi
                return Ok(orders);
            }
            catch (DbUpdateException ex)
            {
                // Xatolikni log qilish yoki mijozga umumiy xabar berish
                // InnerException ni tekshirish foydali bo'lishi mumkin
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while saving orders: {ex.Message}");
            }
        }


        //// PUT: api/Orders/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutOrder(int id, Order order)
        //{
        //    if (id != order.Id)
        //    {
        //        return BadRequest("Order ID mismatch.");
        //    }

        //    _context.Entry(order).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!OrderExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent(); // Muvaffaqiyatli yangilanganida hech qanday kontent qaytarmaydi
        //}


        public record SimpleOrderStatusUpdate(string Status); 

        [HttpPut("{id}")] // Sizning oldingi marshrutingizni saqlab qolamiz
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] SimpleOrderStatusUpdate statusUpdate)
        {
            // 1. Kiruvchi ma'lumotlarni tekshirish
            if (statusUpdate == null || string.IsNullOrEmpty(statusUpdate.Status))
            {
                return BadRequest("Status maydoni majburiy."); // Xatolik xabari
            }

            // 2. Ma'lumotlar bazasidan buyurtmani topish
            var order = await _context.Orders.FindAsync(id); // _context ApplicationDbContext nusxasi deb taxmin qilinadi

            if (order == null)
            {
                return NotFound($"ID si {id} bo'lgan buyurtma topilmadi."); // Buyurtma topilmaganda
            }

            order.Status = statusUpdate.Status; // Order modelida "Status" nomli public string property bo'lishi kerak

            string[] allowedStatuses = { "Yangi", "Tasdiqlangan", "Jo'natilgan", "Yetkazib berildi", "Bekor qilingan" };
            if (!allowedStatuses.Contains(order.Status, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest($"Noto'g'ri status qiymati: {order.Status}. Ruxsat etilgan qiymatlar: {string.Join(", ", allowedStatuses)}");
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Orders.Any(e => e.Id == id))
                {
                    return NotFound($"ID si {id} bo'lgan buyurtma saqlash paytida topilmadi.");
                }
                else
                {
                    throw;
                }
            }
            return Ok(order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound("Orders table is not found.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}