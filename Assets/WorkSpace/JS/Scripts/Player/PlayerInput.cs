using JS.PlayerMove;
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
        Cursor.visible = false;
    }

    public void OnMove(InputValue value)
    {
        // InputValue에서 Vector2 값을 읽어서 Move() 호출
        Vector2 movementInput = value.Get<Vector2>();
        movement.Move(movementInput);
        if (movementInput != Vector2.zero )
        {
            SoundManager.Instance.PlayLoopSound(SoundManager.AudioType.PlayerWalk);
        }
        else
        {
            SoundManager.Instance.StopLoopSound(SoundManager.AudioType.PlayerWalk);
        }
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            movement.Jump();
            SoundManager.Instance.PlaySound(SoundManager.AudioType.PlayerJump);
            SoundManager.Instance.StopLoopSound(SoundManager.AudioType.PlayerWalk);
            SoundManager.Instance.StopLoopSound(SoundManager.AudioType.PlayerSprint);
        }
    }

    public void OnRun(InputValue value)
    {
        if (value.isPressed)
        {
            movement.SetRunning();
            SoundManager.Instance.PlayLoopSound(SoundManager.AudioType.PlayerSprint);
            SoundManager.Instance.StopLoopSound(SoundManager.AudioType.PlayerWalk);
        }
        else 
        {
            movement.StopRunning();
            SoundManager.Instance.StopLoopSound(SoundManager.AudioType.PlayerSprint);
            SoundManager.Instance.PlayLoopSound(SoundManager.AudioType.PlayerWalk);
            Debug.Log("뛰는 소리 멈춤");
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
            SoundManager.Instance.PlaySound(SoundManager.AudioType.PlayerThrow);
            Debug.Log("던지는 소리");
        }
    }

    public void OnUseItem(InputValue value)
    {
        if (value.isPressed)
        {
            movement.UseItem();
        }
    }
}
