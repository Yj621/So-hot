using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Donghyun.Builder;
using static TotalMultiManager;
using Donghyun.Network;
using YJ.Ability;
using NUnit.Framework;
using System.Collections.Generic;
using DG.Tweening;
using System;
using UnityEngine.EventSystems;
using UnityEditor.Search;

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

    [Header("----- 스킬 설명 텍스트 -----")]
    public TextMeshProUGUI descriptionText;
    public GameObject descriptionPanel;
    private string[] skillDescriptions = new string[]
    {
        "불씨를 강하게 던질 수 있는 능력",
        "바닥에 떨어져도 일정 시간 동안 보호되는 능력",
        "뜨거움 게이지를 줄여주는 능력",
        "주변 아이템을 탐지하는 능력"
    };

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

    [Header("----- 리셋 버튼 -----")]
    public Button resetButton;

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

            AddHoverEvents(skillList[index], index);
        }

        for (int i = 0; i < characterList.Count; i++)
        {
            int index = i;
            characterList[index].onClick.AddListener(() => OnCharacterPick(index));
            characterList[index].onClick.AddListener(ToggleCharacterPick);
        }

        skillFrame.onClick.AddListener(ToggleSkillPick);
        characterFrame.onClick.AddListener(ToggleCharacterPick);
        resetButton.onClick.AddListener(ResetSelection);
    }

    //마우스 오버 이벤트
private void AddHoverEvents(Button button, int index)
{
    EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

    EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
    pointerEnter.eventID = EventTriggerType.PointerEnter;
    pointerEnter.callback.AddListener((data) => ShowDescription(index));
    trigger.triggers.Add(pointerEnter);

    EventTrigger.Entry pointerExit = new EventTrigger.Entry();
    pointerExit.eventID = EventTriggerType.PointerExit;
    pointerExit.callback.AddListener((data) => HideDescription());
    trigger.triggers.Add(pointerExit);
}

    private void ShowDescription(int index)
    {
        descriptionText.text = skillDescriptions[index];

        Vector3 mousePos = Input.mousePosition;
        Vector3 offset = new Vector3(150f, 0f, 0f);

        RectTransform rectTransform = descriptionPanel.GetComponent<RectTransform>();
        rectTransform.position = mousePos + offset;

        descriptionPanel.SetActive(true);

        descriptionPanel.GetComponent<Image>().raycastTarget = false;
    }

    private void HideDescription()
    {
        descriptionPanel.SetActive(false);
    }
    private void Update()
    {
        // 설명 UI가 활성화된 경우 마우스 위치에 따라 위치를 업데이트
        if (descriptionPanel.activeSelf)
        {
            Vector3 mousePosition = Input.mousePosition;
            descriptionPanel.GetComponent<RectTransform>().position = mousePosition + new Vector3(150f, 0f, 0f);
        }
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
        if (!openCharacterPick) //열기
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
    }

    public void ResetSelection()
    {
        curCharacterIndex = 0;
        curSkillIndex = 0;
        UpdateCharacterDisplay(curCharacterIndex);
        UpdateSkillDisplay(curSkillIndex);
    }
}