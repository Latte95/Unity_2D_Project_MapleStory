using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public static SoundManager Instance => instance;

    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    AudioSource sfxPlayer;

    private void Awake()
    {
        Init();
        instance.PlayBgm();
    }

    public void Init()
    {
        if (instance == null)
        {
            // 게임 매니저를 찾아서 없으면 오브젝트 생성하고 스크립트 붙이기
            GameObject sm = GameObject.Find("SoundManager");
            if (sm == null)
            {
                sm = new GameObject { name = "SoundManager" };
                sm.AddComponent<SoundManager>();
            }

            //DontDestroyOnLoad(sm);
            sm.TryGetComponent(out instance);
        }

        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;

        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayer = sfxObject.AddComponent<AudioSource>();
        sfxPlayer.playOnAwake = false;
        sfxPlayer.volume = sfxVolume;
    }

    public void PlayBgm()
    {
        if (bgmPlayer.isPlaying)
        {
            bgmPlayer.Stop();
        }

        bgmPlayer.Play();        
    }

    public void PlaySfx<T>(T sfx) where T : Enum
    {
        sfxPlayer.PlayOneShot(sfxClips[Convert.ToInt32(sfx)]);
    }

    public void Clear()
    {
        bgmPlayer.Stop();
        sfxPlayer.Stop();
    }
}
