using System.Collections.Generic;

namespace Tarif_rehberi_uygulamasi.Models
{
    public class Ingredient
    {
        public int IngredientID { get; set; }
        public string IngredientName { get; set; }
        public string TotalAmount { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }

        // Navigational Property for many-to-many relationship
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
    }
}
