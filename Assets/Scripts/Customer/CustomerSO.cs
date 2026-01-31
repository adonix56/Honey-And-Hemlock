using HoneyAndHemlock.Brewing;
using UnityEngine;

namespace HoneyAndHemlock.Customers
{
    [CreateAssetMenu(fileName = "NewCustomer", menuName = "Honey && Hemlock/CustomerSO")]
    public class CustomerSO : ScriptableObject
    {
        [SerializeField] private RecipeSO _requestedPotion;
        [SerializeField] private string[] _request;
        [SerializeField] private string[] _playerResponse;
        [SerializeField] private string[] _successfulResponse;
        [SerializeField] private string[] _failedResponse;
        [SerializeField] private string[] _cosmicResponse;

        public RecipeSO RequestedPotion => _requestedPotion;
        public string[] Request => _request;
        public string[] PlayerResponse => _playerResponse;
        public string[] SuccessfulResponse => _successfulResponse;
        public string[] FailedResponse => _failedResponse;
        public string[] CosmicResponse => _cosmicResponse;
    }
}
