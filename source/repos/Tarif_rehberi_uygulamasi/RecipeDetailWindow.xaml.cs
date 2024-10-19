using System;
using System.Windows;
using Tarif_rehberi_uygulamasi.Models;
using System.Linq;

namespace Tarif_rehberi_uygulamasi
{
    public partial class RecipeDetailWindow : Window
    {
        private readonly Recipe _recipe;

        public RecipeDetailWindow(Recipe recipe)
        {
            InitializeComponent();
            _recipe = recipe;

            // Populate the UI with recipe details
            RecipeNameTextBlock.Text = _recipe.RecipeName;
            PreparationTimeTextBlock.Text = $"{_recipe.PreparationTime} dk";
            CategoryTextBlock.Text = _recipe.Category;

            // Populate ingredients list
            IngredientsListBox.ItemsSource = _recipe.RecipeIngredients.Select(ri =>
                $"{ri.Ingredient.IngredientName} - {ri.IngredientAmount} {ri.Ingredient.Unit}");

            // Populate preparation steps
            var steps = _recipe.Instructions.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var step in steps)
            {
                PreparationStepsListBox.Items.Add(step.Trim());
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Close this window and return to the main screen
            this.Close();
        }
    }
}
