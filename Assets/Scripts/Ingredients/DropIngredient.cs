using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace HoneyAndHemlock.Ingredients 
{
    public class DropIngredient : MonoBehaviour, IAddableToCauldron
    {
        [Header("Ingredient Configuration")]
        [SerializeField] private IngredientSO _data;
        [SerializeField] private XRGrabInteractable _grabInteractable;

        private bool _used = false;

        public IngredientSO IngredientData => _data;

        public bool CanUseIngredient => !_used;

        public void UsedIngredient()
        {
            _used = true;
            if (_grabInteractable != null) _grabInteractable.enabled = false;
        }
    }
}