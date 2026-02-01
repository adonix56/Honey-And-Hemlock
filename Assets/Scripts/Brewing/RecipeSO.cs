using HoneyAndHemlock.Ingredients;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoneyAndHemlock.Brewing
{
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "Honey && Hemlock/RecipeSO")]
    public class RecipeSO : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Color _color;
        [SerializeField] private List<RecipeEntry> _requiredIngredients;

        private HashSet<IngredientSO> _reqIngredientsSet;

        public string DisplayName => _name;
        public string Description => _description;
        public Color Color => _color;
        public List<RecipeEntry> RequiredIngredients => _requiredIngredients;

        private void OnEnable()
        {
            _reqIngredientsSet = new HashSet<IngredientSO>(_requiredIngredients.Select(e => e.ingredient));
        }

        public bool Contains(IngredientSO ingredient) 
        {
            return _reqIngredientsSet.Contains(ingredient);
        }

        public bool ContainsOnly(IEnumerable<IngredientSO> ingredients)
        {
            //Simple Check
            if (_reqIngredientsSet == null || ingredients == null) return false;
            if (_reqIngredientsSet.Count != ingredients.Count()) return false;

            //Check each matching ingredient
            foreach (IngredientSO ingredient in ingredients)
            {
                if (!_reqIngredientsSet.Contains(ingredient)) return false;
            }
            return true;
        }
    }
}