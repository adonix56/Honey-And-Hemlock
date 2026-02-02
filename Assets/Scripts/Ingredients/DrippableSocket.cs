using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace HoneyAndHemlock.Ingredients 
{
    public class DrippableSocket : MonoBehaviour
    {
        [SerializeField] private XRSocketInteractor _corkSocket;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _corkAttached;
        [SerializeField] private AudioClip _corkDetached;

        private void Awake()
        {
            _corkSocket ??= GetComponent<XRSocketInteractor>();
            _audioSource ??= GetComponent<AudioSource>();
            if (_corkSocket != null)
            {
                _corkSocket.selectEntered.AddListener(OnCorkAttached);
                _corkSocket.selectExited.AddListener(OnCorkDetached);
            }
        }

        private void OnDestroy()
        {
            _corkSocket.selectEntered.RemoveAllListeners();
            _corkSocket.selectExited.RemoveAllListeners();
        }

        private void OnCorkAttached(SelectEnterEventArgs args)
        {
            if (_audioSource != null && _corkAttached != null) _audioSource.PlayOneShot(_corkAttached);
        }

        private void OnCorkDetached(SelectExitEventArgs args)
        {
            if (_audioSource != null && _corkDetached != null) _audioSource.PlayOneShot(_corkDetached);
        }
    }
}
