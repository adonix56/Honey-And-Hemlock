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
        [SerializeField] private string _description;
        [SerializeField] private IngredientCategory _category;
        [SerializeField] private IngredientDeliveryMethod _deliveryMethod;

        public IngredientType Type => _type;
        public string DisplayName => _displayName;
        public string Description => _description;
        public IngredientCategory Category => _category;
        public IngredientDeliveryMethod DeliveryMethod => _deliveryMethod;
    }
}
