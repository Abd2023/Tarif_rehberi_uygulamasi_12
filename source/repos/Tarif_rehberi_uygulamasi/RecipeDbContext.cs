using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Tarif_rehberi_uygulamasi.Models;

public class RecipeDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    // Constructor with options
    public RecipeDbContext(DbContextOptions<RecipeDbContext> options)
    : base(options)
    {
    }


    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<RecipeIngredient>()
            .HasKey(ri => new { ri.RecipeID, ri.IngredientID });

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Recipe)
            .WithMany(r => r.RecipeIngredients)
            .HasForeignKey(ri => ri.RecipeID);

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientID);
    }


    public void Seed()
    {
        if (!Recipes.Any())
        {
            var recipes = new List<Recipe>
                {
                    new Recipe { RecipeName = "Spaghetti Aglio e Olio", Category = "Pasta", PreparationTime = 15, Instructions = "Cook spaghetti in salted boiling water until al dente. In a pan, heat olive oil and add minced garlic. Add red pepper flakes and sauté until fragrant. Toss in the cooked spaghetti and mix well. Garnish with parsley and serve." },
                    new Recipe { RecipeName = "Chicken Stir Fry", Category = "Chicken", PreparationTime = 20, Instructions = "Slice chicken breast into strips. Stir-fry in a hot pan with oil until cooked through. Add bell peppers, broccoli, and carrots. Stir in soy sauce and cook until vegetables are tender. Serve hot over rice." },
                    new Recipe { RecipeName = "Vegetable Curry", Category = "Vegetarian", PreparationTime = 30, Instructions = "Heat oil in a pan and add chopped onions, garlic, and ginger. Add diced tomatoes and curry powder; cook until fragrant. Add mixed vegetables (like potatoes, carrots, and peas). Pour in coconut milk and simmer until vegetables are tender. Serve with rice or naan." },
                    new Recipe { RecipeName = "Pancakes", Category = "Breakfast", PreparationTime = 10, Instructions = "In a bowl, mix flour, sugar, baking powder, and salt. In another bowl, whisk milk, egg, and melted butter. Combine wet and dry ingredients, stirring until smooth. Pour batter onto a hot griddle and cook until bubbles form. Flip and cook until golden brown. Serve with syrup." },
                    new Recipe { RecipeName = "Caesar Salad", Category = "Salad", PreparationTime = 15, Instructions = "In a large bowl, combine chopped romaine lettuce and croutons. In a separate bowl, whisk together mayonnaise, lemon juice, garlic, and anchovies. Pour dressing over the salad and toss to coat. Sprinkle with grated Parmesan cheese and serve." }
                };

            Recipes.AddRange(recipes);
            SaveChanges();
            Console.WriteLine("Seeded the database with initial data.");
        }
        else
        {
            Console.WriteLine("Database already contains recipes. Skipping seed.");
        }
    }



    public List<Recipe> SearchRecipes(string recipeName, string ingredient)
    {
        return Recipes
            .Where(r => r.RecipeName.Contains(recipeName) ||
                        r.RecipeIngredients.Any(i => i.Ingredient.IngredientName.Contains(ingredient)))
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .ToList();
    }

    public List<Recipe> FilterRecipes(string category = null, int? maxPrepTime = null, int? minIngredients = null, decimal? maxCost = null)
    {
        var query = Recipes.Include(r => r.RecipeIngredients).AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(r => r.Category == category);
        }

        if (maxPrepTime.HasValue)
        {
            query = query.Where(r => r.PreparationTime <= maxPrepTime);
        }

        if (minIngredients.HasValue)
        {
            query = query.Where(r => r.RecipeIngredients.Count >= minIngredients);
        }

        

        return query.ToList();
    }



}
