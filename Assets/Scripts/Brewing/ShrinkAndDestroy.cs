using HoneyAndHemlock.Ingredients;
using UnityEngine;

namespace HoneyAndHemlock.Brewing
{
    public class ShrinkAndDestroy : MonoBehaviour
    {
        private float _shrinkDurationInSeconds = 2f;
        private float _shrinkDuration;
        private float _startScale;
        private float _currentScale;

        private void Awake()
        {
            if (TryGetComponent<DropIngredient>(out DropIngredient drop))
            {
                Transform dropParent = drop.transform.parent;

                if (dropParent != null && !dropParent.TryGetComponent<ShrinkAndDestroy>(out _))
                {
                    dropParent.gameObject.AddComponent<ShrinkAndDestroy>();
                }
                Destroy(this);
                return;
            }
        }

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
                if (gameObject.TryGetComponent<RespawnableObject>(out RespawnableObject ro)) ro.RespawnMe();
                Destroy(gameObject);
            }
        }
    }
}
