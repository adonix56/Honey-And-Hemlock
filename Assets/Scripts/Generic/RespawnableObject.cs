using HoneyAndHemlock.Brewing;
using UnityEngine;

public class RespawnableObject : MonoBehaviour
{
    [SerializeField] private RespawnSO _respawnData;
    private Vector3 _startPos;
    private Quaternion _startRot;
    //private Vector3 _startScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startPos = transform.position;
        _startRot = transform.rotation;
        //_startScale = transform.localScale;
    }

    public void RespawnMe()
    {
        Debug.Log($"Respawning {name}");
        Transform parent = transform.parent;
        GameObject newMe = Instantiate(_respawnData.RespawnPrefab, _startPos, _startRot, parent);
        //if (newMe.TryGetComponent<ShrinkAndDestroy>(out ShrinkAndDestroy sad))
        //{
        //    Destroy(sad);
        //    newMe.transform.localScale = _startScale;
        //}
    }
}
