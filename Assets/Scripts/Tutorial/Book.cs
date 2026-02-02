using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using HoneyAndHemlock.Ingredients;
using HoneyAndHemlock.Brewing;
using UnityEngine.UI;
using TMPro;

namespace HoneyAndHemlock.Tutorial
{
    public class Book : XRBaseInteractable
    {
        private enum BookState
        {
            Closed, Moving, Open
        }

        private enum BookContentState
        {
            None, Recipe, Ingredients
        }

        private const string OPEN_BOOK = "OpenBook";

        public Action OnBookOpened;
        public Action OnBookClosed;

        [Header("Book Object")]
        [SerializeField] private Animator _animator;
        [SerializeField] private BoxCollider _boxCollider;
        [SerializeField] private Vector3 _centerPositionClosed;
        [SerializeField] private Vector3 _centerSizeClosed;
        [SerializeField] private Vector3 _centerPositionOpened;
        [SerializeField] private Vector3 _centerSizeOpened;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _bookOpenClip;
        [SerializeField] private AudioClip _bookCloseClip;
        [SerializeField] private AudioClip _turnPageClip;
        [SerializeField] private AudioClip _tabClip;

        [Header("Book Contents")]
        [SerializeField] private List<RecipeSO> _recipes;
        [SerializeField] private List<IngredientSO> _ingredients;
        [SerializeField] private Button _recipeTab;
        [SerializeField] private Button _ingredientTab;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _contentText;


        private BookState _bookState;
        private BookContentState _contentState; 
        private bool _isOpen;
        private int _recipeIdx;
        private int _ingredientIdx;

        protected override void Awake()
        {
            base.Awake();
            _animator ??= GetComponent<Animator>();
            _boxCollider ??= GetComponent<BoxCollider>();
            _bookState = BookState.Closed;
            _contentState = BookContentState.None;
            _isOpen = false;
            _recipeIdx = 0;
            _ingredientIdx = 0;
            if (_recipes == null || _recipes.Count == 0) Debug.LogError("Book has no RecipeSO for content.");
            if (_ingredients == null || _ingredients.Count == 0) Debug.LogError("Book has no IngredientSO for content.");
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            Debug.Log("OnSelectEntered on Book");
            if (_bookState != BookState.Moving)
            {
                _isOpen = !_isOpen;
                if (_animator != null && _bookOpenClip != null)
                {
                    if (_isOpen) _audioSource.PlayOneShot(_bookOpenClip);
                    else _audioSource.PlayOneShot(_bookCloseClip);
                }
                
                _animator.SetBool(OPEN_BOOK, _isOpen);
                _boxCollider.center = _isOpen ? _centerPositionOpened : _centerPositionClosed;
                _boxCollider.size = _isOpen ? _centerSizeOpened : _centerSizeClosed;
                _bookState = BookState.Moving;
            }
        }

        public void EndMoving()
        {
            _bookState = _isOpen ? BookState.Open : BookState.Closed;
        }

        public void TabSelected(Button buttonPressed)
        {
            if (buttonPressed == _recipeTab)
            {
                _contentState = BookContentState.Recipe;
                _recipeTab.interactable = false;
                _ingredientTab.interactable = true;
                _nameText.text = _recipes[_recipeIdx].DisplayName;
                _contentText.text = _recipes[_recipeIdx].Description;
            } else
            {
                _contentState = BookContentState.Ingredients;
                _recipeTab.interactable = true;
                _ingredientTab.interactable = false;
                _nameText.text = _ingredients[_ingredientIdx].DisplayName;
                _contentText.text = _ingredients[_ingredientIdx].Description;
            }

            if (_audioSource != null && _tabClip != null) _audioSource.PlayOneShot(_tabClip);
        }

        public void NextPage()
        {
            if (_contentState == BookContentState.Recipe)
            {
                _recipeIdx++;
                if (_recipeIdx >= _recipes.Count) _recipeIdx = 0;
                _nameText.text = _recipes[_recipeIdx].DisplayName;
                _contentText.text = _recipes[_recipeIdx].Description;
            } else if (_contentState == BookContentState.Ingredients)
            {
                _ingredientIdx++;
                if (_ingredientIdx >= _ingredients.Count) _ingredientIdx = 0;
                _nameText.text = _ingredients[_ingredientIdx].DisplayName;
                _contentText.text = _ingredients[_ingredientIdx].Description;

            }
            if (_audioSource != null && _turnPageClip != null) _audioSource.PlayOneShot(_turnPageClip);
        }

        public void PreviousPage()
        {
            if (_contentState == BookContentState.Recipe)
            {
                _recipeIdx--;
                if (_recipeIdx < 0) _recipeIdx = _recipes.Count - 1;
                _nameText.text = _recipes[_recipeIdx].DisplayName;
                _contentText.text = _recipes[_recipeIdx].Description;
            } else if (_contentState == BookContentState.Ingredients)
            {
                _ingredientIdx--;
                if (_ingredientIdx < 0) _ingredientIdx = _ingredients.Count - 1;
                _nameText.text = _ingredients[_ingredientIdx].DisplayName;
                _contentText.text = _ingredients[_ingredientIdx].Description;
            }
            if (_audioSource != null && _turnPageClip != null) _audioSource.PlayOneShot(_turnPageClip);
        }
    }
}

