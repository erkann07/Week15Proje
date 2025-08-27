using TuneStore.Domain.Entities;
using Microsoft.AspNetCore.DataProtection;
using TuneStore.Infrastructure.Data;

namespace TuneStore.Infrastructure.Seed
{
    public static class DbSeeder
    {
        public static async Task Seed(TuneStoreDbContext ctx, IDataProtector? protector = null)
        {
            if (!ctx.Albums.Any())
            {
                ctx.Albums.AddRange(
                    new Album { Title = "Acoustic Breeze", Artist = "Artist A", Price = 49.9m, StockQuantity = 10 },
                    new Album { Title = "Electric Nights", Artist = "Artist B", Price = 79.9m, StockQuantity = 5 }
                );
            }

            if (!ctx.Users.Any())
            {
                var pwd = protector != null ? protector.Protect("Admin123!") : "Admin123!";
                object value = ctx.Users.Add(new User
                {
                    FirstName = "Admin",
                    LastName = "Tune",
                    Email = "admin@tunestore.local",
                    PhoneNumber = "0000000000",
                    Password = pwd,
                    Role = UserRole.Admin
                });
            }

            await ctx.SaveChangesAsync();
        }

        internal static void SeedAsync(TuneStoreDbContext ctx, IDataProtector? protector)
        {
            throw new NotImplementedException();
        }
    }
}
