using UnityEngine;

namespace KJ.Player
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerMovement movement;
        public PlayerState state;
        public Hotgauge hotgauge;
        public PlayerAnimationController animationController;

        private void Awake()
        {
            movement = GetComponent<PlayerMovement>();
            state = GetComponent<PlayerState>();
            hotgauge = GetComponent<Hotgauge>();
            animationController = GetComponent<PlayerAnimationController>();
        }
    }
}