using PsychoticLab;
using System;
using UnityEngine;

namespace HoneyAndHemlock.Customers
{
    public class Customer : MonoBehaviour
    {
        private const string NEW_CUSTOMER = "NewCustomer";
        private const string REACHED_COUNTER = "ReachedCounter";
        private const string WAIT_FOR_POTION = "WaitForPotion";
        private const string RECEIVED_POTION = "ReceivedPotion";

        public Action OnFinishedLoop;

        [SerializeField] private Transform _visual;
        [SerializeField] private Animator _visualAnimator;
        [SerializeField] private Animator _customerAnimator;

        private void Awake()
        {
            _visual ??= transform.GetChild(0);
            _visualAnimator ??= _visual.GetComponent<Animator>();
            _customerAnimator ??= GetComponent<Animator>();
            CharacterRandomizer characterRandomizer = _visual.GetComponent<CharacterRandomizer>();
            characterRandomizer.Randomize();
        }

        public void StartCustomer()
        {
            _customerAnimator.SetTrigger(NEW_CUSTOMER);
        }

        public void ReachingCounter()
        {
            _visualAnimator.SetTrigger(REACHED_COUNTER);
        }

        public void WaitForPotion()
        {
            _visualAnimator.SetTrigger(WAIT_FOR_POTION);
        }

        public void ReceivedPotion()
        {
            _visualAnimator.SetTrigger(RECEIVED_POTION);
            _customerAnimator.SetTrigger(RECEIVED_POTION);
        }

        public void ReachingOutdoors()
        {
            OnFinishedLoop?.Invoke();
        }
    }
}

