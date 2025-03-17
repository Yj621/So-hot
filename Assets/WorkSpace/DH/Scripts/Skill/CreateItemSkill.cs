using UnityEngine;
using YJ.Ability;

namespace Donghyun.Ability
{
    public class CreateItemSkill : SpecialAbility
    {
        public override void ExcuteSkill()
        {
            GameManager.Instance.player.GetComponentInChildren<Inventory>().GetItem(ItemManager.Instance.itemDatas[Random.Range(0, ItemManager.Instance.itemDatas.Length)]);
        }
    }

}
