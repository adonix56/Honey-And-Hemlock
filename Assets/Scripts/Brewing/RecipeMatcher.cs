using HoneyAndHemlock.Ingredients;
using System.Collections.Generic;
using UnityEngine;

namespace HoneyAndHemlock.Brewing
{
    public enum RecipeMatchAmount
    {
        None, One, Multiple
    }

    public class RecipeMatcher : MonoBehaviour
    {
        [SerializeField] private List<RecipeSO> _allRecipes;
        [SerializeField] private List<RecipeSO> _possibleRecipes;

        private void Awake()
        {
            SetupNewRecipe();
        }

        public void SetupNewRecipe()
        {
            _possibleRecipes = new List<RecipeSO>(_allRecipes);
        }

        public void FilterRecipes(Dictionary<IngredientSO, int> currentIngredients)
        {
            if (_possibleRecipes.Count == 0) return;
            if (currentIngredients == null || currentIngredients.Count == 0) return;

            // Iterating backwards protects from index shifting issues when removing from a list during iteration
            for (int i = _possibleRecipes.Count - 1; i >= 0; i--) { 
                RecipeSO currentRecipe = _possibleRecipes[i];
                foreach (IngredientSO ingredient in currentIngredients.Keys)
                {
                    if (!currentRecipe.Contains(ingredient))
                    {
                        _possibleRecipes.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public RecipeMatchAmount GetBestMatch(out RecipeSO matchedRecipe)
        {
            matchedRecipe = null;
            if (_possibleRecipes.Count > 1) return RecipeMatchAmount.Multiple;
            if (_possibleRecipes.Count == 0) return RecipeMatchAmount.None;
            matchedRecipe = _possibleRecipes[0];
            return RecipeMatchAmount.One;
        }
    }
}