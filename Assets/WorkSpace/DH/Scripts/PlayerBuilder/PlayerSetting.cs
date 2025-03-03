using System;
using UnityEngine;
using static ReadyManager;


namespace Donghyun.Builder
{
    public enum CharacterType
    {
        WomanWooga,
        ManWooga,
        OrangeWooga,
        MaskWooga
    }

    [Serializable]
    public class PlayerSetting : MonoBehaviour
    {
        public int playerNumber;
        public CharacterType type;
    }
}
