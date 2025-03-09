using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Donghyun.Builder;
using static TotalMultiManager;
using Donghyun.Network;
using YJ.Ability;
using NUnit.Framework;
using System.Collections.Generic;
using DG.Tweening;
using System;

[Serializable]
public class AnimationInfo
{
    [Header("---- 애니메이션 시작 지점 -----")]
    public float start;
    [Header("---- 애니메이션 끝 지점 -----")]
    public float end;
    [Header("---- 애니메이션 실행 시간 -----")]
    public float duration;
    [Header("---- 애니메이션 그래프 타입 -----")]
    public Ease AnimationType;
}

public class ReadyManager : MonoBehaviourPunCallbacks
{

    public static ReadyManager Instance { get; private set; }

    [Header("----- 이미지들 -----")]
    public Sprite[] characterImages;
    public Sprite[] skillImages;

    [Header("----- 바뀔 이미지 UI -----")]
    public Image characterImage;
    public Image skillImage;

    [Header("----- 스킬 선택 버튼 -----")]
    public Button skillFrame;
    public RectTransform skillPick;
    public AnimationInfo skillPickInfo;
    public List<Button> skillList;

    [Header("----- 캐릭터 선택 버튼 -----")]
    public Button characterFrame;
    public RectTransform characterPick;
    public AnimationInfo characterPickInfo;
    public List<Button> characterList;


    private int curCharacterIndex = 0;
    private int curSkillIndex = 0;

    private bool openCharacterPick = false;
    private bool openSkillPick = false;

    private LobbyPlayer playerSetting;
    private PhotonView pv;

    public void SetPlayer(GameObject player)
    {
        playerSetting = player.GetComponent<LobbyPlayer>();
    }

    private void Awake()
    {
        skillPick.position = new Vector2(skillPick.position.x, skillPickInfo.start);
        characterPick.position = new Vector2(characterPick.position.x, characterPickInfo.start);

        Instance = this;

        pv = GetComponent<PhotonView>();

        for (int i = 0; i < skillList.Count; i++)
        {
            int index = i;
            skillList[index].onClick.AddListener(() => OnSkillPick(index));
            skillList[index].onClick.AddListener(ToggleSkillPick);
        }

        for (int i = 0; i < characterList.Count; i++)
        {
            int index = i;
            characterList[index].onClick.AddListener(() => OnCharacterPick(index));
            characterList[index].onClick.AddListener(ToggleCharacterPick);
        }

        skillFrame.onClick.AddListener(ToggleSkillPick);
        characterFrame.onClick.AddListener(ToggleCharacterPick);
    }

    public void OnCharacterPick(int index)
    {
        curCharacterIndex = index;
        UpdateCharacterDisplay(curCharacterIndex);
    }

    public void OnSkillPick(int index)
    {
        curSkillIndex = index;
        UpdateSkillDisplay(curSkillIndex);
    }

    public void ToggleCharacterPick()
    {
        if(!openCharacterPick) //열기
        {
            characterPick.DOAnchorPosY(characterPickInfo.end, characterPickInfo.duration).SetEase(characterPickInfo.AnimationType);
        }
        else //닫기
        {
            characterPick.DOAnchorPosY(characterPickInfo.start, characterPickInfo.duration).SetEase(characterPickInfo.AnimationType);
        }
        openCharacterPick = !openCharacterPick;
    }

    public void ToggleSkillPick()
    {
        if (!openSkillPick) //열기
        {
            skillPick.DOAnchorPosY(skillPickInfo.end, skillPickInfo.duration).SetEase(skillPickInfo.AnimationType);
        }
        else //닫기
        {
            skillPick.DOAnchorPosY(skillPickInfo.start, skillPickInfo.duration).SetEase(skillPickInfo.AnimationType);
        }
        openSkillPick = !openSkillPick;
    }

    private void UpdateCharacterDisplay(int index)
    {
        Debug.Log(index);
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