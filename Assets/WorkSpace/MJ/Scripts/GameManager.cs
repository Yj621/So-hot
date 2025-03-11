using UnityEngine;
using Photon.Pun;
using KJ.Player;
using UnityEngine.UIElements;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;
using static TotalMultiManager;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance;

    int playerNumber;

    public List<Transform> spawnPoints; //스폰 포인트(새로운 세이브 포인트에 trigger되면 spawnPoints[:4]를 해당 세이브 포인트들로 재할당)
    public GameObject player; //캐릭터 생성 시점에 받아온 player obj
    private PhotonView photonView; //player의 포톤뷰
    public Vector3 fireSavePoint; //현재 저장된 fire 세이브 포인트의 위치 정보(세이브 포인트에 trigger되면 본 필드값이 수정되어야 함)


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

        playerNumber = (int)GetTag(PhotonNetwork.LocalPlayer, "Number");

        //fireSavePoint = Fire.Instance.firstFirePos;

        //TO-DO: On game start, all player's start positions have to be saved at 'spawnPoint[:4]'

        photonView = player.GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("InitPlayerPos", RpcTarget.All);
        }

    }

    [PunRPC]
    void Init()
    {
        Fire.Instance.isOnFire = true;
        Fire.Instance.gameObject.transform.position = fireSavePoint;
        player.gameObject.transform.position = spawnPoints[playerNumber].position;
    }

    [PunRPC]
    public void GameClear()
    {
        //TO-DO: 게임 클리어 연출이 명확해지면 수정
        Debug.Log("게임 클리어");
    }

    void AllPlayerRespawn()
    {
        photonView.RPC("InitInventory", RpcTarget.All); 
        photonView.RPC("Init", RpcTarget.All);
    }

    //[PunRPC]
    //void PlayerDie(int playerViewID)
    //{
    //    GameObject playerObj = PhotonView.Find(playerViewID).gameObject;
    //    PlayerState playerState = playerObj.GetComponent<PlayerState>();
    //    playerState.Die();

    //    //playerViewID가 맞는 Player를 찾아 GameManager에서도 사망 처리
    //    if (playerObj == player1.gameObject) player1Dead = true;
    //    else if (playerObj == player2.gameObject) player2Dead = true;
    //    else if (playerObj == player3.gameObject) player3Dead = true;
    //    else if (playerObj == player4.gameObject) player4Dead = true;

    //    if (player1Dead && player2Dead && player3Dead && player4Dead)
    //    {
    //        photonView.RPC("AllPlayerRespawn", RpcTarget.All);
    //    }
    //    else
    //    {
    //        photonView.RPC("PlayerResurrection", RpcTarget.All, playerViewID);
    //    }

    //}

    //[PunRPC]
    //void PlayerResurrection(int playerViewID)
    //{

    //    GameObject playerObj = PhotonView.Find(playerViewID).gameObject;
    //    PlayerState playerState = playerObj.GetComponent<PlayerState>();
    //    Inventory playerInven = playerObj.GetComponent<Inventory>();
    //    playerInven.InitInventory();
    //    playerState.StartCoroutine(playerState.Revive());
    //}


}
