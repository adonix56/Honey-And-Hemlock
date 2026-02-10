using HoneyAndHemlock.Brewing;
using HoneyAndHemlock.Customers;
using SimpleAudioManager;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Playables;

namespace HoneyAndHemlock.Cosmic
{
    public class CosmicManager : MonoBehaviour
    {
        [SerializeField] private Cauldron _cauldron;
        [SerializeField] private Material _cosmicSkybox;
        [SerializeField] private Canvas _blinder;
        [SerializeField] private XROrigin _normalPlayer;
        [SerializeField] private XROrigin _cosmicPlayer;
        [SerializeField] private CustomerCanvas _customerCanvas;
        [SerializeField] private Transform _demon;
        [SerializeField] private Transform _planets;
        [SerializeField] private PlayableDirector _playableDirector;

        private Material _originalSkybox;

        private void Awake()
        {
            _cauldron ??= FindFirstObjectByType<Cauldron>();
            _cauldron.OnCosmicStart += OnCosmicStart;
            _originalSkybox = RenderSettings.skybox;
        }

        public void OnCosmicStart()
        {
            Manager.instance.StopSong();
            RenderSettings.skybox = _cosmicSkybox;
            RenderSettings.fog = true;
            DynamicGI.UpdateEnvironment();
            //_normalPlayer.gameObject.SetActive(false);
            //_cosmicPlayer.gameObject.SetActive(true);
            _customerCanvas.CosmicStart();
            _playableDirector.Play();
        }
        public void OnCosmicEnd()
        {
            Manager.instance.PlaySong(0);
            RenderSettings.skybox = _originalSkybox;
            RenderSettings.fog = false;
            DynamicGI.UpdateEnvironment();
            _customerCanvas.CosmicEnd();
            // _cosmicPlayer.gameObject.SetActive(false);
            //_normalPlayer.gameObject.SetActive(true);
        }

        public void MakePlayerLookAtDemon()
        {
            Transform player = _cosmicPlayer.Camera.transform;
            Vector3 direction = _demon.position - player.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.0001f) return;

            float targetPlayerYaw = Quaternion.LookRotation(direction).eulerAngles.y;
            float playerLocalYaw = player.localRotation.eulerAngles.y;
            float xROriginYaw = targetPlayerYaw - playerLocalYaw;

            float oldXROriginYaw = _cosmicPlayer.transform.rotation.eulerAngles.y;
            float deltaYaw = Mathf.DeltaAngle(oldXROriginYaw, xROriginYaw);

            Vector3 xROriginEuler = _cosmicPlayer.transform.rotation.eulerAngles;
            xROriginEuler.y = xROriginYaw;
            _cosmicPlayer.transform.rotation = Quaternion.Euler(xROriginEuler);
            
            _planets.Rotate(0f, deltaYaw, 0f, Space.World);
        }
    }
}
