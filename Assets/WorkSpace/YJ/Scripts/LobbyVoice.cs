using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class LobbyVoice : VoiceManager
{
    [SerializeField] private GameObject[] players;  // 각 플레이어 GameObject
    [SerializeField] private TextMeshProUGUI[] playerTexts;  // 각 플레이어의 TextMeshProUGUI 배열

    [SerializeField] private Transform playerGroup; // PlayerGroup의 Transform


    private bool[] isMuted = new bool[4]; // 플레이어 음소거 상태

    protected override void LateUpdate()
    {
        base.LateUpdate(); // Speaker 목록 갱신

        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetActive(false);
        }

        foreach (var speaker in speakers)
        {
            PhotonView pv = speaker.GetComponent<PhotonView>();
            if (pv == null) continue;

            int index = pv.OwnerActorNr - 1;
            if (index < 0 || index >= players.Length) continue;

            players[index].SetActive(true);
            UpdatePlayerUI(index, speaker, pv);
        }
    }

    private void UpdatePlayerUI(int index, Speaker speaker, PhotonView pv)
    {
        Image img = playerTexts[index].GetComponentInChildren<Image>();

        if (pv.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            // 로컬 플레이어의 경우 selfMuted 상태를 반영
            img.sprite = selfMuted ? muteImage : defaultImage;
        }
        else
        {
            // 원격 플레이어의 경우 기존 로직 적용
            img.sprite = isMuted[index] ? muteImage : (speaker.IsPlaying ? speakImage : defaultImage);
        }
        Debug.Log($"speaker.IsPlaying : {speaker.IsPlaying}");

        // 닉네임 업데이트
        playerTexts[index].text = pv.Owner.NickName;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        int index = newPlayer.ActorNumber - 1;
        if (index >= 0 && index < players.Length)
        {
            players[index].SetActive(true);
            playerTexts[index].text = newPlayer.NickName;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        int index = otherPlayer.ActorNumber - 1;
        if (index >= 0 && index < players.Length)
        {
            players[index].SetActive(false);
            playerTexts[index].text = "";
        }
    }

}
