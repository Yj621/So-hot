using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer; // 오디오 믹서를 관리할 AudioMixer 객체

    // 사운드 볼륨을 조절하기 위한 UI 슬라이더
    [SerializeField] private Slider masterVolumeSlider; // 마스터 볼륨 슬라이더
    [SerializeField] private Slider bgmVolumeSlider;    // 배경음 볼륨 슬라이더
    [SerializeField] private Slider sfxVolumeSlider;    // 효과음 볼륨 슬라이더

    // 토글 (사운드 온/오프)
    [SerializeField] private Toggle muteToggle;

    public static AudioMixerManager instance { get; private set; } // 싱글톤 패턴을 위한 인스턴스

    private void Awake()
    {
        // 싱글톤 초기화
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 오브젝트를 씬 전환 시 제거하지 않음
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 방지를 위해 제거
        }
    }

    private void Start()
    {
        // 이전에 저장된 슬라이더 값을 불러옴. 값이 없으면 기본값(1)으로 설정
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // AudioMixer에 저장된 볼륨 값을 슬라이더 값으로 변환하여 초기화
        audioMixer.GetFloat("MasterVolume", out float masterDb);
        masterVolumeSlider.value = Mathf.Pow(10, masterDb / 20); // 데시벨 값을 슬라이더 값으로 변환
        audioMixer.GetFloat("BGMVolume", out float bgmDb);
        bgmVolumeSlider.value = Mathf.Pow(10, bgmDb / 20);
        audioMixer.GetFloat("SFXVolume", out float sfxDb);

        sfxVolumeSlider.value = Mathf.Pow(10, sfxDb / 20);
        // 토글 초기화
        bool isMuted = PlayerPrefs.GetInt("MasterMute", 1) == 0;
        muteToggle.isOn = isMuted;


        // 슬라이더 값 변경 시 호출될 이벤트 리스너를 등록
        masterVolumeSlider.onValueChanged.AddListener((value) => {
            SetMasterVolume(value);
        });
        bgmVolumeSlider.onValueChanged.AddListener((value) => {
            SetBGMVolume(value);
        });
        sfxVolumeSlider.onValueChanged.AddListener((value) => {
            SetSFXVolume(value);
        });


        // 토글 이벤트 등록
        muteToggle.onValueChanged.AddListener(SetMasterMute);


        // 초기 볼륨 설정
        SetMasterVolume(masterVolumeSlider.value);
        SetBGMVolume(bgmVolumeSlider.value);
        SetSFXVolume(sfxVolumeSlider.value);
    }

    // 전체 볼륨 설정
    public void SetMasterVolume(float volume)
    {
        // 슬라이더 값을 데시벨 값으로 변환
        float dbVolume = (volume > 0.0001f) ? Mathf.Log10(volume) * 20 : -80f;
        audioMixer.SetFloat("MasterVolume", dbVolume); // AudioMixer에 설정
        PlayerPrefs.SetFloat("MasterVolume", volume);  // 설정값 저장
        PlayerPrefs.Save(); // 저장된 값 즉시 디스크에 기록

        // AudioMixer 값을 다시 슬라이더에 반영 (변환 과정 확인)
        audioMixer.GetFloat("MasterVolume", out float newDbVolume);
        masterVolumeSlider.value = Mathf.Pow(10, newDbVolume / 20);
    }

    // 배경음 볼륨 설정
    public void SetBGMVolume(float volume)
    {
        if (!muteToggle.isOn)
            return; // 음소거 상태에서는 볼륨을 변경하지 않음

        float dbVolume = (volume > 0.0001f) ? Mathf.Log10(volume) * 20 : -80f;
        audioMixer.SetFloat("BGMVolume", dbVolume);
        PlayerPrefs.SetFloat("BGMVolume", volume);
        PlayerPrefs.Save();
        audioMixer.GetFloat("BGMVolume", out float newDbVolume);
        bgmVolumeSlider.value = Mathf.Pow(10, newDbVolume / 20);
    }

    // 효과음 볼륨 설정
    public void SetSFXVolume(float volume)
    {
        float dbVolume = (volume > 0.0001f) ? Mathf.Log10(volume) * 20 : -80f;
        audioMixer.SetFloat("SFXVolume", dbVolume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
        audioMixer.GetFloat("SFXVolume", out float newDbVolume);
        sfxVolumeSlider.value = Mathf.Pow(10, newDbVolume / 20);
    }

    public void SetMasterMute(bool isMuted)
    {
        if (!isMuted)
        {
            audioMixer.SetFloat("MasterVolume", -80f); // 음소거
        }
        else
        {
            float volume = masterVolumeSlider.value;
            SetMasterVolume(volume); // 슬라이더 값을 기준으로 볼륨 설정
        }

        PlayerPrefs.SetInt("MasterMute", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
}
