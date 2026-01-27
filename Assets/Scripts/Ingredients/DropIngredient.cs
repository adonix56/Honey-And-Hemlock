using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace HoneyAndHemlock.Ingredients 
{
    public class DropIngredient : XRGrabInteractable, IIngredientSource
    {
        [Header("Ingredient Configuration")]
        [SerializeField] private IngredientSO _data;

        public IngredientType IngredientType => _data != null ? _data.Type : default;
        public IngredientSO Data => _data;
        public IngredientDeliveryMethod DeliveryMethod => IngredientDeliveryMethod.Drop;
    }
}