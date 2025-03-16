using UnityEngine;
using YJ.Ability;
using YJ.UIManager;

namespace Donghyun.Ability
{
    public class ThrowFireSkill : SpecialAbility
    {
        public override void ExcuteSkill()
        {
            UIManager.Instance.maxThrow = 150.0f;
        }
    }

}