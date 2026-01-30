using System.Collections;
using UnityEngine;

namespace HoneyAndHemlock.Customers
{
    public class CustomerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _customerPrefab;
        [SerializeField] private float _secondsBetweenCustomers;

        private Customer _currentCustomer;

        private void Start()
        {
            StartCoroutine(SpawnNextCustomer());
        }

        private IEnumerator SpawnNextCustomer()
        {
            GameObject newCustomer = Instantiate(_customerPrefab, transform.position, transform.rotation, transform);
            _currentCustomer = newCustomer.GetComponent<Customer>();
            _currentCustomer.OnFinishedLoop += OnCustomerFinishedLoop;

            yield return new WaitForSeconds(_secondsBetweenCustomers);

            _currentCustomer.StartCustomer();
        }

        public void SetupWaitForPotion()
        {
            _currentCustomer.WaitForPotion();
        }

        public void SetupReceivedPotion()
        {
            _currentCustomer.ReceivedPotion();
        }

        private void OnCustomerFinishedLoop()
        {
            Destroy(_currentCustomer.gameObject);
            StartCoroutine(SpawnNextCustomer());
        }
    }
}