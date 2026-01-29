using UnityEngine;

namespace HoneyAndHemlock.Brewing
{
    public class ShrinkAndDestroy : MonoBehaviour
    {
        private float _shrinkDurationInSeconds = 2f;
        private float _shrinkDuration;
        private float _startScale;
        private float _currentScale;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _shrinkDuration = _shrinkDurationInSeconds;
            _startScale = transform.localScale.x;
        }

        // Update is called once per frame
        void Update()
        {
            if (_shrinkDuration > 0f)
            {
                _shrinkDuration -= Time.deltaTime;
                _shrinkDuration = Mathf.Max(0f, _shrinkDuration);
                _currentScale = _startScale * _shrinkDuration / _shrinkDurationInSeconds;
                transform.localScale = Vector3.one * _currentScale;
            } else
            {
                Destroy(gameObject);
            }
        }
    }
}
