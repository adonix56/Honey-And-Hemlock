using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace HoneyAndHemlock.Ingredients
{
    public class PourableContainer : XRGrabInteractable
    {
        private const float MAX_DOT_PRODUCT = 1f;

        [Header("Ingredient Configuration")]
        [SerializeField] 
        private IngredientSO _data;
        [SerializeField] 
        private XRSocketInteractor _corkSocket;
        [SerializeField]
        private ParticleSystem _particleSystem;
        [SerializeField, Range(-1f, 1f), Tooltip("Signifies at what dot product value to start pouring. -1f is straight up, 1f is straight down, 0f is horizontal")] 
        private float _pourThreshold;
        [SerializeField, Range(0f, 10f)]
        private float _minPourSpeed;
        [SerializeField, Range(0f, 50f)]
        private float _maxPourSpeed;
        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private AudioClip _corkAttached;
        [SerializeField]
        private AudioClip _corkDetached;

        private ParticleSystem.EmissionModule _emission;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _particleSystem ??= GetComponentInChildren<ParticleSystem>();
            _corkSocket ??= GetComponentInChildren<XRSocketInteractor>();
            _audioSource ??= GetComponent<AudioSource>();
        }
#endif

        protected override void Awake()
        {
            base.Awake();
            _particleSystem ??= GetComponentInChildren<ParticleSystem>();
            _corkSocket ??= GetComponentInChildren<XRSocketInteractor>();
            _audioSource ??= GetComponent<AudioSource>();
            _emission = _particleSystem.emission;
            ParticleIngredient _particleIngredient = _particleSystem.GetComponent<ParticleIngredient>();
            _particleIngredient.SetIngredientSO(_data);
            _corkSocket.selectEntered.AddListener(OnCorkAttached);
            _corkSocket.selectExited.AddListener(OnCorkDetached);
        }

        protected override void OnDestroy()
        {
            _corkSocket.selectEntered.RemoveAllListeners();
            _corkSocket.selectExited.RemoveAllListeners();
            base.OnDestroy();
        }

        private void OnCorkAttached(SelectEnterEventArgs args)
        {
            if (_audioSource != null) _audioSource.PlayOneShot(_corkAttached);
        }

        private void OnCorkDetached(SelectExitEventArgs args)
        {
            if (_audioSource != null) _audioSource.PlayOneShot(_corkDetached);
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            // Ensures we are calling on correct Update phase
            if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic) return;

            bool hasCork = _corkSocket != null && _corkSocket.hasSelection;

            if (!isSelected || hasCork)
            {
                _particleSystem.Stop();
                return;
            }

            ProcessPourLogic();
        }

        private void ProcessPourLogic() 
        {
            if (!isSelected) return;
            float pourAngle = Vector3.Dot(transform.up, Vector3.down);
            if (pourAngle > _pourThreshold)
            {
                // Linearly scale pour speed with pour angle
                // Calculate percentage of pour range (0% is at _pourThreshold, 100% is max pour angle)
                float pourRatio = (pourAngle - _pourThreshold) / (MAX_DOT_PRODUCT - _pourThreshold);
                // Get pour speed based on ratio (0% is at _minPourSpeed, 100% is at _maxPourSpeed)
                float pourSpeed = (_maxPourSpeed - _minPourSpeed) * pourRatio + _minPourSpeed;

                _emission.rateOverTime = pourSpeed;

                if (!_particleSystem.isPlaying)
                {
                    if (_audioSource != null) _audioSource.Play();
                    _particleSystem.Play();
                }
            } else
            {
                if (_particleSystem.isPlaying)
                {
                    if (_audioSource != null) _audioSource.Stop();
                    _particleSystem.Stop();
                }
            }
        }
    }
}