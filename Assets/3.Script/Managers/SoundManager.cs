using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
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
        // ����� �ҷ�����
        bgmClip = Resources.Load<AudioClip>("BGM/" + SceneManager.GetActiveScene().name);
        Init();
        PlayBgm();
    }

    void Init()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent =  transform;
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
        // ���� ������̴� �� ���߰� ���
        if (bgmPlayer.isPlaying)
        {
            bgmPlayer.Stop();
        }

        bgmPlayer.Play();
    }

    public void PlaySfx<T>(T sfx) where T : Enum
    {
        // �ѹ��� ���
        sfxPlayer.PlayOneShot(sfxClips[Convert.ToInt32(sfx)]);
    }
}
