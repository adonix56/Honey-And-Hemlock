using UnityEngine;

public class KillFloor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<RespawnableObject>(out RespawnableObject ro)) ro.RespawnMe();
        Destroy(other.gameObject);
    }
}
