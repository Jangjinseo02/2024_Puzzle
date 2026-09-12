using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public InputActionAsset dragControl;
    public InputAction dragStartAction;

    Vector2 mouseStartPos;
    Vector2 mouseEndPos;

    public Action<Vector2> OnDrag;

    private void Awake()
    {
        dragStartAction = dragControl.FindAction("Player/DragStart");
    }

    private void OnEnable()
    {
        dragStartAction.Enable();

        dragStartAction.started += OnDragStart;
        dragStartAction.canceled += OnDragEnd;
    }

    private void OnDisable()
    {
        dragStartAction.started -= OnDragStart;
        dragStartAction.canceled -= OnDragEnd;

        dragStartAction.Disable();
    }

    void OnDragStart(InputAction.CallbackContext context)
    {

        if (Pointer.current != null)
            mouseStartPos = Pointer.current.position.ReadValue();
    }

    void OnDragEnd(InputAction.CallbackContext context)
    {
        if (Pointer.current != null)
            mouseEndPos = Pointer.current.position.ReadValue();

        Vector2 dirVec = mouseEndPos - mouseStartPos;

        OnDrag?.Invoke(dirVec);
    }
}
