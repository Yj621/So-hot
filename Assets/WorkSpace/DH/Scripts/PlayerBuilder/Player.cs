using System;
using UnityEngine;


namespace Donghyun.Builder
{
    public enum PlayerState
    { 
        Red,
        Green, 
        Blue
    }

    [Serializable]
    public class State
    {
        public int playerNumber;
        public PlayerState state;
    }


    public class Player : MonoBehaviour
    {
        public State state;
    }
}
