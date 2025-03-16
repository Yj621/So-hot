using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;
using static TotalMultiManager;

namespace YJ.Ability
{
    public enum SkillType
    {
        Fireball,
        Shield,
        HotChill,
        Detect
    }

    public abstract class SpecialAbility : MonoBehaviour
    {
        //실제 스킬 이미지
        [SerializeField] private Image skillImage;

        [SerializeField] private Sprite[] abilityImage;

        //쿨타임
        [SerializeField] private float coolTime = 30f;
        [SerializeField] private float maxCoolTime = 30f;

        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Image skillCoolTimeImage;

        // 쿨타임 실행 여부를 체크하는 플래그
        private bool isCooldownActive = false;

        void Start()
        {
            SkillUpdate();

            skillCoolTimeImage.fillAmount = 0f;
        }

        void Update()
        {
            // E 키를 눌렀고, 쿨타임이 진행 중이 아닐 때만 실행
            if (Input.GetKeyDown(KeyCode.E) && !isCooldownActive)
            {
                StartCoroutine(CoolTime());
            }
        }

        public abstract void ExcuteSkill();

        // 쿨타임 로직을 처리하는 코루틴
        IEnumerator CoolTime()
        {
            isCooldownActive = true; // 쿨타임 시작 플래그 설정

            skillCoolTimeImage.fillAmount = 1f;

            ExcuteSkill();

            while (coolTime > 0)
            {
                coolTime -= Time.deltaTime;

                // 스킬 쿨타임 UI 업데이트 (Fill Amount를 비율로 설정)
                skillCoolTimeImage.fillAmount = coolTime / maxCoolTime;

                // 2초 이상일 때는 정수, 2초 이하일 때는 소수점 1자리로 표시
                if (coolTime > 2f)
                {
                    timerText.text = $"{Mathf.CeilToInt(coolTime)}"; // 올림 처리로 정수로 표시
                }
                else
                {
                    timerText.text = $"{coolTime:F1}"; // 소수점 1자리로 표시
                }

                yield return null; // 매 프레임 업데이트
            }
            // 쿨타임 종료 후 초기화
            coolTime = maxCoolTime;
            skillCoolTimeImage.fillAmount = 0f;
            timerText.text = "  ";

            isCooldownActive = false; // 쿨타임 종료 플래그 해제
        }

        // 랜덤으로 능력을 부여하는 메서드
        private void SkillUpdate()
        {
            // 랜덤으로 능력 이름 선택
            int index = (int)GetTag(PhotonNetwork.LocalPlayer, "Skill");
            skillImage.sprite = abilityImage[index];
        }
    }
}
