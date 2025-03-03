using System.Collections.Generic;
using UnityEngine;


namespace Donghyun.Builder
{
    public interface IPlayerBuider
    {
        void Character_Part();
        void Animator_Part(PlayerState state);
        void Skill_Part();
        void State_Part(PlayerState state);
        GameObject Result();
    }
    

    public class PlayerBuilder : MonoBehaviour, IPlayerBuider
    {
        [SerializeField] private List<GameObject> playerTypes = new List<GameObject>(4);

        [SerializeField] private Animator animator;

        private GameObject player = new GameObject();

        //플레이어 생성 단계를 나눔

        //플레이어의 캐릭터를 선택
        public void Character_Part()
        {
            throw new System.NotImplementedException();
        }

        //플레이어 애니메이터 파츠
        public void Animator_Part(PlayerState state)
        {
            player.AddComponent<Animator>().runtimeAnimatorController = animator;
        }

        //플레이어 상태 파트
        public void State_Part(PlayerState state)
        {
            //
        }

        //다 붙여서 리턴
        public GameObject Result()
        {
            return player;
        }

        public void Skill_Part()
        {
            throw new System.NotImplementedException();
        }
    }
}