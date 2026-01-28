using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace HoneyAndHemlock.Ingredients
{
    public class DrippableIngredient : XRGrabInteractable, IIngredientSource
    {
        [Header("Ingredient Configuration")]
        [SerializeField] private IngredientSO _data;
        [SerializeField] private AnimationCurve _pressureCurve;
        [SerializeField] private GameObject _fallingDropPrefab;
        [SerializeField] private Transform _fallingDropAttachTransform;
        [SerializeField] private float _baseGrowthRate;
        [SerializeField] private float _gentleThreshold;
        [SerializeField] private float _steadyThreshold;
        [SerializeField] private float _rapidThreshold;

        public IngredientType IngredientType => _data != null ? _data.Type : default;
        public IngredientSO Data => _data;
        public IngredientDeliveryMethod DeliveryMethod => IngredientDeliveryMethod.Drop;

        private IXRSelectInteractor _currentInteractor;
        private FallingDrop _currentFallingDrop;

        private void Start()
        {
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
            float squeezeIntensity = 0f;

            if (_currentInteractor is XRBaseInputInteractor inputInteractor)
            {
                squeezeIntensity = inputInteractor.activateInput.ReadValue();
                _currentFallingDrop.SetCanDrop(squeezeIntensity >= _steadyThreshold);
            }

            float beadGrowthRate = _pressureCurve.Evaluate(squeezeIntensity) * _baseGrowthRate * Time.deltaTime;

            _currentFallingDrop.GrowDrop(beadGrowthRate);
        }

        private void CreateDrop()
        {
            _currentFallingDrop = Instantiate(_fallingDropPrefab, _fallingDropAttachTransform.position, Quaternion.identity, transform).GetComponent<FallingDrop>();
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