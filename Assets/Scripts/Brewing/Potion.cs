using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace HoneyAndHemlock.Brewing
{
    public class Potion : MonoBehaviour
    {
        [SerializeField] private PotionLiquid _liquid;
        [SerializeField] private float _fillAmount;
        [SerializeField] private float _pourThreshold = 0.4f;
        [SerializeField] private float _drainDuration = 2f;
        [SerializeField] private float _pourDelay = 3f;
        [SerializeField] private XRSocketInteractor _corkSocket;
        [SerializeField] private XRGrabInteractable _potionGrabbable;

        public bool CanFillPotion => !_hasCork && _potionContents == null;
        public RecipeSO PotionContents => _potionContents;

        private RecipeSO _potionContents;
        private float _drainTimer;
        private float _pourTimer;
        private bool _hasCork;

        private void Awake()
        {
            _corkSocket ??= GetComponentInChildren<XRSocketInteractor>();
            _liquid ??= GetComponentInChildren<PotionLiquid>();
            if (_liquid == null) Debug.LogError("Potion Object does not have PotionLiquidChild");
            _potionGrabbable ??= GetComponent<XRGrabInteractable>();
            if (_potionGrabbable == null) Debug.LogError("Potion Object does not have XRGrabInteractable");
            _corkSocket.selectEntered.AddListener(CorkInserted);
            _corkSocket.selectExited.AddListener(CorkRemoved);
            _hasCork = false;
        }

        private void CorkInserted(SelectEnterEventArgs args)
        {
            _hasCork = true;
        }

        private void CorkRemoved(SelectExitEventArgs args)
        {
            _hasCork = false;
        }

        public void FillPotion(RecipeSO newPotion)
        {
            _potionContents = newPotion;
            _liquid.SetLiquidColor(_potionContents.Color);
            _liquid.FillLiquid(_fillAmount);
            _pourTimer = _pourDelay;
        }

        private void EmptyPotion()
        {
            _drainTimer = _drainDuration;
            _potionContents = null;
        }

        public bool CanSubmitPotion()
        {
            return _hasCork && _potionContents != null;
        }

        public void SubmitPotion()
        {
            _potionGrabbable.enabled = false;
            Rigidbody rb = GetComponent<Rigidbody>();
            Collider collider = GetComponent<Collider>();
            if (rb != null) rb.isKinematic = true;
            if (collider != null) collider.enabled = false;
        }

        private void Update()
        {
            if (_drainTimer > 0)
            {
                _drainTimer -= Time.deltaTime;
                _drainTimer = Mathf.Max(0, _drainTimer);
                _liquid.FillLiquid(_fillAmount * _drainTimer / _drainDuration);
            }
            if (!_hasCork)
            {
                if (_pourTimer > 0) _pourTimer -= Time.deltaTime;
                else if (_potionContents != null) CalculateEmptyPotion();
            }
        }

        private void CalculateEmptyPotion()
        {
            float pourAngle = Vector3.Dot(transform.up, Vector3.down);
            if (pourAngle > _pourThreshold) EmptyPotion();
        }
    }
}
