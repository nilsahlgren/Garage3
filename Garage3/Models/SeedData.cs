using Garage3.Data;
using Microsoft.EntityFrameworkCore;

namespace Garage3.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider, int parkingSpaces)
        {
            using (var context = new Garage3Context(
                serviceProvider.GetRequiredService<
                    DbContextOptions<Garage3Context>>()))
            {
                // Look for any parking spaces.
                if (context.ParkingSpace.Any())
                {
                    return;   // DB has been seeded
                }

                int nbrOfParkingSpaces = 10;

                for (int i = 0; i < nbrOfParkingSpaces; i++)
                { 
                    context.ParkingSpace.AddRange(
                        new ParkingSpace
                        {
                            SessionId = null,
                        }
                    );
                }
                context.SaveChanges();
            }
        }
    }
}
