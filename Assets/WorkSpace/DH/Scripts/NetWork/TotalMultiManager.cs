using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TotalMultiManager
{
    public static bool master() => PhotonNetwork.LocalPlayer.IsMasterClient;

    /// <summary>
    /// 플레이어의 액터 넘버를 반환
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static int ActorNum(Player player = null)
    {
        if (player == null) player = PhotonNetwork.LocalPlayer;
        return player.ActorNumber;
    }

    /// <summary>
    /// 삭제가 필요하면 마스터에서 삭제
    /// </summary>
    /// <param name="GO"></param>
    public static void Destroy(GameObject go)
    {
        PhotonNetwork.Destroy(go);
    }

    /// <summary>
    /// 키값과 밸류 값을 받아서 커스텀 프로퍼티로 저장
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="player"></param>
    public static void SetTag(string key, object value, Player player = null)
    {
        if (player == null) player = PhotonNetwork.LocalPlayer;
        player.SetCustomProperties(new Hashtable { { key, value } });
    }

    /// <summary>
    /// 해당 키값의 밸류 값을 리턴
    /// </summary>
    /// <param name="player"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static object GetTag(Player player, string key)
    {
        if (player.CustomProperties[key] == null) return null;
        return player.CustomProperties[key];
    }

    /// <summary>
    /// 해당 키를 해당 유저가 가지고 있는지 리턴
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool HasTag(string key, Player player = null)
    {
        if (player == null) player = PhotonNetwork.LocalPlayer;
        if (player.CustomProperties[key] == null) return false;
        else return true;
    }

    /// <summary>
    /// 방에 있는 모두가 해당 키를 가지고 있는지 리턴
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool AllhasTag(string key)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            if (PhotonNetwork.PlayerList[i].CustomProperties[key] == null) return false;
        return true;
    }

}
