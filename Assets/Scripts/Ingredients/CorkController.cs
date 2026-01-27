using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace HoneyAndHemlock.Ingredients
{
    public class CorkController : MonoBehaviour
    {
        public Action OnCorkRemoved;
        [SerializeField] private XRGrabInteractable _cork;

        private void Start()
        {
            _cork.selectEntered.AddListener(CorkRemoved);
            _cork.enabled = false;
        }

        private void CorkRemoved(SelectEnterEventArgs args)
        {
            OnCorkRemoved?.Invoke();
        }

        public void ActivateCork()
        {
            _cork.enabled = true;
        }

        public void DisableCork()
        {
            _cork.enabled = false;
        }
    }
}
