using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public static Audio adManager;
    public UniqueInfo uniq;

    [Header("#ALL")]
    public float allSlider;
    public float bgmSlider;
    public float sfxSlider;


    [Header("#BGM")]
    public AudioClip[] bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channles;
    AudioSource[] sfxPlayers;
    int channlesIndex;

    public enum Sfx { GameOver, BuyItem, Touch, Rack, Shield, Btn, Shop, Slow, UsedItem, Big, EX,HP, hit}//효과음 순서에 맞춰서 기입

    private void Awake()
    {
        adManager = this;
        Init();// 따로 오브젝트를 만들었 다면 불필요.
    }
    private void Start()
    {
        allSlider = uniq.allSlider;
        bgmSlider = uniq.bgmSlider;
        sfxSlider = uniq.sfxSlider;
        AllVolumeSetting();
    }

    public void AllVolumeSetting()
    {
        BgmVolumeSetting();
        SfxVolumeSetting();
    }
    public void BgmVolumeSetting()
    {
        float v = allSlider * bgmSlider;
        bgmVolume = v;
        bgmPlayer.volume = bgmVolume;
    }
    public void SfxVolumeSetting()
    {
        float v = allSlider * sfxSlider;
        sfxVolume = v;
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i].volume = v;
        }
        PlaySfx(Sfx.Btn);
    }

    void Init()
    {
        //배경은 플레이어 초기화
        GameObject bgmObj = new GameObject("BgmPlayer");
        bgmObj.transform.parent = transform;
        bgmPlayer = bgmObj.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = true;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip[0];

        //효과음 플레이어 초기화
        GameObject sfxObj = new GameObject("SfxPlayer");
        sfxObj.transform.parent = transform;
        sfxPlayers = new AudioSource[channles];
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObj.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = sfxVolume;
        }
    }
    public void PlayBgm(int i)
    {
        StartCoroutine(BgmPlaing(i));
    }
    IEnumerator BgmPlaing(int i)
    {
        yield return null;
        float v = bgmVolume;
        while(v > 0)
        {
            v -= Time.deltaTime*(bgmVolume / 2f);
            bgmPlayer.volume = v;
            yield return null;
        }
        yield return null;
        bgmPlayer.Stop();
        bgmPlayer.clip = bgmClip[i];
        bgmPlayer.Play();
        while (v < bgmVolume)
        {
            v += Time.deltaTime * (bgmVolume / 2f);
            bgmPlayer.volume = v;
            yield return null;
        }

    }
    public void PlaySfx(Sfx sfx)
    {
        for (int i = 0;i < sfxPlayers.Length;i++) 
        {
            int loopint = (i + channlesIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopint].isPlaying)
                continue;

            channlesIndex = loopint;
            sfxPlayers[channlesIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[channlesIndex].Play();
            break;
        }
    }
}
