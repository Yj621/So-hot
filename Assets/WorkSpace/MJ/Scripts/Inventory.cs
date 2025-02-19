using KJ.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    Queue<ItemData> inventory = new Queue<ItemData>(); //인벤토리
    PlayerController itemUser; //아이템 사용자(=플레이어 자신)
    [SerializeField] Image frontInven; //인벤토리 앞칸 UI
    [SerializeField] Image terminalInven; //인벤토리 뒷칸 UI

    private void Start()
    {
        itemUser = GetComponent<PlayerController>();
    }
    public void GetItem(ItemData item)
    {
            if (inventory.Count == 0)
            {
                inventory.Enqueue(item);
                frontInven.sprite = item.icon;
            }

            else if (inventory.Count == 1)
            {
                inventory.Enqueue(item);
                terminalInven.sprite = item.icon;
            }

    }
    public void UseItem()
    {
        if (inventory.Count > 0)
        {
            ItemData targetItem = inventory.Dequeue();
            switch (targetItem.itemType)
            {
                case ITEMTYPE.GaugeStop:
                    ItemManager.Instance.GaugeStop(itemUser);
                    IconUpdate();
                    break;

                case ITEMTYPE.NoDie:
                    ItemManager.Instance.NoDie(itemUser);
                    IconUpdate();
                    break;

                case ITEMTYPE.UnlimitRun:
                    ItemManager.Instance.UnlimitRun(itemUser);
                    IconUpdate();
                    break;

            }
        }
    }

    //아이템 사용 후, 인벤토리 UI 업데이트 시켜주는 함수
    void IconUpdate()
    {
        if (inventory.Count == 1)
        {
            frontInven.sprite = terminalInven.sprite;
            terminalInven.sprite = null;
        }
        else
        {
            frontInven.sprite = null;
        }
    }
}