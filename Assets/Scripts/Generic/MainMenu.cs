using System.Runtime.CompilerServices;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private const string VIGNETTE = "Vignette";
    private const string FADE_DURATION = "FadeDuration";

    [SerializeField] private XROrigin _xROrigin;
    [SerializeField] private Image _blinder;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private Image _vignetteLabel;
    [SerializeField] private TextMeshProUGUI _vignetteText;
    [SerializeField] private Sprite _onButton;
    [SerializeField] private Sprite _offButton;

    private bool _switchingScenes;
    private float _fadeTimer;

    private void Awake()
    {
        _vignetteLabel.gameObject.SetActive(PlayerPrefs.GetInt(VIGNETTE, 1) == 1);
    }

    public void OnPlay()
    {
        _vignetteLabel.gameObject.SetActive(false);
        _switchingScenes = true;
        _fadeTimer = 0f;
    }

    public void OnOptions()
    {
        Debug.Log("Uhhh Options??");
        if (_switchingScenes) return;

        Debug.Log("are you there??");
        _vignetteLabel.gameObject.SetActive(!_vignetteLabel.IsActive());
    }

    public void OnVignette()
    {
        if (_switchingScenes) return;
        if (_vignetteLabel.sprite == _onButton)
        {
            _vignetteLabel.sprite = _offButton;
            _vignetteText.text = "Vignette Off";
            PlayerPrefs.SetInt(VIGNETTE, 0);
        } else
        {
            _vignetteLabel.sprite = _onButton;
            _vignetteText.text = "Vignette On";
            PlayerPrefs.SetInt(VIGNETTE, 1);
        }
    }

    private void Update()
    {
        if (!_switchingScenes) return;
        Color color = _blinder.color;
        color.a = _fadeTimer / _fadeDuration;
        _fadeTimer += Time.deltaTime;
        _blinder.color = color;
        if ( _fadeTimer > _fadeDuration)
        {
            _switchingScenes = false;
            PlayerPrefs.SetFloat(FADE_DURATION, _fadeDuration);
            SceneManager.LoadScene(1);
        }
    }
}
