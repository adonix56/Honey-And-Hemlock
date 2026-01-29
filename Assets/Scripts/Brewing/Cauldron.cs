using HoneyAndHemlock.Ingredients;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace HoneyAndHemlock.Brewing
{
    public class Cauldron : MonoBehaviour
    {
        [SerializeField] private RecipeMatcher _recipeMatcher;
        [SerializeField] private int _spinsToBrew;
        [SerializeField] private float _stirProgressDecay;

        private bool _isStirring => _stirTransform != null;
        private Dictionary<IngredientSO, int> _ingredients;
        private Transform _stirTransform;
        private float _prevAngle;
        private float _stirProgress;

        private void Awake()
        {
            _ingredients = new Dictionary<IngredientSO, int>();
            _stirProgress = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IAddableToCauldron>(out IAddableToCauldron ingredient))
            {
                if (ingredient.CanUseIngredient)
                {
                    AddIngredient(ingredient.IngredientData);
                    ingredient.UsedIngredient();
                }
            } else if (other.TryGetComponent<Stirrer>(out Stirrer newStir)) {
                _stirTransform = newStir.StirCalculationPoint;
                _prevAngle = CalculateAngle();
            } else if (other.TryGetComponent<XRGrabInteractable>(out XRGrabInteractable nonIngredient))
            {
                AddJunk(nonIngredient);
                nonIngredient.enabled = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<Stirrer>(out Stirrer stirrer))
            {
                if (_stirTransform == stirrer.StirCalculationPoint) _stirTransform = null;
            }
        }

        private void Update()
        {
            if (_isStirring)
            {
                CalculateStir();
            } else
            {
                DecayStir();
            }
        }

        private void CalculateStir()
        {
            float newAngle = CalculateAngle();
            float deltaAngle = Mathf.DeltaAngle(_prevAngle, newAngle);
            deltaAngle = Mathf.Abs(deltaAngle);
            if (deltaAngle > 0.1)
            {
                _stirProgress += deltaAngle;
            }
            _prevAngle = newAngle;

            if (_stirProgress > _spinsToBrew * 360f)
            {
                CompleteBrew();
            }
        }

        private float CalculateAngle()
        {
            Vector3 stirOffset = _stirTransform.position - transform.position;
            stirOffset.y = 0f;
            return Mathf.Atan2(stirOffset.z, stirOffset.x) * Mathf.Rad2Deg;
        }

        private void DecayStir()
        {
            if (_stirProgress > 0)
            {
                _stirProgress -= Time.deltaTime * _stirProgressDecay;
                _stirProgress = Mathf.Max(_stirProgress, 0f);
            }
        }

        public void AddIngredient(IngredientSO ingredient)
        {
            Debug.Log($"Adding {ingredient.DisplayName}");
            if (_ingredients.ContainsKey(ingredient))
            {
                _ingredients[ingredient]++;
            } else
            {
                _ingredients[ingredient] = 1;
                _recipeMatcher.FilterRecipes(_ingredients);
            }
        }

        private void AddJunk(XRGrabInteractable nonIngredient)
        {
            Debug.Log($"Adding NonIngredient {nonIngredient.name}");
        }

        public void PrintDictionary()
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            foreach (IngredientSO ingredient in _ingredients.Keys)
            {
                stringBuilder.Append($"{ingredient.DisplayName}: {_ingredients[ingredient]}\n");
            }
            Debug.Log(stringBuilder.ToString());
        }

        public void CompleteBrew()
        {
            _stirTransform = null;
            RecipeMatchAmount result = _recipeMatcher.GetBestMatch(out RecipeSO matchedRecipe);

            switch (result)
            {
                case RecipeMatchAmount.None:
                    BadPotionBrewed();
                    break;
                case RecipeMatchAmount.Multiple:
                    BadPotionBrewed();
                    break;
                default: // Found a match!
                    PotionBrewed(matchedRecipe);
                    break;
            }
        }

        private void BadPotionBrewed() { 
            
        }

        private void PotionBrewed(RecipeSO matchedRecipe)
        {

        }
    }
}