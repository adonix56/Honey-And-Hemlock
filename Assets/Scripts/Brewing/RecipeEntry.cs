using HoneyAndHemlock.Ingredients;
using System;
using UnityEngine;

namespace HoneyAndHemlock.Brewing
{
    [Serializable]
    public class RecipeEntry
    {
        public IngredientSO ingredient;
        public int minAmount;
        public int maxAmount;
    }
}
