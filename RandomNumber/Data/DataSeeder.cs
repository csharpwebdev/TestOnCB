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
                var date = DateTime.UtcNow;
                var matches = Enumerable.Range(1, 100)
                    .Select(id => { date = date.AddSeconds(60 + rand.Next(60)); return new Match() { Name = $"{date.ToShortDateString()} - {id}", ExpiryDate = date };})
                    .ToList();
                
                context.AddRange(matches);
                context.SaveChanges();
            }
        }
    }
}
