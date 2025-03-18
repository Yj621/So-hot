using Photon.Pun;
using TMPro;
using UnityEngine;

public class NickNamePanel : MonoBehaviourPunCallbacks
{
    private TextMeshProUGUI nickNameText;
    private void Start()
    {
        nickNameText = GetComponentInChildren<TextMeshProUGUI>();        
        // 이 PhotonView가 본인 소유일 때만 닉네임을 업데이트합니다.
        if (photonView.IsMine)
        {
            nickNameText.text = PhotonNetwork.LocalPlayer.NickName;
        }
    }

    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}
