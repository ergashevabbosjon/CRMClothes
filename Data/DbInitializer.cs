using ClothingWholesaleAPI.Models;

namespace ClothingWholesaleAPI.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Check if database has been seeded
            if (context.Orders.Any())
            {
                return; // Database has been seeded
            }

            // Create 100 sample orders
            var orders = new List<Order>();
            var random = new Random();

            // Sample data arrays
            var customerNames = new[]
            {
                "John Smith", "Sarah Johnson", "Michael Brown", "Emily Davis", "David Wilson",
                "Jessica Miller", "Christopher Garcia", "Ashley Martinez", "Matthew Rodriguez", "Amanda Clark",
                "Ali Karimov", "Fatima Usmanova", "Bobur Tashmatov", "Nigora Hamidova", "Jasur Olimov",
                "Malika Saidova", "Rustam Nazarov", "Zilola Rahimova", "Sardor Yusupov", "Gulnoza Abdullayeva",
                "Alexandr Petrov", "Elena Smirnova", "Dmitry Volkov", "Anastasia Kozlova", "Vladimir Sokolov",
                "Anna Popova", "Sergey Morozov", "Olga Karpova", "Pavel Lebedev", "Maria Fedorova",
                "Ahmed Hassan", "Fatima Al-Rashid", "Omar Abdullah", "Layla Ibrahim", "Yousef Al-Mansouri",
                "Aisha Khalil", "Khalid Farouk", "Nadia Amini", "Hassan Al-Zahra", "Leila Hosseini",
                "Pierre Dubois", "Sophie Martin", "Jean Leclerc", "Camille Rousseau", "Antoine Moreau",
                "Isabella Rossi", "Marco Ferrari", "Giulia Romano", "Alessandro Marino", "Francesca Ricci",
                "Carlos Rodriguez", "Maria Gonzalez", "Jose Martinez", "Ana Lopez", "Luis Hernandez",
                "Chen Wei", "Li Na", "Wang Fei", "Zhang Lei", "Liu Mei",
                "Rajesh Sharma", "Priya Patel", "Amit Kumar", "Sneha Singh", "Vikram Gupta",
                "Emma Thompson", "Oliver Davies", "Sophie Williams", "James Taylor", "Charlotte Evans",
                "Akiko Tanaka", "Hiroshi Sato", "Yuki Yamamoto", "Rei Takahashi", "Kenji Nakamura",
                "Lars Andersson", "Astrid Larsson", "Erik Nilsson", "Ingrid Johansson", "Magnus Eriksson",
                "Kofi Asante", "Akosua Osei", "Kwame Mensah", "Ama Boateng", "Yaw Opoku",
                "Mohammed Al-Rashid", "Ayesha Khan", "Tariq Ahmad", "Zara Malik", "Imran Sheikh",
                "Isabella Santos", "Rafael Silva", "Camila Oliveira", "Lucas Costa", "Beatriz Ferreira",
                "Kim Min-jun", "Park So-young", "Lee Dong-hyun", "Choi Ji-woo", "Jung Hae-won",
                "Nguyen Van Duc", "Tran Thi Mai", "Le Hoang Nam", "Pham Thu Lan", "Vo Minh Tuan"
            };

            var statuses = new[] { "Yangi", "Tayyorlanmoqda", "Yuborilgan", "Yetkazib berilgan", "Bekor qilingan" };

            var cities = new[]
            {
                "New York, NY", "Los Angeles, CA", "Chicago, IL", "Houston, TX", "Phoenix, AZ",
                "Philadelphia, PA", "San Antonio, TX", "San Diego, CA", "Dallas, TX", "San Jose, CA",
                "Tashkent, Uzbekistan", "Samarkand, Uzbekistan", "Bukhara, Uzbekistan", "Andijan, Uzbekistan", "Namangan, Uzbekistan",
                "Moscow, Russia", "St. Petersburg, Russia", "Novosibirsk, Russia", "Yekaterinburg, Russia", "Kazan, Russia",
                "London, UK", "Manchester, UK", "Birmingham, UK", "Leeds, UK", "Glasgow, UK",
                "Paris, France", "Marseille, France", "Lyon, France", "Toulouse, France", "Nice, France",
                "Berlin, Germany", "Hamburg, Germany", "Munich, Germany", "Cologne, Germany", "Frankfurt, Germany",
                "Madrid, Spain", "Barcelona, Spain", "Valencia, Spain", "Seville, Spain", "Zaragoza, Spain",
                "Rome, Italy", "Milan, Italy", "Naples, Italy", "Turin, Italy", "Palermo, Italy",
                "Dubai, UAE", "Abu Dhabi, UAE", "Sharjah, UAE", "Ajman, UAE", "Ras Al Khaimah, UAE",
                "Istanbul, Turkey", "Ankara, Turkey", "Izmir, Turkey", "Bursa, Turkey", "Antalya, Turkey",
                "Beijing, China", "Shanghai, China", "Guangzhou, China", "Shenzhen, China", "Chengdu, China",
                "Tokyo, Japan", "Osaka, Japan", "Yokohama, Japan", "Nagoya, Japan", "Sapporo, Japan",
                "Seoul, South Korea", "Busan, South Korea", "Incheon, South Korea", "Daegu, South Korea", "Daejeon, South Korea"
            };

            var phoneFormats = new[]
            {
                "+1-555-", "+998-90-", "+998-91-", "+998-93-", "+998-94-", "+998-95-",
                "+7-495-", "+7-812-", "+44-20-", "+33-1-", "+49-30-", "+34-91-",
                "+39-06-", "+971-4-", "+90-212-", "+86-10-", "+81-3-", "+82-2-"
            };

            var notes = new[]
            {
                "Rush order for special event",
                "Customer requested expedited shipping",
                "Corporate bulk order",
                "Wedding collection order",
                "Summer fashion collection",
                "Winter clothing order",
                "Back to school collection",
                "Holiday season special order",
                "VIP customer - priority handling",
                "First time customer",
                "Repeat customer - loyal client",
                "Custom sizing required",
                "Gift order - special packaging",
                "Business uniform order",
                "Fashion week collection",
                "End of season clearance",
                "New product launch order",
                "Exclusive design request",
                "Wholesale distributor order",
                "Retail chain bulk purchase",
                null, null, null, null, null // Some orders without notes
            };

            // Generate 100 orders
            for (int i = 1; i <= 100; i++)
            {
                var orderDate = DateTime.UtcNow.AddDays(-random.Next(1, 365)); // Random date within last year
                var status = statuses[random.Next(statuses.Length)];
                var hasDeliveryDate = status == "Yetkazib berilgan" || (status == "Yuborilgan" && random.Next(2) == 0);

                var order = new Order
                {
                    CustomerName = customerNames[random.Next(customerNames.Length)],
                    Status = status,
                    OrderDate = orderDate,
                    DeliveryDate = hasDeliveryDate ? orderDate.AddDays(random.Next(1, 14)) : null,
                    TotalAmount = Math.Round((decimal)(random.NextDouble() * 4900 + 100), 2), // $100 - $5000
                    Currency = "USD",
                    ShippingAddress = GenerateAddress(cities[random.Next(cities.Length)], random),
                    ContactPhone = phoneFormats[random.Next(phoneFormats.Length)] + random.Next(100, 999) + "-" + random.Next(1000, 9999),
                    TrackingNumber = "TRK" + (1000000 + i).ToString(),
                    Notes = notes[random.Next(notes.Length)]
                };

                orders.Add(order);
            }

            // Add orders to context
            context.Orders.AddRange(orders);

            // Save changes to database
            context.SaveChanges();
        }

        private static string GenerateAddress(string city, Random random)
        {
            var streetNumbers = random.Next(1, 9999);
            var streetNames = new[]
            {
                "Main St", "Oak Ave", "Pine St", "Elm St", "Maple Ave", "Cedar St", "Park Ave", "First St",
                "Second St", "Third St", "Fourth St", "Fifth St", "Broadway", "Market St", "Church St",
                "School St", "Water St", "Union St", "Washington St", "Lincoln Ave", "Madison St",
                "Jefferson St", "Franklin St", "Jackson Ave", "Wilson St", "Johnson St", "Brown St",
                "Davis Ave", "Miller St", "Wilson Ave", "Moore St", "Taylor Ave", "Anderson St",
                "Chilonzor", "Yashnobod", "Mirzo Ulugbek", "Shayxontohur", "Mirobod", "Yunusobod",
                "Sergeli", "Bektemir", "Uchtepa", "Almazar", "Yakkasaray"
            };

            var streetName = streetNames[random.Next(streetNames.Length)];
            var apartment = random.Next(2) == 0 ? $", Apt {random.Next(1, 999)}" : "";

            return $"{streetNumbers} {streetName}{apartment}, {city}";
        }
    }
}