using KJ.Player;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static ReadyManager;


namespace Donghyun.Builder
{
    public interface IPlayerBuider
    {
        void Character_Part(CharacterType type);
        void Animator_Part(GameObject player);
        void Skill_Part();
        void Effect_Part();
        List<GameObject> Return_Parts();
    }

    [Serializable]
    public class Character
    {
        [Header("----- Waman -----")]
        public List<GameObject> womanWooga = new List<GameObject>();
        [Header("----- Man -----")]
        public List<GameObject> manWooga = new List<GameObject>();
        [Header("----- Orange -----")]
        public List<GameObject> orangeWooga = new List<GameObject>();
        [Header("----- Mask -----")]
        public List<GameObject> maskWooga = new List<GameObject>();
    }


    public class PlayerBuilder : MonoBehaviour, IPlayerBuider
    {
        [SerializeField] private RuntimeAnimatorController animationController;
        [SerializeField] private List<GameObject> effects = new List<GameObject>();
        //[SerializeField] private Character characterParts = new Character();

        private List<GameObject> parts = new List<GameObject>();


        private void Awake()
        {
        }

        //플레이어의 캐릭터를 선택
        public void Character_Part(CharacterType type)
        {
        }

        //플레이어 애니메이터 파츠
        public void Animator_Part(GameObject player)
        {
            player.GetComponent<Animator>().runtimeAnimatorController = animationController;
        }

        //플레이어 이펙트 파트
        public void Effect_Part()
        {
            foreach (GameObject part in effects)
            {
                parts.Add(part);
            }
        }

        //플레이어 스킬 파트
        public void Skill_Part()
        {
            return;
        }

        public List<GameObject> Return_Parts()
        {
            return parts;
        }
    }
}