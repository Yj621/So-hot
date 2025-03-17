using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using static TotalMultiManager;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance;

    public int playerNumber;

    public List<Transform> spawnPoints; //스폰 포인트
    public GameObject player; //캐릭터 생성 시점에 받아온 player obj
    private PhotonView gmPv; //GameManager의 포톤뷰 
    private PhotonView playerPv; //플레이어의 포톤뷰
    public bool[] deadPlayers; //플레이어 전원 죽음 상태 기록
    private bool allPlayerDead; //플레이어 전원 사망 여부 체크
    public Transform fireSavePoint; //현재 저장된 fire 세이브 포인트의 위치 정보
    public GameObject frontInventoryObj; //인벤토리 앞칸 UI 오브젝트
    public GameObject terminalInventoryObj; //인벤토리 앞칸 UI 오브젝트
    private float elapsedTime = 0f;
    private bool isRunning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        gmPv = GetComponent<PhotonView>();
    }
    void Start()
    {
        StartTimer();
    }

    public void SetPlayerPhotonView(GameObject newPlayer)
    {
        playerPv = newPlayer.GetComponentInChildren<PhotonView>();
        player = playerPv.gameObject;
        if (PhotonNetwork.IsMasterClient)
        {
            gmPv.RPC("Init", RpcTarget.AllViaServer);
        }
    }


    [PunRPC]
    public void Init()
    {
        Fire.Instance.isOnFire = true;
        Fire.Instance.isOnGround = false;
        Fire.Instance.timer = 5f;
        player.transform.position = spawnPoints[playerNumber].position;
        Fire.Instance.gameObject.transform.position = fireSavePoint.position;
    }


    [PunRPC]
    public void GameClear()
    {
        //TO-DO: 게임 클리어 연출이 명확해지면 수정
        StopTimer();
        SceneManager.LoadScene("EndingScene");
        Debug.Log("게임 클리어");
    }

    void AllPlayerRespawn()
    {
        SoundManager.Instance.PlaySound(SoundManager.AudioType.GameOver);
        for (int i = 0; i < deadPlayers.Length; i++)
        {
            deadPlayers[i] = false;
        }
        allPlayerDead = false;
        playerPv.RPC("InitInventory", RpcTarget.All);
        gmPv.RPC("Init", RpcTarget.AllViaServer);

    }

    public void PlayerRespawn(int i)
    {
        deadPlayers[i] = false;
        playerPv.RPC("InitInventory", RpcTarget.All);
        gmPv.RPC("Init", RpcTarget.AllViaServer);

    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (deadPlayers[0] && deadPlayers[1] && deadPlayers[2] && deadPlayers[3])
            {
                if (!allPlayerDead)
                {
                    allPlayerDead = true;
                    AllPlayerRespawn();
                }
            }
            if (!Fire.Instance.isOnFire)
            {
                AllPlayerRespawn();
            }
        }
        else return;

        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = false;
    }
}
