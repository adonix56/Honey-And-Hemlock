using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefsLoader : MonoBehaviour
{
    private const string VIGNETTE = "Vignette";
    private const string FADE_DURATION = "FadeDuration";

    [SerializeField] private Image _fade;
    [SerializeField] private GameObject _vignette;

    private float _fadeDuration;
    private float _fadeTimer;

    private void Start()
    {
        _vignette.SetActive(PlayerPrefs.GetInt(VIGNETTE, 1) == 1);
        _fadeDuration = PlayerPrefs.GetFloat(FADE_DURATION);
        _fadeTimer = _fadeDuration;
    }

    private void Update()
    {
        if (_fadeTimer <= 0)
        {
            Destroy(this);
            return;
        }
        _fadeTimer -= Time.deltaTime;
        Color color = _fade.color;
        color.a = _fadeTimer / _fadeDuration;
        _fade.color = color;
    }
}
