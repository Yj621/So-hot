using JS.PlayerMove;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using YJ.UIManager;

public class Inventory : MonoBehaviour
{
    Queue<ItemData> inventory = new Queue<ItemData>(); //인벤토리
    PlayerMove itemUser; //아이템 사용자(=플레이어 자신)
    public int effectNumber; //사용 아이템의 이펙트 인덱스
    [SerializeField] Image frontInven; //인벤토리 앞칸 UI 이미지 컴포넌트
    [SerializeField] Image terminalInven; //인벤토리 뒷칸 UI 이미지 컴포넌트
    GameObject frontInventoryObj; //인벤토리 앞칸 UI 오브젝트
    GameObject terminalInventoryObj; //인벤토리 앞칸 UI 오브젝트

    private void Start()
    {
        frontInventoryObj = GameObject.Find("Front");
        terminalInventoryObj = GameObject.Find("Terminal");
        frontInven = frontInventoryObj.GetComponent<Image>();
        terminalInven = terminalInventoryObj.GetComponent<Image>();
        frontInventoryObj.SetActive(false);
        terminalInventoryObj.SetActive(false);

        StartCoroutine(FindPlayerControllerWithDelay());
    }

    IEnumerator FindPlayerControllerWithDelay()
    {
        yield return new WaitForSeconds(5f); // 네트워크 동기화가 완료될 시간을 확보
        itemUser = GetComponent<PlayerMove>();
    }

    public void GetItem(ItemData item)
    {
        if (inventory.Count == 0)
        {
            inventory.Enqueue(item);
            frontInventoryObj.SetActive(true);
            frontInven.sprite = item.icon;
        }

        else if (inventory.Count == 1)
        {
            inventory.Enqueue(item);
            terminalInventoryObj.SetActive(true);
            terminalInven.sprite = item.icon;

        }

    }

    [PunRPC]
    public void InitInventory()
    {
        inventory.Clear();
        frontInven.sprite = null;
        terminalInven.sprite = null;
        frontInventoryObj.SetActive(false);
        terminalInventoryObj.SetActive(false);
    }


    public void UseItem()
    {
        if (inventory.Count > 0)
        {
            ItemData targetItem = inventory.Dequeue();
            effectNumber = targetItem.effectNum;

            SoundManager.Instance.PlaySound(SoundManager.AudioType.PlayerUsedItem);
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
            terminalInventoryObj.SetActive(false);
        }
        else
        {
            frontInven.sprite = null;
            frontInventoryObj.SetActive(false);
        }
    }
}