using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ReadyManager : MonoBehaviourPunCallbacks
{
    public Text playerNameText;
    public GameObject[] characters;
    public Sprite[] characterImages;
    public Image characterImage;
    public Sprite unknownCharacterSprite;
    public GameObject blackCharacter;

    private int currentIndex = -1;
    private GameObject currentCharacter;
    public int playerSlot = -1; // 플레이어 슬롯 (1P~4P)

    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerSlot"))
        {
            playerSlot = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerSlot"];
            Debug.Log($"내 슬롯 번호: {playerSlot + 1}P");
        }
        else
        {
            Debug.LogError("플레이어 슬롯 정보를 찾을 수 없습니다.");
        }

        ResetSelection();
    }

    public void OnLeftArrow()
    {
        if (!CanControl()) return;

        currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        UpdateCharacterDisplay();
    }

    public void OnRightArrow()
    {
        if (!CanControl()) return;

        currentIndex = (currentIndex + 1) % characters.Length;
        UpdateCharacterDisplay();
    }

    public void OnSelect()
    {
        if (!CanControl() || currentIndex == -1) return;

        Debug.Log($"{playerSlot + 1}P가 캐릭터 {characters[currentIndex].name}을(를) 선택함.");
    }

    public void OnCancel()
    {
        if (!CanControl()) return;

        ResetSelection();
    }

    private void UpdateCharacterDisplay()
    {
        blackCharacter.SetActive(false);

        if (currentCharacter != null)
        {
            currentCharacter.SetActive(false);
        }

        currentCharacter = characters[currentIndex];
        currentCharacter.SetActive(true);

        characterImage.sprite = characterImages[currentIndex];
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

    private bool CanControl()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber == playerSlot;
    }
}
