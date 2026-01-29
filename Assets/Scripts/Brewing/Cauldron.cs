using HoneyAndHemlock.Ingredients;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace HoneyAndHemlock.Brewing
{
    public class Cauldron : MonoBehaviour
    {
        private const float JITTER_THRESHOLD = 0.1f;

        [SerializeField] private RecipeMatcher _recipeMatcher;
        [SerializeField] private LiquidTop _liquidTop;
        [SerializeField] private int _spinsToBrew;
        [SerializeField] private float _stirProgressDecay;

        private bool _isStirring => _stirTransform != null;
        private bool _isStirrable;
        private Dictionary<IngredientSO, int> _ingredients;
        private List<GameObject> _dryIngredients;
        private Transform _stirTransform;
        private float _prevAngle;
        private float _stirProgress;
        private RecipeSO _matchedRecipe;

        private void Awake()
        {
            _recipeMatcher ??= GetComponent<RecipeMatcher>();
            _liquidTop ??= GetComponentInChildren<LiquidTop>();
            _ingredients = new Dictionary<IngredientSO, int>();
            _dryIngredients = new List<GameObject>();
            _stirProgress = 0;
            _isStirrable = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IAddableToCauldron>(out IAddableToCauldron ingredient))
            {
                if (ingredient.CanUseIngredient)
                {
                    if (!_isStirrable && ingredient.IngredientData.DeliveryMethod != IngredientDeliveryMethod.Pour)
                    {
                        _dryIngredients.Add(other.gameObject);
                    }
                    AddIngredient(ingredient.IngredientData);
                    ingredient.UsedIngredient();
                }
            } else if (other.TryGetComponent<Stirrer>(out Stirrer newStir)) {
                if (_isStirrable)
                {
                    _stirTransform = newStir.StirCalculationPoint;
                    _prevAngle = CalculateAngle();
                }
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
            } else if (_stirProgress > 0)
            {
                DecayStir();
            }
        }

        private void CalculateStir()
        {
            if (_stirTransform == null) return;
            float newAngle = CalculateAngle();
            float deltaAngle = Mathf.DeltaAngle(_prevAngle, newAngle);
            deltaAngle = Mathf.Abs(deltaAngle);
            if (deltaAngle > JITTER_THRESHOLD)
            {
                _stirProgress += deltaAngle;
            }
            _prevAngle = newAngle;

            Color resultingColor = _matchedRecipe != null ? _matchedRecipe.Color : Color.purple;
            _liquidTop.SetLiquidColor(resultingColor, Mathf.Clamp01(_stirProgress / (_spinsToBrew * 360f)));

            if (_stirProgress > _spinsToBrew * 360f)
            {
                CompleteBrew();
            }
        }

        private float CalculateAngle()
        {
            Vector3 stirOffset = _stirTransform.position - transform.position;
            stirOffset.y = 0f;

            // If too close to the center to calculate Angle, return last known angle
            if (stirOffset.magnitude < 0.001f) return _prevAngle;

            return Mathf.Atan2(stirOffset.z, stirOffset.x) * Mathf.Rad2Deg;
        }

        private void DecayStir()
        {
            _stirProgress -= Time.deltaTime * _stirProgressDecay;
            _stirProgress = Mathf.Max(_stirProgress, 0f);
        }

        public void AddIngredient(IngredientSO ingredient)
        {
            Debug.Log($"Adding {ingredient.DisplayName}");
            ResetStirring();
            if (_ingredients.ContainsKey(ingredient))
            {
                _ingredients[ingredient]++;
            } else
            {
                _ingredients[ingredient] = 1;
                _recipeMatcher.FilterRecipes(_ingredients);
                _recipeMatcher.GetBestMatch(out _matchedRecipe);
                if (ingredient.DeliveryMethod == IngredientDeliveryMethod.Pour)
                {
                    SetStirrable();
                }
            }
        }

        private void ResetStirring()
        {
            _stirProgress = 0f;
        }

        private void SetStirrable()
        {
            _isStirrable = true;
            _liquidTop.SetLiquid();
            for (int i = _dryIngredients.Count - 1; i >= 0; i--) 
            {
                // TODO: Set Dissolve Animation, Low Priority
                Destroy(_dryIngredients[i]);
            }
            _dryIngredients.Clear();
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
            StartCoroutine(WaitThenReset());
        }

        private void PotionBrewed(RecipeSO matchedRecipe)
        {
            foreach (RecipeEntry recipeEntry in matchedRecipe.RequiredIngredients)
            {
                int ingredientQty = _ingredients[recipeEntry.ingredient];
                switch (recipeEntry.ingredient.DeliveryMethod)
                {
                    case IngredientDeliveryMethod.Drop:
                        break;
                    case IngredientDeliveryMethod.Drip:
                        break;
                    default: // Pour
                        break;
                }
            }
            StartCoroutine(WaitThenReset());
        }

        private IEnumerator WaitThenReset()
        {
            yield return new WaitForSeconds(2f);
            ResetBrew();
        }

        private void ResetBrew()
        {
            _stirTransform = null;
            _dryIngredients.Clear();
            _ingredients.Clear();
            _recipeMatcher.SetupNewRecipe();
            _liquidTop.ResetLiquidColor();
            _liquidTop.HideLiquid();
        }
    }
}