using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarBGM : MonoBehaviour
{
    private static StarBGM instance;
    private AudioSource backmusic;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            backmusic = GetComponent<AudioSource>();
            backmusic.Play();
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 2)
        {
            backmusic.enabled = false;
        }
        else if (scene.buildIndex == 0 || scene.buildIndex == 1)
        {
            if (!backmusic.isPlaying)
            {
                backmusic.enabled = true;
                backmusic.Play();
            }
        }
    }
}
