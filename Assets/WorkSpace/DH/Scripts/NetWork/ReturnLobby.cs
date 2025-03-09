using Photon.Pun;
using UnityEngine;
using static TotalMultiManager;

public class ReturnLobby : MonoBehaviour
{
    public void Return()
    {
        if(master()) PhotonNetwork.LoadLevel("LobbyScene");
    }
}
