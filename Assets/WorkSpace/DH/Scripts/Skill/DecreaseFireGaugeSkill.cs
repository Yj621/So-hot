using Photon.Pun;
using System;
using UnityEngine;
using YJ.Ability;
using YJ.UIManager;

namespace Donghyun.Ability
{
    public class DecreaseFireGaugeSkill : SpecialAbility
    {
        public override void ExcuteSkill()
        {
            UIManager.Instance.DecreaseHeat(30.0f);
        }
    }
}