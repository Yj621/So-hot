using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Donghyun.Builder;

public class ReadyManager : MonoBehaviourPunCallbacks
{
    public GameObject[] characters;
    public Sprite[] characterImages;
    public Image characterImage;
    

    private static int currentIndex = 0;
    private GameObject currentCharacter;
    private PhotonView pv;
    
    private int mySlotIndex;

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        if (pv.IsMine)
        {
            ResetSelection();
        }
    }

    public void OnLeftArrow()
    {
        if (!pv.IsMine) return;

        currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        pv.RPC("SyncCharacterSelection", RpcTarget.All, currentIndex);
        UpdateProperties();
    }

    public void OnRightArrow()
    {
        if (!pv.IsMine) return;
        currentIndex = (currentIndex + 1) % characters.Length;
        pv.RPC("SyncCharacterSelection", RpcTarget.All, currentIndex);
        UpdateProperties();
    }


    private void UpdateProperties()
    {
        CharacterType selectedCharacter = (CharacterType)currentIndex;

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
    {
        { "Character", selectedCharacter.ToString() }  // Enum 값을 문자열로 저장
    });

        SpawnManager.PlayerSetting.type = selectedCharacter;

        Debug.Log("Character updated: " + selectedCharacter);
    }


    [PunRPC]
    private void SyncCharacterSelection(int index)
    {
        currentIndex = index;
        UpdateCharacterDisplay();
    }



    private void UpdateCharacterDisplay()
    {

        if (currentCharacter != null)
        {
            currentCharacter.SetActive(false);
        }

        if (currentIndex >= 0)
        {
            currentCharacter = characters[currentIndex];
            currentCharacter.SetActive(true);
            characterImage.sprite = characterImages[currentIndex];
        }
    }

    
    private void ResetSelection()
    {
        if (currentCharacter != null)
        {
            currentCharacter.SetActive(false);
        }

        characters[0].SetActive(true);
        currentIndex = 0;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        pv.RPC("SyncCharacterSelection", RpcTarget.All, currentIndex);
    }
}