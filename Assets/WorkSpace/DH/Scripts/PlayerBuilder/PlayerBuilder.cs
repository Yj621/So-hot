using KJ.Player;
using System.Collections.Generic;
using UnityEngine;
using static ReadyManager;


namespace Donghyun.Builder
{
    public interface IPlayerBuider
    {
        void Character_Part(CharacterType type);
        void Animator_Part();
        void Skill_Part();
        void Effect_Part();
        GameObject Result();
    }
    

    public class PlayerBuilder : MonoBehaviour, IPlayerBuider
    {
        [SerializeField] private List<GameObject> playerTypes = new List<GameObject>(4);

        [SerializeField] private RuntimeAnimatorController animationController;
        [SerializeField] private GameObject GaugeStop_Effect;
        [SerializeField] private GameObject NoDie_Effect;
        [SerializeField] private GameObject UnlimitRun_Effect;

        private GameObject player;

        //플레이어의 캐릭터를 선택
        public void Character_Part(CharacterType type)
        {
            player = playerTypes[(int)type];
        }

        //플레이어 애니메이터 파츠
        public void Animator_Part()
        {
            player.AddComponent<Animator>().runtimeAnimatorController = animationController;
        }

        //플레이어 이펙트 파트
        public void Effect_Part()
        {
            Instantiate(GaugeStop_Effect, player.transform);
            Instantiate(NoDie_Effect, player.transform);
            Instantiate(UnlimitRun_Effect, player.transform);
        }

        //플레이어 스킬 파트
        public void Skill_Part()
        {
            return;
        }

        //다 붙여서 리턴
        public GameObject Result()
        {
            return player;
        }
    }
}