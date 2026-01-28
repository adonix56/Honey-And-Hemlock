using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace HoneyAndHemlock.Ingredients
{
    public class DrippableContainer : XRGrabInteractable
    {
        [Header("Ingredient Configuration")]
        [SerializeField] private IngredientSO _data;
        [SerializeField] private AnimationCurve _pressureCurve;
        [SerializeField] private GameObject _fallingDropPrefab;
        [SerializeField] private Transform _fallingDropAttachTransform;
        [SerializeField] private float _baseGrowthRate;
        [SerializeField] private float _holdThreshold;

        private IXRSelectInteractor _currentInteractor;
        private FallingDrop _currentFallingDrop;

        protected override void Awake()
        {
            base.Awake();
            CreateDrop();
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            _currentInteractor = args.interactorObject;
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            _currentInteractor = null;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            // Ensures we are calling on correct Update phase
            if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic) return;
            if (!isSelected) return;

            ProcessDripLogic();
        }

        private void ProcessDripLogic()
        {
            if (_currentInteractor == null) return;
            if (_currentFallingDrop == null) return;

            float squeezeIntensity = 0f;

            if (_currentInteractor is XRBaseInputInteractor inputInteractor)
            {
                squeezeIntensity = inputInteractor.activateInput.ReadValue();
                _currentFallingDrop.SetCanDrop(squeezeIntensity >= _holdThreshold);
            }

            float beadGrowthRate = _pressureCurve.Evaluate(squeezeIntensity) * _baseGrowthRate * Time.deltaTime;

            _currentFallingDrop.GrowDrop(beadGrowthRate);
        }

        private void CreateDrop()
        {
            if (_fallingDropPrefab == null || _fallingDropAttachTransform == null)
            {
                Debug.LogError("DrippableIngredient missing prefab or attach transform!", this);
                return;
            }

            GameObject newDrop = Instantiate(
                _fallingDropPrefab, 
                _fallingDropAttachTransform.position, 
                Quaternion.identity, 
                _fallingDropAttachTransform);

            _currentFallingDrop = newDrop.GetComponent<FallingDrop>();

            if (_currentFallingDrop == null)
            {
                Debug.LogError("FallingDropPrefab missing FallingDrop component!", this);
                Destroy(newDrop);
                return;
            }
            _currentFallingDrop.Initialize(_data);
            _currentFallingDrop.OnDrop += OnDrop;
        }

        private void OnDrop()
        {
            _currentFallingDrop.OnDrop -= OnDrop;
            _currentFallingDrop = null;
            CreateDrop();
        }
    }
}