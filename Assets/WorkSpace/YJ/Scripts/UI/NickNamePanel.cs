using Photon.Pun;
using TMPro;
using UnityEngine;

public class NickNamePanel : MonoBehaviourPunCallbacks
{
    private TextMeshProUGUI nickNameText;
    private void Start()
    {
        nickNameText = GetComponentInChildren<TextMeshProUGUI>();
        PhotonView pv = transform.parent.GetComponent<PhotonView>();
        Debug.Log($"pv : {pv}");
        // 이 PhotonView가 본인 소유일 때만 닉네임을 업데이트
        if (pv.IsMine)
        {
            pv.RPC("SetNickNameRPC", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        }
    }
    [PunRPC]
    public void SetNickNameRPC(string nickName)
    {
        nickNameText.text = nickName;
    }

    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}
