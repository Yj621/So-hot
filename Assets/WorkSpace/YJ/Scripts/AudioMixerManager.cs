using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    // 사운드 볼륨슬라이더
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    public static AudioMixerManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 오디오 슬라이더 저장 값 불러오기, 없으면 기본 값 1
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1);
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume",1);

        // 오디오믹서의 현재 볼륨을 슬라이더 값으로 변환
        if (audioMixer.GetFloat("Master", out float masterDb))
            masterVolumeSlider.value = Mathf.Pow(10, masterDb / 20);
        if (audioMixer.GetFloat("BGM", out float bgmDb))
            bgmVolumeSlider.value = Mathf.Pow(10, bgmDb / 20);
        if (audioMixer.GetFloat("SFX", out float sfxDb))
            sfxVolumeSlider.value = Mathf.Pow(10, sfxDb / 20);

        // 슬라이더 값 변경 리스너 등록
        masterVolumeSlider.onValueChanged.AddListener((value) =>
        {
            SetMasterVolume(value);
            PlayerPrefs.SetFloat("MasterVolume", value); // 값 저장
            PlayerPrefs.Save();
        });

        bgmVolumeSlider.onValueChanged.AddListener((value) =>
        {
            SetBGMVolume(value);
            PlayerPrefs.SetFloat("BGMVolume", value); // 값 저장
            PlayerPrefs.Save();
        });

        sfxVolumeSlider.onValueChanged.AddListener((value) =>
        {
            SetSFXVolume(value);
            PlayerPrefs.SetFloat("SFXVolume", value); // 값 저장
            PlayerPrefs.Save();
        });

        // 초기 오디오 볼륨 설정
        SetMasterVolume(masterVolumeSlider.value);
        SetBGMVolume(bgmVolumeSlider.value);
        SetSFXVolume(sfxVolumeSlider.value);

    }

    //AudioMixer는 볼륨값이 -80~0이다.
    /*value는 슬라이더의 값으로 최솟값 0.0001 최댓값 1로 설정한다
    그럼 Log10을하고 *20을 하면(-80,0) 사이의 값이 도출된다.*/

    public void SetMasterVolume(float volume)
    {
        Debug.Log(volume);
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }

    public void SetBGMVolume(float volume)
    {
        Debug.Log(volume);
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }
}
