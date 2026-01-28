using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputPress : MonoBehaviour
{
    public InputAction inputAction;

    public UnityEvent OnPress;

    private void Awake()
    {
        inputAction.started += Pressed;
    }

    private void OnDestroy()
    {
        inputAction.started -= Pressed;
    }

    private void OnEnable()
    {
        inputAction.Enable();
    }

    private void OnDisable()
    {
        inputAction.Disable();
    }

    private void Pressed(InputAction.CallbackContext context)
    {
        OnPress.Invoke();
    }
}
