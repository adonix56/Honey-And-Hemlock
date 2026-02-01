using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using HoneyAndHemlock.Brewing;
using System;
using UnityEngine.XR.Interaction.Toolkit;

namespace HoneyAndHemlock.Customers
{
    public class PotionDelivery : XRSocketInteractor
    {
        public Action<Potion> OnPotionSubmitted;

        private Potion _submittedPotion;

        public override bool CanHover(IXRHoverInteractable interactable)
        {
            if (!IsValidPotion(interactable)) return false;
            return base.CanHover(interactable);
        }

        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            if (!IsValidPotion(interactable)) return false;
            return base.CanSelect(interactable);
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            // Redundant check - If potion isn't ready to be submitted, let it go
            // Protects against opening the cork and/or pouring contents out while hovering
            if (_submittedPotion == null || !_submittedPotion.CanSubmitPotion())
            {
                interactionManager.SelectExit(this, args.interactableObject);
                return;
            }
            _submittedPotion.SubmitPotion();
            _submittedPotion.transform.position = attachTransform.position;
            _submittedPotion.transform.rotation = attachTransform.rotation;
            _submittedPotion.transform.parent = attachTransform;
            OnPotionSubmitted?.Invoke(_submittedPotion);
        }

        private bool IsValidPotion(IXRInteractable interactable)
        {
            if (interactable.transform.TryGetComponent<Potion>(out Potion potion))
            {
                if (potion.CanSubmitPotion())
                {
                    _submittedPotion = potion;
                    return true;
                }
            }
            return false;
        }
    }
}
