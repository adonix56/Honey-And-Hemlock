using HoneyAndHemlock.Brewing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoneyAndHemlock.Customers
{
    public class CustomerManager : MonoBehaviour
    {
        private const string OPEN_DOOR = "OpenDoor";

        [SerializeField] private GameObject _customerPrefab;
        [SerializeField] private float _secondsBetweenCustomers;
        [SerializeField] private CustomerCanvas _customerCanvas;
        [SerializeField] private List<CustomerSO> _customers;
        [SerializeField] private Animator _animator;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _openDoor;
        [SerializeField] private AudioClip _closeDoor;

        private Customer _currentCustomer;
        private int _customerIdx;
        private int _storyIdx;
        private bool _lastResultClosed;

        private void Start()
        {
            _customerCanvas.OnPlayerResponsePressed += OnWaitForPotion;
            _customerCanvas.OnResultPressed += OnResultClosed;
            _audioSource ??= GetComponent<AudioSource>();
            StartCoroutine(SpawnNextCustomer(true));
        }

        // 1) Spawns Customer -> Customer
        // 16) Last Customer Ended -> CustomerCanvas (Display Results)
        private IEnumerator SpawnNextCustomer(bool firstCustomer = false)
        {
            GameObject newCustomer = Instantiate(_customerPrefab, transform.position, transform.rotation, transform);
            _currentCustomer = newCustomer.GetComponent<Customer>();
            _currentCustomer.OnReachedCounter += OnCustomerReachedCounter;
            _currentCustomer.OnReceivedPotion += OnReceivedPotion;
            _currentCustomer.OnFinishedLoop += OnCustomerFinishedLoop;
            _currentCustomer.OnOpenDoor += OpenDoor;

            if (!firstCustomer)
            {
                _customerCanvas.DisplayResults();
                yield return new WaitUntil(() => _lastResultClosed);
            }

            yield return new WaitForSeconds(_secondsBetweenCustomers);

            _lastResultClosed = false;
            _customerIdx = firstCustomer ? 0 : Random.Range(0, _customers.Count);
            _storyIdx = _currentCustomer.StartCustomer() ? 0 : 1;
            OpenDoor();
        }

        // 4) Customer -> () -> CustomerCanvas
        private void OnCustomerReachedCounter()
        {
            _customerCanvas.StartCustomerRequest(_customers[_customerIdx], _storyIdx);
        }

        // 9) CustomerCanvas -> () -> Customer
        private void OnWaitForPotion()
        {
            _currentCustomer.WaitForPotion();
        }

        // 12) Customer -> () -> CustomerCanvas
        private void OnReceivedPotion(RecipeSO potion)
        {
            _customerCanvas.ReceivedPotion(potion);
        }

        // 15) Customer -> () -> this.SpawnNextCustomer() 
        private void OnCustomerFinishedLoop()
        {
            Destroy(_currentCustomer.gameObject);
            StartCoroutine(SpawnNextCustomer());
        }

        // 20) CustomerCanvas -> () END
        private void OnResultClosed()
        {
            _lastResultClosed = true;
        }

        public void OpenDoor()
        {
            _animator.SetTrigger(OPEN_DOOR);
        }

        public void PlayOpenDoor()
        {
            if (_audioSource != null && _openDoor != null) _audioSource.PlayOneShot(_openDoor);
        }

        public void PlayCloseDoor()
        {
            if (_audioSource != null && _closeDoor != null) _audioSource.PlayOneShot(_closeDoor);
        }
    }
}