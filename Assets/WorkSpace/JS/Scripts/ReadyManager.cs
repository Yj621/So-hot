using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Donghyun.Network;

public class ReadyManager : MonoBehaviourPunCallbacks
{
    public GameObject[] characters;
    public Sprite[] characterImages;
    public Image characterImage;
    public Sprite unknownCharacterSprite;
    public GameObject blackCharacter;

    private static int currentIndex = -1;
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
    }

    public void OnRightArrow()
    {
        if (!pv.IsMine) return;

        currentIndex = (currentIndex + 1) % characters.Length;
        pv.RPC("SyncCharacterSelection", RpcTarget.All, currentIndex);
    }

    public void OnCancel()
    {
        if (!pv.IsMine) return;

        pv.RPC("SyncCharacterSelection", RpcTarget.All, -1);
    }

    [PunRPC]
    private void SyncCharacterSelection(int index)
    {
        currentIndex = index;
        UpdateCharacterDisplay();
    }

    private void UpdateCharacterDisplay()
    {
        blackCharacter.SetActive(currentIndex == -1);

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
        else
        {
            characterImage.sprite = unknownCharacterSprite;
        }
    }

    private void ResetSelection()
    {
        if (currentCharacter != null)
        {
            currentCharacter.SetActive(false);
        }

        blackCharacter.SetActive(true);
        currentIndex = -1;
        characterImage.sprite = unknownCharacterSprite;
    }


}