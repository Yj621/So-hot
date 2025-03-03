using KJ.Player;
using System.Collections.Generic;
using UnityEngine;
using static ReadyManager;


namespace Donghyun.Builder
{
    public interface IPlayerBuider
    {
        void Character_Part(ref GameObject player, GameObject Character);
        void Animator_Part(ref GameObject player);
        void Skill_Part(ref GameObject player);
        void Effect_Part(ref GameObject player);
    }
    

    public class PlayerBuilder : MonoBehaviour, IPlayerBuider
    {
        [SerializeField] private RuntimeAnimatorController animationController;
        [SerializeField] private GameObject GaugeStop_Effect;
        [SerializeField] private GameObject NoDie_Effect;
        [SerializeField] private GameObject UnlimitRun_Effect;

        //플레이어의 캐릭터를 선택
        public void Character_Part(ref GameObject player, GameObject Character)
        {
            player = Character;
        }

        //플레이어 애니메이터 파츠
        public void Animator_Part(ref GameObject player)
        {
            player.GetComponent<Animator>().runtimeAnimatorController = animationController;
        }

        //플레이어 이펙트 파트
        public void Effect_Part(ref GameObject player)
        {
            Instantiate(GaugeStop_Effect, player.transform);
            Instantiate(NoDie_Effect, player.transform);
            Instantiate(UnlimitRun_Effect, player.transform);
        }

        //플레이어 스킬 파트
        public void Skill_Part(ref GameObject player)
        {
            return;
        }
    }
}