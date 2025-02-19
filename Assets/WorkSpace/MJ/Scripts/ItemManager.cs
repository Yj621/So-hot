using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using KJ.Player;
using System.Collections;
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

    public void GaugeStop(PlayerController player)
    {

        player.hotgauge.gaugePause = true;
        //photonView.RPC("ShowItemEffect", RpcTarget.All);

        //5초 뒤, 아이템 효과 해제
        StartCoroutine(ItemEndToFalse(5f, player.hotgauge.gaugePause));

    }

    public void NoDie(PlayerController player)
    {
        //죽음 면제 (true 상태일 때 죽는 상황이 오면, 죽기 대신 savelife를 false, 캐릭터 부활상태로 초기화)
        player.state.saveLife = true;
        //photonView.RPC("ShowItemEffect", RpcTarget.All);
    }

    public void UnlimitRun(PlayerController player)
    {
        player.movement.runLimit = false;
        //photonView.RPC("ShowItemEffect", RpcTarget.All);

        //5초 뒤, 아이템 효과 해제
        StartCoroutine(ItemEndToTrue(5f, player.movement.runLimit));
    }


    [PunRPC]
    void ShowItemEffect()
    {
        //TO-DO:로직 만들기
        //(파라미터값을 받아 해당 Player에게 붙어있는 이펙트 중 아이템에 맞는를 SetActive(true);)
    }


    //아이템 효과가 끝난 field가 true로 초기화 되어야 하는 경우에 사용하는 코루틴 
    IEnumerator ItemEndToTrue(float time, bool field)
    {
        yield return new WaitForSeconds(time); 
        field = true;
    }

    //아이템 효과가 끝난 field가 false로 초기화 되어야 하는 경우에 사용하는 코루틴 
    IEnumerator ItemEndToFalse(float time, bool field)
    {
        yield return new WaitForSeconds(time);
        field = false;
    }
}
