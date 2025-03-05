using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Donghyun.Builder;
using static TotalMultiManager;
using Donghyun.Network;
using YJ.Ability;

public class ReadyManager : MonoBehaviourPunCallbacks
{
    public static ReadyManager Instance { get; private set; }

    [Header("----- 이미지들 -----")]
    public Sprite[] characterImages;
    public Sprite[] skillImages;

    [Header("----- 바뀔 이미지 UI -----")]
    public Image characterImage;
    public Image skillImage;

    private int curCharacterIndex = 0;
    private int curSkillIndex = 0;

    private int characterLength;
    private int skillLength;

    private LobbyPlayer playerSetting;
    private PhotonView pv;

    public void SetPlayer(GameObject player)
    {
        playerSetting = player.GetComponent<LobbyPlayer>();
    }

    private void Awake()
    {
        Instance = this;

        pv = GetComponent<PhotonView>();

        characterLength = characterImages.Length;
        skillLength = skillImages.Length;
    }

    public void OnCharacterLeftArrow()
    {
        curCharacterIndex = (curCharacterIndex - 1 + characterLength) % characterLength;
        UpdateCharacterDisplay(curCharacterIndex);
    }
    public void OnSkillLeftArrow()
    {
        curSkillIndex = (curSkillIndex - 1 + skillLength) % skillLength;
        UpdateSkillDisplay(curSkillIndex);
    }

    public void OnCharacterRightArrow()
    {
        curCharacterIndex = (curCharacterIndex + 1) % characterLength;
        UpdateCharacterDisplay(curCharacterIndex);
    }

    public void OnSkillRightArrow()
    {
        curSkillIndex = (curSkillIndex + 1) % skillLength;
        UpdateSkillDisplay(curSkillIndex);
    }

    private void UpdateCharacterDisplay(int index)
    {
        playerSetting.SetCharacterRPC(index);
        characterImage.sprite = characterImages[index];
    }

    private void UpdateSkillDisplay(int index)
    {
        skillImage.sprite = skillImages[index];
    }


    public void SetPlayerInfoRPC()
    {
        pv.RPC("SetPlayerInfo", RpcTarget.AllViaServer);
    }


    [PunRPC]
    private void SetPlayerInfo()
    {
        SkillType seledtedSkill = (SkillType)curCharacterIndex;
        CharacterType selectedCharacter = (CharacterType)curCharacterIndex;

        SetTag("Skill", seledtedSkill, PhotonNetwork.LocalPlayer);
        SetTag("Character", selectedCharacter, PhotonNetwork.LocalPlayer);

        SetTag("HasInfo", true);
        gameObject.SetActive(false);
        Debug.Log("Character updated: " + selectedCharacter);
    }

    public void ResetSelection()
    {
        curCharacterIndex = 0;
        curSkillIndex = 0;
        UpdateCharacterDisplay(curCharacterIndex);
        UpdateSkillDisplay(curSkillIndex);
    }
}