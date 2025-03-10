using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using JS.PlayerMove;
using System.Collections;
using JetBrains.Annotations;
using YJ.UIManager;

public enum ITEMTYPE
{
    GaugeStop,
    NoDie,
    UnlimitRun

}
public class ItemManager : MonoBehaviourPun
{
    public static ItemManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void GaugeStop(PlayerMove player)
    {
        //코루틴 중복 실행 방지
        if (player.gaugeStopCoroutine != null)
        {
            StopCoroutine(player.gaugeStopCoroutine);
        }

        UIManager.Instance.gaugePause = true;

        int effectIdx = player.gameObject.GetComponent<Inventory>().effectNumber;

        //아이템 이펙트 효과 보이기
        player.photonView.RPC("ItemEffectOn", RpcTarget.All, player.photonView.ViewID, effectIdx);

        //5초 뒤, 아이템 효과 해제
        player.gaugeStopCoroutine = StartCoroutine(CorGaugeStop(5f, player));

    }

    public void NoDie(PlayerMove player)
    {
        player.state.saveLife = true;

        int effectIdx = player.gameObject.GetComponent<Inventory>().effectNumber;

        //아이템 이펙트 효과 보이기
        player.photonView.RPC("ItemEffectOn", RpcTarget.All, player.photonView.ViewID, effectIdx);

        //NoDie의 경우, Effect 종료 및 사용 효과 종료는 플레이어의 죽음 시점이 되어야 하므로
        //플레이어에서 RPC ItemEffectOff를 호출해주어야 함 (effectIdx는 1이다)
    }

    public void UnlimitRun(PlayerMove player)
    {
        //코루틴 중복 실행 방지
        if (player.unlimitRunCoroutine != null)
        {
            StopCoroutine(player.unlimitRunCoroutine);
        }

        UIManager.Instance.runLimit = false;

        int effectIdx = player.gameObject.GetComponent<Inventory>().effectNumber;

        //아이템 이펙트 효과 보이기
        player.photonView.RPC("ItemEffectOn", RpcTarget.All, player.photonView.ViewID, effectIdx);

        //5초 뒤, 아이템 효과 해제
        player.unlimitRunCoroutine = StartCoroutine(CorUnlimitRun(5f, player));
    }


    [PunRPC]
    void ItemEffectOn(int playerViewID, int idx)
    {
        GameObject playerObj = PhotonView.Find(playerViewID).gameObject;
        PlayerController player = playerObj.GetComponent<PlayerController>();

        player.effectList[idx].SetActive(true);
    }

    [PunRPC]
    void ItemEffectOff(int playerViewID, int idx)
    {
        GameObject playerObj = PhotonView.Find(playerViewID).gameObject;
        PlayerController player = playerObj.GetComponent<PlayerController>();

        player.effectList[idx].SetActive(false);
    }


    //아이템 효과가 끝난 field가 true로 초기화 되어야 하는 경우에 사용하는 코루틴 
    IEnumerator CorUnlimitRun(float time, PlayerController player)
    {
        int effectIdx = player.gameObject.GetComponent<Inventory>().effectNumber;
        yield return new WaitForSeconds(time);
        player.photonView.RPC("ItemEffectOff", RpcTarget.All, player.photonView.ViewID, effectIdx);
        UIManager.Instance.runLimit = true;
        player.unlimitRunCoroutine = null;
    }

    //아이템 효과가 끝난 field가 false로 초기화 되어야 하는 경우에 사용하는 코루틴 
    IEnumerator CorGaugeStop(float time, PlayerMove player)
    {
        int effectIdx = player.gameObject.GetComponent<Inventory>().effectNumber;
        yield return new WaitForSeconds(time);
        player.photonView.RPC("ItemEffectOff", RpcTarget.All, player.photonView.ViewID, effectIdx);
        UIManager.Instance.gaugePause = false;
        player.gaugeStopCoroutine = null;
    }

}
