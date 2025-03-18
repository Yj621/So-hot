using JS.PlayerMove;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerInput : MonoBehaviourPun
{
    private PlayerMove movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMove>();
    }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            this.enabled = false;
            return;
        }
        Cursor.lockState = CursorLockMode.Locked; // 마우스 고정
        Cursor.visible = false;
    }

    public void OnMove(InputValue value)
    {
        if (!photonView.IsMine) return;

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
        if (!photonView.IsMine) return;

        if (value.isPressed)
        {
            movement.Jump();
        }
    }

    public void OnRun(InputValue value)
    {
        if (!photonView.IsMine) return;

        if (value.isPressed)
        {
            movement.SetRunning();
        }
        else 
        {
            movement.StopRunning();
            Debug.Log("뛰는 소리 멈춤");
        }
    }


    public void OnThrow(InputValue value)
    {
        if (!photonView.IsMine) return;

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
        if (!photonView.IsMine) return;

        if (value.isPressed)
        {
            movement.UseItem();
        }
    }
}
