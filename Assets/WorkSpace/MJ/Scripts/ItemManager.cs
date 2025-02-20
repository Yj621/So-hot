using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using KJ.Player;
using System.Collections;
using JetBrains.Annotations;
public enum ITEMTYPE
{
    GaugeStop,
    NoDie,
    UnlimitRun

}
public class ItemManager : MonoBehaviourPun
{
    public static ItemManager Instance;
    Coroutine unlimitRunCoroutine; // 실행 중인 unlimit run 코루틴 저장
    Coroutine gaugeStopCoroutine; // 실행 중인 gauge stop 코루틴 저장
    private void Awake()
    {
        Instance = this;
    }

    public void GaugeStop(PlayerController player)
    {
        if (gaugeStopCoroutine != null)
        {
            StopCoroutine(gaugeStopCoroutine);
        }
        player.hotgauge.gaugePause = true;
        //photonView.RPC("ShowItemEffect", RpcTarget.All);

        //5초 뒤, 아이템 효과 해제
        gaugeStopCoroutine = StartCoroutine(CorGaugeStop(5f, player));

    }

    public void NoDie(PlayerController player)
    {
        //죽음 면제 (true 상태일 때 죽는 상황이 오면, 죽기 대신 savelife를 false, 캐릭터 부활상태로 초기화)
        player.state.saveLife = true;
        //photonView.RPC("ShowItemEffect", RpcTarget.All);
    }

    public void UnlimitRun(PlayerController player)
    {
        if (unlimitRunCoroutine != null)
        {
            StopCoroutine(unlimitRunCoroutine);
        }
        player.movement.runLimit = false;
        //photonView.RPC("ShowItemEffect", RpcTarget.All);

        //5초 뒤, 아이템 효과 해제
        unlimitRunCoroutine = StartCoroutine(CorUnlimitRun(5f, player));
    }


    [PunRPC]
    void ShowItemEffect()
    {
        //TO-DO:로직 만들기
        //(파라미터값을 받아 해당 Player에게 붙어있는 이펙트 중 아이템에 맞는를 SetActive(true);)
    }


    //아이템 효과가 끝난 field가 true로 초기화 되어야 하는 경우에 사용하는 코루틴 
    IEnumerator CorUnlimitRun(float time, PlayerController player)
    {
        yield return new WaitForSeconds(time); 
        player.movement.runLimit = true;
        unlimitRunCoroutine = null;
    }

    //아이템 효과가 끝난 field가 false로 초기화 되어야 하는 경우에 사용하는 코루틴 
    IEnumerator CorGaugeStop(float time, PlayerController player)
    {
        yield return new WaitForSeconds(time);
        player.hotgauge.gaugePause = false;
        gaugeStopCoroutine = null;
    }

}
