using HoneyAndHemlock.Brewing;
using UnityEngine;

namespace HoneyAndHemlock.Ingredients
{
    public class ParticleIngredient : MonoBehaviour
    {
        private IngredientSO _data;

        public void SetIngredientSO(IngredientSO data)
        {
            if (_data == null) _data = data;
        }

        private void OnParticleCollision(GameObject other)
        {
            if (other.TryGetComponent<Cauldron>(out Cauldron cauldron) && _data != null)
            {
                cauldron.AddIngredient(_data);
            }
        }
    }
}
