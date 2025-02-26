using Photon.Pun;
using UnityEngine;

public class Fire : MonoBehaviourPun
{
    public static Fire Instance;
    public bool isOnFire; //불이 켜져있는지 확인하는 bool 값
    public Vector3 firstFirePos; //불이 처음 로드될 때의 position 값

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }

        isOnFire = true;
        firstFirePos = gameObject.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            GameManager.Instance.photonView.RPC("GameClear", RpcTarget.All);
        }
        else if (other.CompareTag("Water"))
        {
            isOnFire = false;
            GameManager.Instance.photonView.RPC("AllPlayerRespawn", RpcTarget.All);

        }
        //TO-DO: 불이 바닥에 오래 떨어져있으면, 불이 꺼지고 All Player Respawn 되는 if문 추가
    }

    
}
