using Donghyun.Ability;
using Photon.Pun;
using System;
using System.Collections;
using TMPro;
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
        // 쿨타임 실행 여부를 체크하는 플래그
        private bool isCooldownActive = false;

        //쿨타임 관련 변수
        private float coolTime;
        private float maxCoolTime;

        //스킬 매니저 캐싱
        protected SkillManager skillManager;

        private SkillType skillType;

        void Awake()
        {
            skillManager = SkillManager.Instance;
            coolTime = skillManager.coolTime;
            maxCoolTime = skillManager.maxCoolTime;
            skillType = skillManager.SkillType;

            SkillUpdate();

            skillManager.skillCoolTimeImage.fillAmount = 0f;
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

        // 랜덤으로 능력을 부여하는 메서드
       private void SkillUpdate()
        {
            // 랜덤으로 능력 이름 선택
            skillManager.skillImage.sprite = skillManager.abilityImage[(int)skillType];
        }

        // 쿨타임 로직을 처리하는 코루틴
        IEnumerator CoolTime()
        {
            isCooldownActive = true; // 쿨타임 시작 플래그 설정

            skillManager.skillCoolTimeImage.fillAmount = 1f;

            ExcuteSkill();

            while (coolTime > 0)
            {
                coolTime -= Time.deltaTime;

                // 스킬 쿨타임 UI 업데이트 (Fill Amount를 비율로 설정)
                skillManager.skillCoolTimeImage.fillAmount = coolTime / maxCoolTime;

                // 2초 이상일 때는 정수, 2초 이하일 때는 소수점 1자리로 표시
                if (coolTime > 2f)
                {
                    skillManager.timerText.text = $"{Mathf.CeilToInt(coolTime)}"; // 올림 처리로 정수로 표시
                }
                else
                {
                    skillManager.timerText.text = $"{coolTime:F1}"; // 소수점 1자리로 표시
                }

                yield return null; // 매 프레임 업데이트
            }
            // 쿨타임 종료 후 초기화s
            coolTime = maxCoolTime;
            skillManager.skillCoolTimeImage.fillAmount = 0f;
            skillManager.timerText.text = "  ";

            isCooldownActive = false; // 쿨타임 종료 플래그 해제
        }
    }
}
