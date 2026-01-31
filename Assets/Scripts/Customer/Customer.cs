using PsychoticLab;
using System;
using UnityEngine;
using HoneyAndHemlock.Brewing;

namespace HoneyAndHemlock.Customers
{
    public class Customer : MonoBehaviour
    {
        private const string NEW_CUSTOMER = "NewCustomer";
        private const string REACHED_COUNTER = "ReachedCounter";
        private const string WAIT_FOR_POTION = "WaitForPotion";
        private const string RECEIVED_POTION = "ReceivedPotion";

        public Action OnReachedCounter;
        public Action<RecipeSO> OnReceivedPotion;
        public Action OnFinishedLoop;

        [SerializeField] private Transform _visual;
        [SerializeField] private Animator _visualAnimator;
        [SerializeField] private Animator _customerAnimator;

        private void Awake()
        {
            _visual ??= transform.GetChild(0);
            _visualAnimator ??= _visual.GetComponent<Animator>();
            _customerAnimator ??= GetComponent<Animator>();
        }

        void LateUpdate()
        {
            // TODO: Temporary fix for instantiated root motions
            // When Customer is in Hierarchy, it works perfect.
            // When Customer is Instantiated, the _visual seems to get unparented and animations break.
            // This forces visual to stay, but character slides feet and doesn't feel natural.
            // Customer's WalkOutPosition Animation is adjusted for this fix. Revisit if issue is fixed.
            _visual.localPosition = Vector3.zero;
        }

        // 2) CustomerManager -> () -> Animator
        public bool StartCustomer()
        {
            CharacterRandomizer characterRandomizer = _visual.GetComponent<CharacterRandomizer>();
            _customerAnimator.SetTrigger(NEW_CUSTOMER);
            return characterRandomizer.Randomize();
        }

        // 3) Animator -> () -> CustomerManager
        public void ReachingCounter()
        {
            _visualAnimator.SetTrigger(REACHED_COUNTER);
            OnReachedCounter?.Invoke();
        }

        // 10) CustomerManager -> () -> Wait for player to give potion
        public void WaitForPotion()
        {
            _visualAnimator.SetTrigger(WAIT_FOR_POTION);
        }

        // 11) Player gave potion -> () Walk out -> CustomerManager
        // TODO: Call this from XRSocketInteractable in Customer hand
        public void ReceivedPotion(RecipeSO potion)
        {
            _visualAnimator.SetTrigger(RECEIVED_POTION);
            _customerAnimator.SetTrigger(RECEIVED_POTION);
            OnReceivedPotion?.Invoke(potion);
        }

        // 14) CustomerCanvas -> () -> CustomerManager
        public void ReachingOutdoors()
        {
            OnFinishedLoop?.Invoke();
        }
    }
}

