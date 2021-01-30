using RandomNumber.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomNumber.Data
{
    public class DataSeeder
    {
        public static void SeedMatches(ApplicationDbContext context)
        {
            if (!context.Matches.Any(m => m.ExpiryDate >= DateTime.UtcNow))
            {
                var rand = new Random(DateTime.Now.Millisecond);
                var matches = Enumerable.Range(1, 10)
                    .Select(id => { var now = DateTime.UtcNow.AddSeconds(60 + rand.Next(120)); return new Match() { Name = $"{now.ToShortDateString()} - {id}", ExpiryDate = now };})
                    .ToList();
                
                context.AddRange(matches);
                context.SaveChanges();
            }
        }
    }
}
