using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace HoneyAndHemlock.Ingredients
{
    public interface IIngredientSource
    {
        IngredientType IngredientType { get; }
        IngredientSO Data { get; }
        IngredientDeliveryMethod DeliveryMethod { get; }
    }

    public enum IngredientDeliveryMethod
    {
        Drop, Pour, Drip
    }
}
