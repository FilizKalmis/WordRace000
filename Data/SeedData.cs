using Microsoft.EntityFrameworkCore;
using WordRace000.Models;

namespace WordRace000.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Look for any existing words
                if (context.Words.Any())
                {
                    return;   // DB has been seeded
                }

                // Add categories
                var objects = new Category { CategoryName = "Eşyalar" };
                var animals = new Category { CategoryName = "Hayvanlar" };
                var colors = new Category { CategoryName = "Renkler" };

                context.Category.AddRange(
                    objects,
                    animals,
                    colors
                );
                context.SaveChanges();

                // Add words
                context.Words.AddRange(
                    new Word
                    {
                        English = "CHAIR",
                        Turkish = "SANDALYE",
                        CategoryId = objects.Id,
                        Difficulty = 1
                    },
                    new Word
                    {
                        English = "TABLE",
                        Turkish = "MASA",
                        CategoryId = objects.Id,
                        Difficulty = 1
                    },
                    new Word
                    {
                        English = "LION",
                        Turkish = "ASLAN",
                        CategoryId = animals.Id,
                        Difficulty = 1
                    },
                    new Word
                    {
                        English = "TIGER",
                        Turkish = "KAPLAN",
                        CategoryId = animals.Id,
                        Difficulty = 1
                    },
                    new Word
                    {
                        English = "BLACK",
                        Turkish = "SİYAH",
                        CategoryId = colors.Id,
                        Difficulty = 1
                    },
                    new Word
                    {
                        English = "WHITE",
                        Turkish = "BEYAZ",
                        CategoryId = colors.Id,
                        Difficulty = 1
                    }
                );

                context.SaveChanges();
            }
        }
    }
} 