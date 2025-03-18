using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YJ.Ability;
using static TotalMultiManager;

namespace Donghyun.Ability
{
    public class SkillManager : MonoBehaviour
    {
        public static SkillManager Instance { get; private set; }

        //실제 스킬 이미지
        public Image skillImage;

        public Sprite[] abilityImage;

        //쿨타임
        public float coolTime;
        public float maxCoolTime;

        public TextMeshProUGUI timerText;
        public Image skillCoolTimeImage;

        public TMP_Text skillText;
        public string[] skillTextList;
        public string[] itemTextList;

        public SkillType SkillType { get; private set; }
        void Awake()
        {
            Instance = this;

            SkillType = (SkillType)GetTag(PhotonNetwork.LocalPlayer, "Skill");
            skillText.gameObject.SetActive(false);

            switch (SkillType)
            {
                case SkillType.Fireball:
                    gameObject.AddComponent<ThrowFireSkill>();
                    break;
                case SkillType.CreateItem:
                    gameObject.AddComponent<CreateItemSkill>();
                    break;
                case SkillType.Shield:
                    gameObject.AddComponent<ProtectFireSkill>();
                    break;
                case SkillType.HotChill:
                    gameObject.AddComponent<DecreaseFireGaugeSkill>();
                    break;
            }
        }
    }

}