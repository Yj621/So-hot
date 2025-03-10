using Photon.Pun;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MapVoice : VoiceManager
{
    // 맵씬에서는 매 프레임마다 "Player" 태그를 가진 오브젝트를 찾아 UI를 갱신합니다.
    protected override void LateUpdate()
    {
        base.LateUpdate(); // Speaker 목록 갱신

        // "Player" 태그가 달린 모든 오브젝트를 찾습니다.
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject playerObj in playerObjects)
        {
            // Player 오브젝트에서 Speaker 컴포넌트를 자식에서 찾습니다.
            Speaker speaker = playerObj.GetComponentInChildren<Speaker>();
            if (speaker == null)
                continue;

            // PhotonView를 통해 플레이어의 ActorNumber를 가져옵니다.
            PhotonView pv = playerObj.GetComponent<PhotonView>();
            if (pv == null)
                continue;

            // UI 업데이트를 위한 Image 컴포넌트를 자식에서 찾습니다.
            Image img = playerObj.GetComponentInChildren<Image>();
            if (img == null)
                continue;

            // 배열 인덱스 (예: 최대 4명 플레이어 가정)
            int index = pv.OwnerActorNr - 1;

            // 로컬 플레이어와 원격 플레이어 구분
            if (pv.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                // 로컬 플레이어: selfMuted 상태에 따라 이미지 변경
                img.sprite = selfMuted ? muteImage : defaultImage;
            }
            else
            {
                // 원격 플레이어: isMuted 배열과 Speaker의 IsPlaying으로 상태 판단
                img.sprite = isMuted[index] ? muteImage : (speaker.IsPlaying ? speakImage : defaultImage);
            }
        }
    }

}
