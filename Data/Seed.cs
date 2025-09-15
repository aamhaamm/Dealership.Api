using Dealership.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Dealership.Api.Data;

public class Seed(AppDbContext db)
{
    public async Task RunAsync()
    {
        // Add Admin if not exists
        if (!await db.Users.AnyAsync())
        {
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@demo.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("P@ssw0rd!"), // default password
                Role = "Admin"
            };
            db.Users.Add(admin);
        }

        // Add Vehicles if not exists
        if (!await db.Vehicles.AnyAsync())
        {
            var rnd = new Random(42);
            var vehicles = Enumerable.Range(1, 10).Select(i => new Vehicle
            {
                Id = Guid.NewGuid(),
                Make = i % 2 == 0 ? "Toyota" : "Honda",
                Model = i % 3 == 0 ? "Corolla" : "Civic",
                Year = 2018 + (i % 6),
                Price = 45000 + rnd.Next(5000, 25000),
                Color = new[] { "White", "Black", "Silver", "Blue", "Red" }[i % 5],
                MileageKm = 30000 + rnd.Next(0, 90000),
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            });
            db.Vehicles.AddRange(vehicles);
        }

        await db.SaveChangesAsync();
    }
}
