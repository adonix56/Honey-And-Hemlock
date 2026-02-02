using HoneyAndHemlock.Ingredients;
using System;
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

        public Action OnCosmicStart;

        [SerializeField] private RecipeMatcher _recipeMatcher;
        [SerializeField] private LiquidTop _liquidTop;
        [SerializeField] private int _spinsToBrew;
        [SerializeField] private float _stirProgressDecay;
        [SerializeField] private IngredientSO _junkIngredientSO;
        [SerializeField] private RecipeSO _junkRecipeSO;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private List<AudioClip> _ingredientDropClips;
        [SerializeField] private AudioClip _junkDropClip;
        [SerializeField] private AudioClip _finishStirringClip;
        [SerializeField] private AudioClip _cosmicClip;
        [SerializeField] private RecipeSO _cosmicRecipe;

        private bool _isStirring => _stirTransform != null;
        private bool _isStirrable;
        private bool _potionComplete;
        private Dictionary<IngredientSO, int> _ingredients;
        private List<GameObject> _dryIngredients;
        private Transform _stirTransform;
        private float _prevAngle;
        private float _stirProgress;
        private RecipeSO _matchedRecipe;
        private RecipeMatchAmount _filterRecipeResult;

        private void Awake()
        {
            _recipeMatcher ??= GetComponent<RecipeMatcher>();
            _liquidTop ??= GetComponentInChildren<LiquidTop>();
            _audioSource ??= GetComponent<AudioSource>();
            _ingredients = new Dictionary<IngredientSO, int>();
            _dryIngredients = new List<GameObject>();
            _stirProgress = 0;
            _isStirrable = false;
            _potionComplete = false;
            _filterRecipeResult = RecipeMatchAmount.Multiple;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IAddableToCauldron>(out IAddableToCauldron ingredient))
            {
                if (ingredient.CanUseIngredient)
                {
                    if (ingredient.IngredientData.DeliveryMethod != IngredientDeliveryMethod.Pour)
                    {
                        DestroyOrStoreDryIngredient(other.gameObject);
                    }
                    AddIngredient(ingredient.IngredientData);
                    ingredient.UsedIngredient();
                }
            } else if (other.TryGetComponent<Stirrer>(out Stirrer newStir)) {
                if (_isStirrable)
                {
                    _stirTransform = newStir.StirCalculationPoint;
                    if (_audioSource != null) _audioSource.Play();
                    _prevAngle = CalculateAngle();
                }
            } else if (other.TryGetComponent<XRGrabInteractable>(out XRGrabInteractable nonIngredient))
            {
                if (_isStirrable && other.TryGetComponent<Potion>(out Potion emptyPotion))
                {
                    FillPotionAndResetCauldron(emptyPotion);
                } else
                {
                    // Only add non ingredients if they are dropped in, not while holding them.
                    if (nonIngredient.isSelected) return;
                    DestroyOrStoreDryIngredient(other.gameObject);
                    AddJunk(nonIngredient);
                    nonIngredient.enabled = false;
                }
            }
        }

        private void DestroyOrStoreDryIngredient(GameObject newIngredient)
        {
            // If it's stirrable, destroy it, otherwise, add to list of dry ingredients
            if (_isStirrable) newIngredient.AddComponent<ShrinkAndDestroy>();
            else _dryIngredients.Add(newIngredient);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<Stirrer>(out Stirrer stirrer))
            {
                if (_stirTransform == stirrer.StirCalculationPoint)
                {
                    if (_audioSource != null) _audioSource.Stop();
                    _stirTransform = null;
                }
            }
        }

        private void Update()
        {
            if (_isStirring && !_potionComplete)
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

            UpdateLiquidColor();

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
            if (_potionComplete) return;
            _stirProgress -= Time.deltaTime * _stirProgressDecay;
            _stirProgress = Mathf.Max(_stirProgress, 0f);
            UpdateLiquidColor();
        }

        private void UpdateLiquidColor()
        {
            Color resultingColor = _matchedRecipe != null ? _matchedRecipe.Color : Color.purple;
            _liquidTop.SetLiquidColor(resultingColor, Mathf.Clamp01(_stirProgress / (_spinsToBrew * 360f)));
        }

        public void AddIngredient(IngredientSO ingredient)
        {
            Debug.Log($"Adding {ingredient.DisplayName}");
            ResetStirring();
            _potionComplete = false;
            HandleIngredientSound(ingredient);

            if (_ingredients.ContainsKey(ingredient))
            {
                _ingredients[ingredient]++;
            } else
            {
                _ingredients[ingredient] = 1;
                _recipeMatcher.FilterRecipes(_ingredients);
                _filterRecipeResult = _recipeMatcher.GetBestMatch(out RecipeSO matchedRecipe);
                if (_filterRecipeResult == RecipeMatchAmount.One)
                {
                    _matchedRecipe = matchedRecipe.ContainsOnly(_ingredients.Keys) ? matchedRecipe : null;
                }
                if (ingredient.DeliveryMethod == IngredientDeliveryMethod.Pour)
                {
                    SetStirrable();
                }
            }
        }

        private void HandleIngredientSound(IngredientSO ingredient)
        {
            if (_audioSource == null) return;
            if (_ingredientDropClips == null) return;
            if (_ingredientDropClips.Count != 3) return;

            switch (ingredient.DeliveryMethod)
            {
                case IngredientDeliveryMethod.Drop:
                    if (_isStirrable) _audioSource.PlayOneShot(_ingredientDropClips[0]);
                    break;
                case IngredientDeliveryMethod.Drip:
                    if (_isStirrable) _audioSource.PlayOneShot(_ingredientDropClips[1]);
                    break;
                default:
                    _audioSource.PlayOneShot(_ingredientDropClips[2]);
                    break;
            }
        }

        private void ResetStirring()
        {
            _stirProgress = 0f;
        }

        private void SetStirrable()
        {
            _isStirrable = true;
            _liquidTop.ShowLiquid();
            for (int i = _dryIngredients.Count - 1; i >= 0; i--) 
            {
                _dryIngredients[i].AddComponent<ShrinkAndDestroy>();
            }
            _dryIngredients.Clear();
        }

        private void AddJunk(XRGrabInteractable nonIngredient)
        {
            Debug.Log($"Adding NonIngredient {nonIngredient.name}");
            if (_isStirrable && _audioSource != null && _junkDropClip != null) _audioSource.PlayOneShot(_junkDropClip);
            if (!_ingredients.ContainsKey(_junkIngredientSO)) _ingredients[_junkIngredientSO] = 1;
            _filterRecipeResult = RecipeMatchAmount.None;
            _matchedRecipe = null;
            ResetStirring();
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
            _potionComplete = true;

            if (_matchedRecipe == _cosmicRecipe)
            {
                CosmicActivate();
                return;
            }

            if (_audioSource != null)
            {
                _audioSource.Stop();
                if (_finishStirringClip != null) _audioSource.PlayOneShot(_finishStirringClip);
            }
        }

        private void FillPotionAndResetCauldron(Potion emptyPotion)
        {
            if (emptyPotion.CanFillPotion)
            {
                if (!_potionComplete || _matchedRecipe == null) _matchedRecipe = _junkRecipeSO;
                emptyPotion.FillPotion(_matchedRecipe);
                ResetBrew();
            }
        }

        private void ResetBrew()
        {
            ResetStirring();
            _isStirrable = false;
            _potionComplete = false;
            _stirTransform = null;
            _dryIngredients.Clear();
            _ingredients.Clear();
            _recipeMatcher.SetupNewRecipe();
            _liquidTop.ResetLiquidColor();
            _liquidTop.HideLiquid();
            _filterRecipeResult = RecipeMatchAmount.Multiple;
        }

        private void CosmicActivate()
        {
            _audioSource.Stop();
            ResetBrew();
            OnCosmicStart?.Invoke();
            return;
        }
    }
}