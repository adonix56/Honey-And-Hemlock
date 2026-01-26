using TMPro;
using UnityEngine;

namespace HoneyAndHemlock.Ingredients
{
    [CreateAssetMenu(fileName = "NewIngredient", menuName = "Honey && Hemlock/IngredientSO")]
    public class IngredientSO : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private IngredientType _type;
        [SerializeField] private string _displayName;

        public IngredientType Type => _type;
        public string DisplayName => _displayName;
    }
}
