using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Tarif_rehberi_uygulamasi.Models;

namespace Tarif_rehberi_uygulamasi
{
    public partial class RecipeAddUpdateWindow : Window
    {
        public Recipe Recipe { get; set; }

        public RecipeAddUpdateWindow(Recipe recipe = null)
        {
            InitializeComponent();
            Recipe = recipe ?? new Recipe();
            LoadRecipeData();
        }

        private void LoadRecipeData()
        {
            if (Recipe != null)
            {
                RecipeNameTextBox.Text = Recipe.RecipeName;
                CategoryTextBox.Text = Recipe.Category;
                PreparationTimeTextBox.Text = Recipe.PreparationTime.ToString();

                // Load ingredients if available
                if (Recipe.RecipeIngredients != null)
                {
                    foreach (var ingredient in Recipe.RecipeIngredients)
                    {
                        AddIngredientControl(ingredient.Ingredient.IngredientName, ingredient.IngredientAmount, ingredient.Ingredient.TotalAmount, ingredient.Ingredient.Unit, ingredient.Ingredient.UnitPrice);
                    }
                }

                // Load preparation steps if available
                if (Recipe.Instructions != null)
                {
                    var steps = Recipe.Instructions.Split('\n');
                    foreach (var step in steps)
                    {
                        AddStepControl(step);
                    }
                }
            }
        }

        private void AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            AddIngredientControl();
        }

        private void AddIngredientControl(string name = "", double amount = 0, string totalAmount = "", string unit = "", decimal unitPrice = 0)
        {
            var ingredientPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0) };

            var ingredientName = new TextBox { Text = name, Width = 150, Margin = new Thickness(0, 0, 10, 0) };
            var ingredientAmount = new TextBox { Text = amount > 0 ? amount.ToString() : "", Width = 100 };
            var ingredientTotalAmount = new TextBox { Text = totalAmount, Width = 100, Margin = new Thickness(10, 0, 10, 0) };
            var ingredientUnit = new TextBox { Text = unit, Width = 100, Margin = new Thickness(10, 0, 10, 0) };
            var ingredientUnitPrice = new TextBox { Text = unitPrice > 0 ? unitPrice.ToString() : "", Width = 100 };

            ingredientPanel.Children.Add(ingredientName);
            ingredientPanel.Children.Add(new TextBlock { Text = "Miktar:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 5, 0) });
            ingredientPanel.Children.Add(ingredientAmount);
            ingredientPanel.Children.Add(new TextBlock { Text = "Total Amount:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 5, 0) });
            ingredientPanel.Children.Add(ingredientTotalAmount);
            ingredientPanel.Children.Add(new TextBlock { Text = "Unit:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 5, 0) });
            ingredientPanel.Children.Add(ingredientUnit);
            ingredientPanel.Children.Add(new TextBlock { Text = "Unit Price:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 5, 0) });
            ingredientPanel.Children.Add(ingredientUnitPrice);

            IngredientsStackPanel.Children.Add(ingredientPanel);
        }

        private void AddStep_Click(object sender, RoutedEventArgs e)
        {
            AddStepControl();
        }

        private void AddStepControl(string step = "")
        {
            var stepTextBox = new TextBox { Text = step, Width = 400, Margin = new Thickness(0, 5, 0, 0) };
            StepsStackPanel.Children.Add(stepTextBox);
        }

        private void SaveRecipe_Click(object sender, RoutedEventArgs e)
        {
            Recipe.RecipeName = RecipeNameTextBox.Text;
            Recipe.Category = CategoryTextBox.Text;
            Recipe.PreparationTime = int.Parse(PreparationTimeTextBox.Text);

            // Collect ingredients
            var ingredients = new List<RecipeIngredient>();
            foreach (StackPanel panel in IngredientsStackPanel.Children)
            {
                var name = ((TextBox)panel.Children[0]).Text;
                var amount = float.Parse(((TextBox)panel.Children[2]).Text);
                var totalAmount = ((TextBox)panel.Children[4]).Text;
                var unit = ((TextBox)panel.Children[6]).Text;
                var unitPrice = decimal.Parse(((TextBox)panel.Children[8]).Text);

                var ingredient = new Ingredient
                {
                    IngredientName = name,
                    TotalAmount = totalAmount,
                    Unit = unit,
                    UnitPrice = unitPrice
                };

                ingredients.Add(new RecipeIngredient { Ingredient = ingredient, IngredientAmount = amount });
            }
            Recipe.RecipeIngredients = ingredients;

            // Collect steps
            var steps = new List<string>();
            foreach (TextBox textBox in StepsStackPanel.Children)
            {
                steps.Add(textBox.Text);
            }
            Recipe.Instructions = string.Join("\n", steps);

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
