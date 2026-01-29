using HoneyAndHemlock.Ingredients;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace HoneyAndHemlock.Brewing
{
    public class Cauldron : MonoBehaviour
    {
        [SerializeField] private RecipeMatcher _recipeMatcher;

        private Dictionary<IngredientSO, int> _ingredients;

        private void Awake()
        {
            _ingredients = new Dictionary<IngredientSO, int>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IAddableToCauldron>(out IAddableToCauldron ingredient))
            {
                if (ingredient.CanUseIngredient)
                {
                    AddIngredient(ingredient.IngredientData);
                    ingredient.UsedIngredient();
                }
            } else if (other.TryGetComponent<XRGrabInteractable>(out XRGrabInteractable nonIngredient))
            {
                AddJunk(nonIngredient);
                nonIngredient.enabled = false;
            }
        }

        public void AddIngredient(IngredientSO ingredient)
        {
            Debug.Log($"Adding {ingredient.DisplayName}");
            if (_ingredients.ContainsKey(ingredient))
            {
                _ingredients[ingredient]++;
            } else
            {
                _ingredients[ingredient] = 1;
                _recipeMatcher.FilterRecipes(_ingredients);
            }
        }

        private void AddJunk(XRGrabInteractable nonIngredient)
        {
            Debug.Log($"Adding NonIngredient {nonIngredient.name}");
        }

        public void PrintDictionary()
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            foreach (IngredientSO ingredient in _ingredients.Keys)
            {
                stringBuilder.Append($"{ingredient.DisplayName}: {_ingredients[ingredient]}\n");
            }
            Debug.Log(stringBuilder.ToString());
        }
    }
}