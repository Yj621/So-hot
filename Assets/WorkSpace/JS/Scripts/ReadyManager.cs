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
    private PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        ResetSelection();
    }

    public void OnLeftArrow()
    {
        if (!pv.IsMine) return;

        currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        UpdateCharacterDisplay();
    }

    public void OnRightArrow()
    {
        if (!pv.IsMine) return;

        currentIndex = (currentIndex + 1) % characters.Length;
        UpdateCharacterDisplay();
    }

    
    public void OnCancel()
    {
        if (!pv.IsMine) return;

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

}
