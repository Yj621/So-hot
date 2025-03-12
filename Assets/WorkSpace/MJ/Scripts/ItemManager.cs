using JS.PlayerMove;
using System.Collections;
using JetBrains.Annotations;
using YJ.UIManager;
using Photon.Pun;
using UnityEngine;

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
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [PunRPC]
    public void GaugeStop(PlayerMove player)
    {
        if (player.gaugeStopCoroutine != null)
        {
            StopCoroutine(player.gaugeStopCoroutine);
        }

        UIManager.Instance.gaugePause = true;

        int effectIdx = player.gameObject.GetComponent<Inventory>().effectNumber;

        // 변경된 RPC 호출 방식
        photonView.RPC("ItemEffectOn", RpcTarget.All, player.photonView.ViewID, effectIdx);

        StartCoroutine(CorGaugeStop(5f, player));
    }

    public void NoDie(PlayerMove player)
    {
        player.saveLife = true;

        int effectIdx = player.gameObject.GetComponent<Inventory>().effectNumber;

        photonView.RPC("ItemEffectOn", RpcTarget.All, player.photonView.ViewID, effectIdx);
    }

    public void UnlimitRun(PlayerMove player)
    {
        if (player.unlimitRunCoroutine != null)
        {
            StopCoroutine(player.unlimitRunCoroutine);
        }

        UIManager.Instance.runLimit = false;

        int effectIdx = player.gameObject.GetComponent<Inventory>().effectNumber;

        photonView.RPC("ItemEffectOn", RpcTarget.All, player.photonView.ViewID, effectIdx);

        player.unlimitRunCoroutine = StartCoroutine(CorUnlimitRun(5f, player));
    }

    [PunRPC]
    public void ItemEffectOn(int playerViewID, int idx)
    {
        GameObject playerObj = PhotonView.Find(playerViewID)?.gameObject;
        Debug.Log(playerObj.name);
        if (playerObj == null)
        {
            Debug.LogError($"Player with ViewID {playerViewID} not found!");
            return;
        }

        PlayerMove player = playerObj.GetComponent<PlayerMove>();
        if (player == null)
        {
            Debug.LogError($"PlayerMove component not found on player with ViewID {playerViewID}!");
            return;
        }

        player.ItemEffectOn(idx); // 이제 플레이어 오브젝트에서 실행됨
    }

    [PunRPC]
    void ItemEffectOff(int playerViewID, int idx)
    {
        GameObject playerObj = PhotonView.Find(playerViewID)?.gameObject;
        if (playerObj == null)
        {
            Debug.LogError($"Player with ViewID {playerViewID} not found!");
            return;
        }

        PlayerMove player = playerObj.GetComponent<PlayerMove>();
        if (player == null)
        {
            Debug.LogError($"PlayerMove component not found on player with ViewID {playerViewID}!");
            return;
        }

        player.ItemEffectOff(idx); // 플레이어 오브젝트에서 실행됨
    }


    IEnumerator CorUnlimitRun(float time, PlayerMove player)
    {
        int effectIdx = player.gameObject.GetComponent<Inventory>().effectNumber;
        yield return new WaitForSeconds(time);
        photonView.RPC("ItemEffectOff", RpcTarget.All, player.photonView.ViewID, effectIdx);
        UIManager.Instance.runLimit = true;
        player.unlimitRunCoroutine = null;
    }

    IEnumerator CorGaugeStop(float time, PlayerMove player)
    {
        int effectIdx = player.gameObject.GetComponent<Inventory>().effectNumber;
        yield return new WaitForSeconds(time);
        photonView.RPC("ItemEffectOff", RpcTarget.All, player.photonView.ViewID, effectIdx);
        UIManager.Instance.gaugePause = false;
        player.gaugeStopCoroutine = null;
    }
}
