using HoneyAndHemlock.Brewing;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HoneyAndHemlock.Customers
{
    public class CustomerCanvas : MonoBehaviour
    {
        private const string CUSTOMER_REQUEST = "CustomerRequest";
        private const string PLAYER_BUTTON = "PlayerButton";
        private const string CUSTOMER_RESULT = "CustomerResult";
        private const string RESULT_BUTTON = "ResultButton";
        private const string CUSTOMER_COSMIC = "CustomerCosmic";
        private const string CUSTOMER_NAME = "Customer";
        private const string RESULTS = "Results";
        private const string SUCCESS = "Alright!";
        private const string FAIL = "Darn...";

        public Action OnPlayerResponsePressed;
        public Action OnResultPressed;

        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _customerRequest;
        [SerializeField] private TextMeshProUGUI _customerName;
        [SerializeField] private Image _customerNameImage;
        [SerializeField] private Sprite _customerNameSprite;
        [SerializeField] private Sprite _customerResultSprite;
        [SerializeField] private TextMeshProUGUI _customerDialogue;
        [SerializeField] private Image _customerDialogueImage;
        [SerializeField] private Sprite _customerDialogueRequestSprite;
        [SerializeField] private Sprite _customerDialogueResultSprite;
        [SerializeField] private Button _playerButton;
        [SerializeField] private TextMeshProUGUI _playerButtonText;
        [SerializeField] private Button _resultButton;
        [SerializeField] private TextMeshProUGUI _resultButtonText;
        [SerializeField] private Image _resultButtonImage;
        [SerializeField] private Sprite _resultButtonSuccess;
        [SerializeField] private Sprite _resultButtonFail;
        [SerializeField] private float _dialogueSpeed;

        private CustomerSO _currentCustomer;
        private int _storyIdx;
        private bool _success;

        private void Awake()
        {
            _animator ??= GetComponent<Animator>();
        }

        // 5) CustomerManager -> () -> Animator
        public void StartCustomerRequest(CustomerSO customerSO, int storyIndex)
        {
            _currentCustomer = customerSO;
            _storyIdx = storyIndex;
            _customerName.text = CUSTOMER_NAME;
            _customerNameImage.sprite = _customerNameSprite;
            _customerDialogue.text = _storyIdx < _currentCustomer.Request.Length ? _currentCustomer.Request[_storyIdx] : _currentCustomer.Request[0];
            _customerDialogueImage.sprite = _customerDialogueRequestSprite;
            _customerDialogue.ForceMeshUpdate();
            _customerDialogue.maxVisibleCharacters = 0;
            _playerButtonText.text = _storyIdx < _currentCustomer.PlayerResponse.Length ? _currentCustomer.PlayerResponse[_storyIdx] : _currentCustomer.PlayerResponse[0];
            _playerButtonText.ForceMeshUpdate();
            _playerButtonText.maxVisibleCharacters = 0;
            _resultButton.interactable = false;
            _animator.SetBool(CUSTOMER_REQUEST, true);
        }

        // 6) Animator -> () -> this -> Animator (Open Player Response)
        // Animator -> () -> this -> Animator (Results Button)
        public void CustomerRequestOpened()
        {
            StartCoroutine(DialogueToButton());
        }

        // 7) Animator -> () -> this -> Wait for player to press button
        public void PlayerResponseOpened()
        {
            StartCoroutine(ButtonToActivate(_playerButton, _playerButtonText));
        }

        // 8) Waited for player to press button -> () -> CustomerManager
        public void PlayerResponsePressed()
        {
            OnPlayerResponsePressed?.Invoke();
            _animator.SetBool(PLAYER_BUTTON, false);
        }

        // 13) CustomerManager -> () -> Wait for Customer to reach outside (Customer.ReachingOutdoors())
        public void ReceivedPotion(RecipeSO potion)
        {
            _playerButton.interactable = false;
            _animator.SetBool(CUSTOMER_REQUEST, false);
            _success = _currentCustomer.RequestedPotion == potion;
        }

        // 17) CustomerManager -> () -> Animator
        public void DisplayResults()
        {
            _customerName.text = RESULTS;
            _customerNameImage.sprite = _customerResultSprite;
            if (_success)
            {
                _customerDialogue.text = _storyIdx < _currentCustomer.SuccessfulResponse.Length ? _currentCustomer.SuccessfulResponse[_storyIdx] : _currentCustomer.SuccessfulResponse[0];
                _resultButtonText.text = SUCCESS;
                _resultButtonImage.sprite = _resultButtonSuccess;
            } else
            {
                _customerDialogue.text = _storyIdx < _currentCustomer.FailedResponse.Length ? _currentCustomer.FailedResponse[_storyIdx] : _currentCustomer.FailedResponse[0];
                _resultButtonText.text = FAIL;
                _resultButtonImage.sprite = _resultButtonFail;
            }
            _customerDialogueImage.sprite = _customerDialogueResultSprite;
            _customerDialogue.ForceMeshUpdate();
            _customerDialogue.maxVisibleCharacters = 0;
            _resultButtonText.ForceMeshUpdate();
            _resultButtonText.maxVisibleCharacters = 0;
            _currentCustomer = null;

            _animator.SetBool(CUSTOMER_RESULT, true);
        }

        // 18) Animator -> () -> this -> Wait for player to press button
        public void ResultButtonOpened()
        {
            StartCoroutine(ButtonToActivate(_resultButton, _resultButtonText));
        }

        // 19) Waited for player to press button -> () -> CustomerManager
        public void ResultButtonPressed()
        {
            OnResultPressed?.Invoke();
            _animator.SetBool(CUSTOMER_RESULT, false);
        }

        private IEnumerator ButtonToActivate(Button button, TextMeshProUGUI buttonText)
        {
            yield return StartCoroutine(TypeText(buttonText));
            yield return new WaitForSeconds(0.5f);
            button.interactable = true;
        }

        private IEnumerator DialogueToButton()
        {
            yield return StartCoroutine(TypeText(_customerDialogue));
            yield return new WaitForSeconds(0.5f);
            if (_currentCustomer != null) _animator.SetBool(PLAYER_BUTTON, true);
            else _animator.SetTrigger(RESULT_BUTTON);
        }

        private IEnumerator TypeText(TextMeshProUGUI tmp)
        {
            int totalCharacters = tmp.textInfo.characterCount;

            for (int i = 0; i < totalCharacters; i++)
            {
                tmp.maxVisibleCharacters = i + 1;
                yield return new WaitForSeconds(_dialogueSpeed);
            }
        }
    }
}
