using UnityEngine;

namespace HoneyAndHemlock.Brewing
{
    public class Stirrer : MonoBehaviour
    {
        [SerializeField] private Transform _stirCalculationPoint;

        public Transform StirCalculationPoint => _stirCalculationPoint;
    }
}
