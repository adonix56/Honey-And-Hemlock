using UnityEngine;

namespace HoneyAndHemlock.Brewing
{
    public class LiquidTop : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;

        private Material _material;
        private Color _baseColor;

        private void Start()
        {
            _renderer ??= GetComponent<Renderer>();
            _material = _renderer.material;
            _baseColor = _material.GetColor("_WaterColor");
            _material.SetColor("_DarkFoamColor", _baseColor * 0.8f);
        }

        public void SetLiquid()
        {
            _renderer.enabled = true;
        }

        public void SetLiquidColor(Color color, float t = 0)
        {
            Color colorLerp = Color.Lerp(_baseColor, color, t);
            _material.SetColor("_WaterColor", colorLerp);
            _material.SetColor("_DarkFoamColor", colorLerp * 0.8f);
        }

        public void ResetLiquidColor()
        {
            SetLiquidColor(_baseColor);
        }

        public void HideLiquid()
        {
            _renderer.enabled = false;
        }
    }
}
