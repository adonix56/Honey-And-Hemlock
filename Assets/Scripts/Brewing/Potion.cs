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
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _corkAttached;
        [SerializeField] private AudioClip _corkDetached;
        [SerializeField] private AudioClip _submitPotionClip;

        public bool CanFillPotion => _cork == null && _potionContents == null;
        public RecipeSO PotionContents => _potionContents;

        private RecipeSO _potionContents;
        private float _drainTimer;
        private float _pourTimer;
        private RespawnableObject _cork;

        private void Awake()
        {
            _corkSocket ??= GetComponentInChildren<XRSocketInteractor>();
            _liquid ??= GetComponentInChildren<PotionLiquid>();
            if (_liquid == null) Debug.LogError("Potion Object does not have PotionLiquidChild");
            _potionGrabbable ??= GetComponent<XRGrabInteractable>();
            if (_potionGrabbable == null) Debug.LogError("Potion Object does not have XRGrabInteractable");
            _corkSocket.selectEntered.AddListener(CorkInserted);
            _corkSocket.selectExited.AddListener(CorkRemoved);
            _audioSource ??= GetComponent<AudioSource>();
        }

        private void CorkInserted(SelectEnterEventArgs args)
        {
            _cork = args.interactableObject.transform.GetComponent<RespawnableObject>();
            if (_audioSource != null && _corkAttached != null) _audioSource.PlayOneShot(_corkAttached);
        }

        private void CorkRemoved(SelectExitEventArgs args)
        {
            _cork = null;
            if (_audioSource != null && _corkDetached != null) _audioSource.PlayOneShot(_corkDetached);
        }

        public void FillPotion(RecipeSO newPotion)
        {
            _potionContents = newPotion;
            _liquid.SetLiquidColor(_potionContents.Color);
            _liquid.FillLiquid(_fillAmount);
            _pourTimer = _pourDelay;
            _audioSource.Play();
        }

        private void EmptyPotion()
        {
            _drainTimer = _drainDuration;
            _potionContents = null;
        }

        public bool CanSubmitPotion()
        {
            return _cork != null && _potionContents != null;
        }

        public void SubmitPotion()
        {
            if (_audioSource != null && _submitPotionClip) _audioSource.PlayOneShot(_submitPotionClip);
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
            if (_pourTimer > 0) _pourTimer -= Time.deltaTime;
            else if (_cork == null &&_potionContents != null) CalculateEmptyPotion();
        }

        private void CalculateEmptyPotion()
        {
            float pourAngle = Vector3.Dot(transform.up, Vector3.down);
            if (pourAngle > _pourThreshold) EmptyPotion();
        }

        public void DestroyPotion()
        {
            if (_cork != null) _cork.RespawnMe(true);
            if (TryGetComponent<RespawnableObject>(out RespawnableObject ro)) ro.RespawnMe(true);
            Destroy(_cork.gameObject);
            Destroy(gameObject);
        }
    }
}
