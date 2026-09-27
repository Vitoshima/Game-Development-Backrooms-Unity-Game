using UnityEngine;
using UnityEngine.InputSystem;

public class WebGLCursorFix : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference attackAction;

    private void OnEnable()
    {
        interactAction?.action.Enable();
        attackAction?.action.Enable();
    }

    private void OnDisable()
    {
        interactAction?.action.Disable();
        attackAction?.action.Disable();
    }

    void Update()
    {
        // === ESCAPE - Unlock Cursor ===
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UnlockCursor();
            return;
        }

        // === CLICK - Re-lock Cursor ===
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            bool clicked = false;

            if (attackAction != null && attackAction.action.WasPressedThisFrame())
                clicked = true;
            else if (interactAction != null && interactAction.action.WasPressedThisFrame())
                clicked = true;

            if (clicked)
            {
                LockCursor();
            }
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}