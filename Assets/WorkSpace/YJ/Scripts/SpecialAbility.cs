using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace YJ.Ability
{

    public class SpecialAbility : MonoBehaviour
    {
        //실제 스킬 이미지
        [SerializeField] private Image skillImage;
        //스킬 이름만 표시해주는 역할
        [SerializeField] private string skillName;

        [SerializeField] private string[] abilityName = new string[4] { "Fireball", "Shield", "HotChill", "Detect" };
        [SerializeField] private Sprite[] abilityImage;

        private Dictionary<string, Sprite> skill = new Dictionary<string, Sprite>();

        //쿨타임
        [SerializeField] private float coolTime = 30f;
        [SerializeField] private float maxCoolTime = 30f;

        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Image skillCoolTimeImage;

        // 쿨타임 실행 여부를 체크하는 플래그
        private bool isCooldownActive = false;

        void Start()
        {
            // 스킬 이름과 이미지를 딕셔너리에 매핑
            if (abilityName.Length == abilityImage.Length)
            {
                for (int i = 0; i < abilityName.Length; i++)
                {
                    skill.Add(abilityName[i], abilityImage[i]);
                }
                // 게임 시작 시 랜덤 능력 부여
                AssignRandomAbility();
            }
            else
            {
                // 스킬 이름과 이미지 배열의 길이가 다를 경우 경고 메시지 출력
                Debug.Log("뭔가 잘못됨");
            }
            skillCoolTimeImage.fillAmount = 0f;
        }

        void Update()
        {
            // E 키를 눌렀고, 쿨타임이 진행 중이 아닐 때만 실행
            if (Input.GetKeyDown(KeyCode.E) && !isCooldownActive)
            {
                StartCoroutine(CoolTime());
                skillCoolTimeImage.fillAmount = 1f;
            }
        }

        // 쿨타임 로직을 처리하는 코루틴
        IEnumerator CoolTime()
        {
            isCooldownActive = true; // 쿨타임 시작 플래그 설정

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
        private void AssignRandomAbility()
        {
            // 랜덤으로 능력 이름 선택
            int randomIndex = UnityEngine.Random.Range(0, abilityName.Length);
            string randomAbility = abilityName[randomIndex];

            // 선택한 능력으로 스킬 이미지 업데이트
            SkillUpdate(randomAbility);
        }

        // 특수 능력에 따라 스킬 이미지를 업데이트하는 메서드
        private void SkillUpdate(string ability)
        {
            if(skill.ContainsKey(ability))
            {
                skillImage.sprite = skill[ability];

                skillName = ability;
            }
            else
            {
                Debug.LogWarning($"'{ability}'는 유효한 능력 이름이 아닙니다.");
            }
        }
    }
}
