using UnityEngine;
using YJ.Ability;

namespace Donghyun.Ability
{
    public class ProtectFireSkill : SpecialAbility
    {
        public override void ExcuteSkill()
        {
            Fire.Instance.IncreseTimerRPC(5.0f);
        }
    }
}