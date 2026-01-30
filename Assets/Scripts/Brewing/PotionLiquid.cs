using UnityEngine;

namespace HoneyAndHemlock.Brewing
{
    public class PotionLiquid : MonoBehaviour
    {
        private const float FLOAT_ERROR = 0.0001f;
        [SerializeField] private float _maxWobble = 0.03f;
        [SerializeField] private float _wobbleSpeedMove = 1f;
        [SerializeField] private float _recovery = 1f;
        [SerializeField] private Renderer _renderer;

        private Vector3 _lastPos;
        private Vector3 _velocity;
        private Quaternion _lastRot;
        private Vector3 _angularVelocity;
        private float _wobbleAmountX;
        private float _wobbleAmountZ;
        private float _wobbleAmountToAddX;
        private float _wobbleAmountToAddZ;
        private float _pulse;
        private Material _material;
        private float _time = 0.5f;

        private void Awake()
        {
            Setup();
        }

        private void Setup()
        {
            _renderer ??= GetComponent<Renderer>();
            if (_renderer != null) _material = _renderer.material;
            _renderer.enabled = false;
        }
        private void Update()
        {
            _time += Time.deltaTime;

            // decrease wobble over time
            _wobbleAmountToAddX = Mathf.Lerp(_wobbleAmountToAddX, 0, (Time.deltaTime * _recovery));
            _wobbleAmountToAddZ = Mathf.Lerp(_wobbleAmountToAddZ, 0, (Time.deltaTime * _recovery));

            // make a sine wave of the decreasing wobble
            _pulse = 2 * Mathf.PI * _wobbleSpeedMove;
            _wobbleAmountX = _wobbleAmountToAddX * Mathf.Sin(_pulse * _time);
            _wobbleAmountZ = _wobbleAmountToAddZ * Mathf.Sin(_pulse * _time);

            // velocity
            _velocity = (_lastPos - transform.position) / Time.deltaTime;
            _angularVelocity = GetAngularVelocity(_lastRot, transform.rotation);

            // add clamped velocity to wobble
            _wobbleAmountToAddX += Mathf.Clamp((_velocity.x + (_velocity.y * 0.2f) + _angularVelocity.z + _angularVelocity.y) * _maxWobble, -_maxWobble, _maxWobble);
            _wobbleAmountToAddZ += Mathf.Clamp((_velocity.z + (_velocity.y * 0.2f) + _angularVelocity.x + _angularVelocity.y) * _maxWobble, -_maxWobble, _maxWobble);

            // send it to the shader
            _material.SetFloat("_WobbleX", _wobbleAmountX);
            _material.SetFloat("_WobbleZ", _wobbleAmountZ);

            // keep last position
            _lastPos = transform.position;
            _lastRot = transform.rotation;
        }

        private Vector3 GetAngularVelocity(Quaternion foreLastFrameRotation, Quaternion lastFrameRotation)
        {
            Quaternion q = lastFrameRotation * Quaternion.Inverse(foreLastFrameRotation);
            // no rotation?
            // You may want to increase this closer to 1 if you want to handle very small rotations.
            // Beware, if it is too close to one your answer will be Nan
            if (Mathf.Abs(q.w) > 1023.5f / 1024.0f) return Vector3.zero;
            float gain;
            // handle negatives, we could just flip it but this is faster
            if (q.w < 0.0f)
            {
                float angle = Mathf.Acos(-q.w);
                gain = -2.0f * angle / (Mathf.Sin(angle) * Time.deltaTime);
            } else
            {
                float angle = Mathf.Acos(q.w);
                gain = 2.0f * angle / (Mathf.Sin(angle) * Time.deltaTime);
            }
            Vector3 angularVelocity = new Vector3(q.x * gain, q.y * gain, q.z * gain);

            if (float.IsNaN(angularVelocity.z))
            {
                angularVelocity = Vector3.zero;
            }
            return angularVelocity;
        }

        public void SetLiquidColor(Color color)
        {
            _material.SetColor("_TopColor", color);
            _material.SetColor("_SideColor", color * 0.8f);
        }

        public void FillLiquid(float percentFill)
        {
            if (percentFill <= FLOAT_ERROR) _renderer.enabled = false;
            else if (!_renderer.enabled) _renderer.enabled = true;
            _material.SetFloat("_LiquidFill", percentFill);
        }
    }
}