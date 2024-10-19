using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tarif_rehberi_uygulamasi.Models;

namespace Tarif_rehberi_uygulamasi
{
    public partial class MainWindow : Window
    {
        private readonly RecipeDbContext _context;

        public MainWindow()
        {
            InitializeComponent();

            // Build configuration to get connection string from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Get connection string from configuration
            var connectionString = configuration.GetConnectionString("RecipeDatabase");

            // Configure DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<RecipeDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // Pass the options to the context
            _context = new RecipeDbContext(optionsBuilder.Options);

            LoadRecipes();

            // Subscribe to selection changed event of the RecipeDataGrid
            RecipeDataGrid.SelectionChanged += RecipeDataGrid_SelectionChanged;
        }

        private void LoadRecipes()
        {
            var recipes = _context.Recipes
                .Where(r => r.RecipeName != null && r.PreparationTime != null) // Exclude null entries
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ToList();

            // Create a helper property to display ingredients in a readable format
            foreach (var recipe in recipes)
            {
                recipe.IngredientsDisplay = string.Join(", ", recipe.RecipeIngredients.Select(ri => ri.Ingredient.IngredientName));
            }

            RecipeDataGrid.ItemsSource = recipes;
        }

        private void AddRecipe_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new RecipeAddUpdateWindow();
            if (addWindow.ShowDialog() == true)
            {
                _context.Recipes.Add(addWindow.Recipe);
                _context.SaveChanges();
                LoadRecipes();
            }
        }

        private void UpdateRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeDataGrid.SelectedItem is Recipe selectedRecipe)
            {
                var updateWindow = new RecipeAddUpdateWindow(selectedRecipe);
                if (updateWindow.ShowDialog() == true)
                {
                    _context.Recipes.Update(updateWindow.Recipe);
                    _context.SaveChanges();
                    LoadRecipes();
                }
            }
        }

        private void DeleteRecipe_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected recipe from the DataGrid
            if (RecipeDataGrid.SelectedItem is Recipe selectedRecipe)
            {
                // Ask the user to confirm the deletion
                var result = MessageBox.Show($"Are you sure you want to delete the recipe '{selectedRecipe.RecipeName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Remove the recipe from the context
                    _context.Recipes.Remove(selectedRecipe);

                    // Remove related RecipeIngredients
                    var recipeIngredients = _context.RecipeIngredients.Where(ri => ri.RecipeID == selectedRecipe.RecipeID);
                    _context.RecipeIngredients.RemoveRange(recipeIngredients);

                    // Save changes to the database
                    try
                    {
                        _context.SaveChanges();
                        MessageBox.Show("Recipe deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while deleting the recipe: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    // Reload the recipes in the DataGrid to reflect the changes
                    LoadRecipes();
                }
            }
            else
            {
                MessageBox.Show("Please select a recipe to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void MaxDurationTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            FilterRecipes();
        }

        private void IngredientSearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            FilterRecipes();
        }

        private void FilterRecipes()
        {
            var query = _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .AsQueryable();

            // Search by recipe name
            if (!string.IsNullOrWhiteSpace(SearchByNameTextBox.Text))
            {
                string searchTerm = SearchByNameTextBox.Text.ToLower();
                query = query.Where(r => r.RecipeName.ToLower().Contains(searchTerm));
            }

            // Filter by category
            if (CategoryComboBox.SelectedItem is ComboBoxItem selectedCategory)
            {
                if (selectedCategory.Content.ToString() != "All")
                {
                    query = query.Where(r => r.Category == selectedCategory.Content.ToString());
                }
            }

            // Filter by max duration
            if (int.TryParse(MaxDurationTextBox.Text, out int maxDuration))
            {
                query = query.Where(r => r.PreparationTime <= maxDuration);
            }

            // Filter by ingredient search
            if (!string.IsNullOrWhiteSpace(IngredientSearchTextBox.Text))
            {
                var ingredients = IngredientSearchTextBox.Text
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(i => i.Trim().ToLower());

                query = query.Where(r =>
                    r.RecipeIngredients.Any(ri => ingredients.Contains(ri.Ingredient.IngredientName.ToLower())));
            }

            var recipes = query.ToList();

            // Create a helper property to display ingredients in a readable format
            foreach (var recipe in recipes)
            {
                recipe.IngredientsDisplay = string.Join(", ", recipe.RecipeIngredients.Select(ri => ri.Ingredient.IngredientName));
            }

            RecipeDataGrid.ItemsSource = recipes;
        }

        private void ApplyFilters()
        {
            FilterRecipes();
        }

        private void SearchByNameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            FilterRecipes();
        }

        private void RecipeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable the "View Details" button only when an item is selected
            ViewDetailsButton.IsEnabled = RecipeDataGrid.SelectedItem != null;
        }


        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeDataGrid.SelectedItem is Recipe selectedRecipe)
            {
                // Open RecipeDetailWindow and pass the selected recipe
                RecipeDetailWindow detailWindow = new RecipeDetailWindow(selectedRecipe);
                detailWindow.ShowDialog(); // Open as a modal window
            }
        }
    }
}
