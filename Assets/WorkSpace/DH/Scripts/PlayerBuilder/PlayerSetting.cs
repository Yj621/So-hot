using System;


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
    public class PlayerSetting
    {
        public int playerNumber;
        public CharacterType type;

        public PlayerSetting(int _num, CharacterType _type)
        {
            playerNumber = _num;
            type = _type;
        }
    }
}
