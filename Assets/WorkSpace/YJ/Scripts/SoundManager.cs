using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // 재생 가능한 오디오 타입 열거형 정의
    public enum AudioType
    {
        PlayerWalk, PlayerSprint, PlayerThrow, PlayerDie,
        PlayerHot, PlayerUsedItem, PlayerUsedSkill, 
        Spear, FallRocks, Spikes, FallCherry, Bouncing, Water,
        SavePoint, GameOver, GameClear
    }
    // 버튼 클릭 및 마우스 오버 사운드
    public AudioSource uIClick;
    public AudioSource BackClick;
    public AudioSource ForwardClick;
    public AudioSource overSound;



    //사용법 : SoundManager.Instance.PlaySound(SoundManager.AudioType.PlayerJump);

    [System.Serializable]
    // 오디오 데이터를 담는 구조체
    public struct Audio
    {
        public AudioType type; // 오디오 타입
        public AudioSource audioSource; // 오디오 소스
    }
    public Audio[] audios; // 여러 오디오 데이터를 저장
    private Dictionary<AudioType, AudioSource> audioDic; // 오디오 타입과 소스를 매핑하는 딕셔너리

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        InitializeAudioDictionary();
    }

    // 오디오 딕셔너리 초기화
    private void InitializeAudioDictionary()
    {
        audioDic = new Dictionary<AudioType, AudioSource>();
        foreach (var audio in audios)
        {
            audioDic[audio.type] = audio.audioSource; // 오디오 타입을 키로 설정
        }
    }

    // 버튼 클릭 사운드 재생
    public void ButtonSound()
    {
        uIClick.Play();
        Debug.Log("버튼 클릭음 재생");
    }

    // 버튼 마우스 오버 사운드 재생
    public void ButtonOverSound()
    {
        overSound.Play();
    }

    // 특정 오디오 타입의 사운드 재생
    public void PlaySound(AudioType audioType)
    {
        if (audioDic.TryGetValue(audioType, out AudioSource audioSource))
        {
            Debug.Log("소리 재생");
            audioSource.Play(); // 다시 재생
        }
        else
        {
            Debug.Log($"{audioType}가 없습니다."); // 없는 타입에 대한 경고
        }
    }

    // 특정 오디오 타입의 사운드 정지
    public void StopSound(AudioType audioType)
    {
        if (audioDic.TryGetValue(audioType, out AudioSource audioSource))
        {
            audioSource.Stop();
        }
        else
        {
            Debug.Log($"{audioType}가 없습니다."); // 없는 타입에 대한 경고
        }
    }

    //꾹 눌러서 계속 나와야하는 애들
    public void PlayLoopSound(AudioType type)
    {
        if (audioDic.TryGetValue(type, out AudioSource source))
        {
            if (!source.isPlaying)
            {
                source.loop = true; // 루프 설정
                source.Play();
            }
        }
    }

    public void StopLoopSound(AudioType type)
    {
        if (audioDic.TryGetValue(type, out AudioSource source))
        {
            if (source.isPlaying)
            {
                source.loop = false; // 루프 해제
                source.Stop();
            }
        }
    }

}
