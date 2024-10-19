using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tarif_rehberi_uygulamasi.Models
{
    public class Recipe
    {
        public int RecipeID { get; set; }
        public string RecipeName { get; set; }
        public string Category { get; set; }
        public int PreparationTime { get; set; }
        public string Instructions { get; set; }

        // Navigation property for the relationship to RecipeIngredients
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; }


        // This property is not mapped to the database
        [NotMapped]
        public string IngredientsDisplay { get; set; }

    }

}
