using UnityEngine;

namespace HoneyAndHemlock.Ingredients
{
    public interface IAddableToCauldron
    {
        IngredientSO IngredientData { get; }
        bool CanUseIngredient { get; }

        public void UsedIngredient();
    }
}