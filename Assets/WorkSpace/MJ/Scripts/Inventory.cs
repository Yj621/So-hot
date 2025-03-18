using JS.PlayerMove;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using YJ.UIManager;
using Donghyun.Builder;
using Donghyun.Ability;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
using static TotalMultiManager;

public class Inventory : MonoBehaviour
{
    Queue<ItemData> inventory = new Queue<ItemData>(); //인벤토리
    PlayerMove itemUser; //아이템 사용자(=플레이어 자신)
    public int effectNumber; //사용 아이템의 이펙트 인덱스
    Image frontInven; //인벤토리 앞칸 UI 이미지 컴포넌트
    Image terminalInven; //인벤토리 뒷칸 UI 이미지 컴포넌트
    GameObject frontInventoryObj; //인벤토리 앞칸 UI 오브젝트
    GameObject terminalInventoryObj; //인벤토리 앞칸 UI 오브젝트
    public int n;

    private void Start()
    {
        SetTag("hasInventory", true);
        frontInventoryObj = GameManager.Instance.frontInventoryObj;
        terminalInventoryObj = GameManager.Instance.terminalInventoryObj;
        frontInven = frontInventoryObj.GetComponent<Image>();
        terminalInven = terminalInventoryObj.GetComponent<Image>();
        frontInventoryObj.SetActive(false);
        terminalInventoryObj.SetActive(false);
        StartCoroutine(FindPlayerControllerWithDelay());
    }

    private void Update()
    {
        n = inventory.Count;
    }
    

    IEnumerator FindPlayerControllerWithDelay()
    {
        yield return new WaitForSeconds(4f); // 네트워크 동기화가 완료될 시간을 확보
        itemUser = GetComponent<PlayerMove>();
    }

    public void GetItem(ItemData item)
    {
    
        SoundManager.Instance.PlaySound(SoundManager.AudioType.PlayerGetItem);
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
        Debug.Log($"현재 인벤토리에 존재하는 아이템의 개수는{inventory.Count}");
    }

    [PunRPC]
    public void InitInventory()
    {
        inventory.Clear();
        frontInven.sprite = null;
        terminalInven.sprite = null;
        frontInventoryObj.SetActive(false);
        terminalInventoryObj.SetActive(false);
        Debug.Log($"인벤토리 초기화, 아이템의 개수는{inventory.Count}");
    }


    public void UseItem()
    {
        Debug.Log($"inventory {inventory.Count}");
        if (inventory.Count > 0)
        {
            ItemData targetItem = inventory.Dequeue();
            effectNumber = targetItem.effectNum;

            SoundManager.Instance.PlaySound(SoundManager.AudioType.PlayerUsedItem);

            SkillManager.Instance.skillText.text = "아이템 사용 : " + SkillManager.Instance.itemTextList[(int)targetItem.itemType];
            SkillManager.Instance.skillText.gameObject.SetActive(true);

            switch (targetItem.itemType)
            {
                case ITEMTYPE.GaugeStop:
                    Debug.Log("GaugeStop 아이템 사용");
                    ItemManager.Instance.GaugeStop(itemUser);
                    IconUpdate();
                    break;

                case ITEMTYPE.NoDie:
                    Debug.Log("NoDie 아이템 사용");
                    ItemManager.Instance.NoDie(itemUser);
                    IconUpdate();
                    break;

                case ITEMTYPE.UnlimitRun:
                    Debug.Log("UnlimitRun 아이템 사용");
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
            Debug.Log("아이템 개수 1개, 아이콘 업데이트 완료");
        }
        else
        {
            frontInven.sprite = null;
            frontInventoryObj.SetActive(false);
            Debug.Log("아이템 개수 0 혹은 2개, 아이콘 업데이트 완료");
        }
    }
}