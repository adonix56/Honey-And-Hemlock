using UnityEngine;

[CreateAssetMenu(fileName = "RespawnSO", menuName = "Honey && Hemlock/RespawnSO")]
public class RespawnSO : ScriptableObject
{
    [SerializeField] private GameObject _respawnPrefab;

    public GameObject RespawnPrefab => _respawnPrefab;
}
