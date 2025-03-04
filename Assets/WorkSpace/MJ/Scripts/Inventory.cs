using KJ.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    Queue<ItemData> inventory = new Queue<ItemData>(); //인벤토리
    PlayerController itemUser; //아이템 사용자(=플레이어 자신)
    public int effectNumber; //사용 아이템의 이펙트 인덱스
    [SerializeField] Image frontInven; //인벤토리 앞칸 UI
    [SerializeField] Image terminalInven; //인벤토리 뒷칸 UI

    private void Start()
    {
        StartCoroutine(FindPlayerControllerWithDelay());
    }

    IEnumerator FindPlayerControllerWithDelay()
    {
        yield return new WaitForSeconds(5f); // 네트워크 동기화가 완료될 시간을 확보
        itemUser = GetComponent<PlayerController>();

        if (itemUser == null)
        {
            Debug.LogError("PlayerController를 찾을 수 없음!");
        }
    }

    public void GetItem(ItemData item)
    {
        if (inventory.Count == 0)
        {
            inventory.Enqueue(item);
            frontInven.sprite = item.icon;
            frontInven.gameObject.SetActive(true);
        }

        else if (inventory.Count == 1)
        {
            inventory.Enqueue(item);
            terminalInven.sprite = item.icon;
            terminalInven.gameObject.SetActive(true);
        }

    }

    public void InitInventory()
    {
        inventory.Clear();
        frontInven.sprite = null;
        terminalInven.sprite = null;
        frontInven.gameObject.SetActive(false);
        terminalInven.gameObject.SetActive(false);

    }

    public void UseItem()
    {
        if (inventory.Count > 0)
        {
            ItemData targetItem = inventory.Dequeue();
            effectNumber = targetItem.effectNum;
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
            terminalInven.gameObject.SetActive(false);
        }
        else
        {
            frontInven.sprite = null;
            frontInven.gameObject.SetActive(false);
        }
    }
}