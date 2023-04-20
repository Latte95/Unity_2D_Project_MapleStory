using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            Init();
            return instance;
        }
    }

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

    static void Init()
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
        bgmObject.transform.parent = instance.transform;
        instance.bgmPlayer = bgmObject.AddComponent<AudioSource>();
        instance.bgmPlayer.playOnAwake = false;
        instance.bgmPlayer.loop = true;
        instance.bgmPlayer.volume = instance.bgmVolume;
        instance.bgmPlayer.clip = instance.bgmClip;

        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = instance.transform;
        instance.sfxPlayer = sfxObject.AddComponent<AudioSource>();
        instance.sfxPlayer.playOnAwake = false;
        instance.sfxPlayer.volume = instance.sfxVolume;
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
