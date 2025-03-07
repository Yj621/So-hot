using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerMove movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMove>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 고정
    }

    public void OnMove(InputValue value)
    {
        // InputValue에서 Vector2 값을 읽어서 Move() 호출
        Vector2 movementInput = value.Get<Vector2>();
        movement.Move(movementInput);
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            movement.Jump();
        }
    }

    public void OnRun(InputValue value)
    {
        if (value.isPressed)
        {
            movement.SetRunning();
        }
        else 
        {
            movement.StopRunning();
        }
    }


    public void OnThrow(InputValue value)
    {
        if (value.isPressed)
        {
            movement.StartThrow();
        }
        else
        {
            movement.ReleaseThrow();
        }
    }
}
