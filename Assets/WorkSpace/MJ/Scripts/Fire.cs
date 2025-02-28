using Photon.Pun;
using UnityEngine;

public class Fire : MonoBehaviourPun
{
    public static Fire Instance;
    public bool isOnFire; //불이 켜져있는지 확인하는 bool 값
    public bool isOnGround; //불이 바닥에 떨어져있는지 여부 확인하는 bool 값
    public float timer = 5f; //바닥에 떨어졌을 때의 isOnFire 유지 시간
    public Vector3 firstFirePos; //불이 처음 로드될 때의 position 값

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        isOnFire = true;
        firstFirePos = gameObject.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            GameManager.Instance.photonView.RPC("GameClear", RpcTarget.All);
        }
        else if (other.CompareTag("Water"))
        {
            FireOff();
        }
        else if (other.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            timer = 5f;
            isOnGround = false;
        }
    }

    void FireOff()
    {
        //중복 호출 방지(불이 꺼져있다면, 메서드 동작 종료)
        if (!isOnFire) return;

        isOnFire = false;
        GameManager.Instance.photonView.RPC("AllPlayerRespawn", RpcTarget.All);
    }

    private void Update()
    {
        if(isOnGround)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                FireOff();
                timer = 5f;
                isOnGround = false;
            }

        }
    }
}
