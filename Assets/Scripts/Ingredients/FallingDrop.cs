using System;
using UnityEngine;

namespace HoneyAndHemlock.Ingredients
{
    public class FallingDrop : MonoBehaviour, IIngredientSource
    {
        public event Action OnDrop;

        public enum FallingDropState
        {
            Growing, Hold, Dropped
        }

        [SerializeField] private float _maxScaleHold;
        [SerializeField] private float _dropThreshold;
        [SerializeField] private float _decaySpeed;
        [SerializeField] private Rigidbody _rigidbody;

        private FallingDropState _state;
        private bool _canDrop;
        private bool _decay;
        private float _scaleValue;
        private IngredientSO _data;

        public IngredientType IngredientType => _data != null ? _data.Type : default;
        public IngredientSO Data => _data;
        public IngredientDeliveryMethod DeliveryMethod => IngredientDeliveryMethod.Drip;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _rigidbody ??= GetComponent<Rigidbody>();
            if (_dropThreshold < _maxScaleHold)
            {
                float temp = _maxScaleHold;
                _maxScaleHold = _dropThreshold;
                _dropThreshold = temp;
            }
        }
#endif
        public void Initialize(IngredientSO ingredient)
        {
            _state = FallingDropState.Growing;
            _canDrop = false;
            _scaleValue = 0;
            transform.localScale = Vector3.zero;
            _rigidbody.isKinematic = true;
            _data = ingredient;
        }

        public void SetCanDrop(bool canDrop)
        {
            if (_state == FallingDropState.Dropped) return;

            _canDrop = canDrop;
            
            _state = _canDrop ? FallingDropState.Growing : FallingDropState.Hold;
        }

        public void GrowDrop(float growthValue)
        {
            if (_state == FallingDropState.Dropped) return;

            _decay = growthValue <= 0.00001f;

            if (_state == FallingDropState.Hold && _scaleValue >= _maxScaleHold) return;

            _scaleValue += growthValue;
            if (_scaleValue >= _dropThreshold)
            {
                _scaleValue = _dropThreshold;
                Drop();
            }
            transform.localScale = Vector3.one * _scaleValue;
        }

        private void Update()
        {
            if (_decay)
            {
                _scaleValue -= Mathf.Lerp(0, _scaleValue, _decaySpeed * Time.deltaTime);
            }
        }

        private void Drop() 
        {
            if (_canDrop)
            {
                _state = FallingDropState.Dropped;
                transform.SetParent(null);
                _rigidbody.isKinematic = false;
                OnDrop?.Invoke();
            }
        }
    }
}