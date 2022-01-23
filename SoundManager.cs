using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
 
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [Header("[Game Settings] ─────────────────────")]
    public string defaultBtnSoundName;

    public float masterVolumeSFX = 1f;
    public float masterVolumeBGM = 1f;
    [SerializeField] AudioClip[] BGMClips; // 오디오 소스들 지정.
    [SerializeField] AudioClip[] audioClips; // 오디오 소스들 지정.
    Dictionary<string, AudioClip> audioClipsDic;
    AudioSource sfxPlayer;
    AudioSource bgmPlayer;
    [SerializeField] AudioClip[] testClips; // 오디오 소스들 지정.

    void Awake() {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
    void Start()
    {
        sfxPlayer = GetComponent<AudioSource>();
        //SetupBGM();

        LoadResources();

        PutSoundsToDictionary();


    }
    void LoadResources(){
        audioClips = Resources.LoadAll<AudioClip>("Sounds");
    }
    void PutSoundsToDictionary(){

        audioClipsDic = new Dictionary<string, AudioClip>();
        foreach (AudioClip a in audioClips)
        {
            if (audioClipsDic.ContainsKey(a.name) == false){

                audioClipsDic.Add(a.name, a);
                //print(a.name);
            }
        }
    }

    // void SetupBGM()
    // {
    //     if (BGMClip == null) return;
        
    //     GameObject child = new GameObject("BGM");
    //     child.transform.SetParent(transform);
    //     bgmPlayer = child.AddComponent<AudioSource>();
    //     bgmPlayer.clip = BGMClip;
    //     bgmPlayer.volume = masterVolumeBGM;
    // }

    // 한 번 재생 : 볼륨 매개변수로 지정
    public void PlaySound(string soundFildName, float a_volume = 1f)
    {
        if (audioClipsDic.ContainsKey(soundFildName) == false)
        {
            Debug.Log(soundFildName + " is not Contained audioClipsDic");
            return;
        }
        sfxPlayer.PlayOneShot(audioClipsDic[soundFildName], a_volume * masterVolumeSFX);
        
    }

    #region 옵션에서 볼륨조절
    public void SetVolumeSFX(float a_volume)
    {
        masterVolumeSFX = a_volume;
    }

    public void SetVolumeBGM(float a_volume)
    {
        masterVolumeBGM = a_volume;
        bgmPlayer.volume = masterVolumeBGM;
    }
    #endregion

}