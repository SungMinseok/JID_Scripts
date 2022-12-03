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
    public string defaultGetItemSoundName = "item_get";
    public string defaultDoorSoundName = "opendoor";
    public string defaultAddDirtName = "dirt_charge";
    public float changeBgmDelta = 0.005f;

    //public float masterVolumeSFX = 1f;
    public float masterVolumeBGM = 1f;
    [SerializeField] AudioClip[] bgmClips; // 오디오 소스들 지정.
    [SerializeField] AudioClip[] audioClips; // 오디오 소스들 지정.
    Dictionary<string, AudioClip> audioClipsDic;
    Dictionary<string, AudioClip> bgmClipsDic;
    public AudioSource sfxPlayer;
    public AudioSource bgmPlayer;
    public AudioSource[] audioSources;
    WaitForSeconds wait10ms = new WaitForSeconds(0.01f);
    //[SerializeField] AudioClip[] testClips; // 오디오 소스들 지정.
    public Coroutine curChangeBgmCoroutine;
    public Coroutine curFadeOutBgmCoroutine;
    public Coroutine curFadeInBgmCoroutine;

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
        LoadResources();

        PutSoundsToDictionary();

        GetAudioSources();
    }
    void Start(){
        //audioSources = FindObjectsOfType<AudioSource>();

#if UNITY_EDITOR
        DBManager.instance.bgmName0 = "suomi";
#endif
    }
    public void GetAudioSources(){
        Debug.Log("Getaudiosource");

        if(audioSources!=null) Array.Clear(audioSources,0,audioSources.Length);

        audioSources = FindObjectsOfType<AudioSource>();

    }
    void OnDisable(){
//        Debug.Log("bgmVolume : "+bgmPlayer.volume);
        StopAllCoroutines();
    }
    void Update()
    {
        //sfxPlayer = GetComponent<AudioSource>();
        //SetupBGM();

        //PlayBGM("ant mill");
        //SetBgmByMapNum(0);

//        Debug.Log("bgmVolume : "+bgmPlayer.volume);

    }
    //public Sprite[] itemSprites;
    void LoadResources(){
        audioClips = Resources.LoadAll<AudioClip>("Sounds/SFX");
        bgmClips = Resources.LoadAll<AudioClip>("Sounds/BGM");
        //bgmClips = Resources.LoadAll<AudioClip>("BGM");
        //itemSprites = Resources.LoadAll<Sprite>("Sprites/Items");
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

        
        bgmClipsDic = new Dictionary<string, AudioClip>();
        foreach (AudioClip a in bgmClips)
        {
            if (bgmClipsDic.ContainsKey(a.name) == false){

                bgmClipsDic.Add(a.name, a);
                
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
    public void PlaySound(string soundFileName, float a_volume = 1f)
    {
        if (audioClipsDic.ContainsKey(soundFileName) == false)
        {
            Debug.LogError(soundFileName + " is not Contained audioClipsDic");
            return;
        }
        sfxPlayer.PlayOneShot(audioClipsDic[soundFileName], a_volume * sfxPlayer.volume);
        //Debug.Log(soundFileName);
    }
    
    public void PlayLoopSound(string soundFileName)
    {
        if (audioClipsDic.ContainsKey(soundFileName) == false)
        {
            Debug.LogError(soundFileName + " is not Contained audioClipsDic");
            return;
        }
        sfxPlayer.clip = audioClipsDic[soundFileName];
        sfxPlayer.loop = true;
        sfxPlayer.Play();
        //sfxPlayer.PlayOneShot(audioClipsDic[soundFileName], a_volume * sfxPlayer.volume);
        
    }
    public void StopLoopSound(){
        sfxPlayer.Stop();
    }
    public void PlayBGM(string soundFileName)
    {
        Debug.Log("PlayBGM : "+soundFileName);
        if (bgmClipsDic.ContainsKey(soundFileName) == false)
        {
            Debug.LogError(soundFileName + " is not Contained audioClipsDic");
            return;
        }
        bgmPlayer.clip = bgmClipsDic[soundFileName];
        bgmPlayer.loop = true;
        bgmPlayer.Play();
        
    }
    public void SetBgmByMapNum(int mapNum){
        Debug.Log($"SetBgmByMapNum : {mapNum}");
        string soundFileName = "";
        switch(mapNum){
            case 6:
            case 11:
            case 13:
            case 14:
            case 18:
            case 19:
            case 21:
                soundFileName = DBManager.instance.bgmName1;//"ant mill";
                break;
            case 22:
            case 23:
            case 24:
                soundFileName = DBManager.instance.bgmName2;//"royalroad";
                break;
            case 25:
                soundFileName = DBManager.instance.bgmName3;//"JellyVillage";
                break;
            default : 
                soundFileName = DBManager.instance.bgmName0;//"juicy drug";
                break;
        }
        //Debug.Log("SetBgmByMapNum : "+ soundFileName);
        
        ChangeBgm(soundFileName);
    }
    public void ChangeBgm(string soundFileName){
        if(!bgmClipsDic.ContainsKey(soundFileName)){
            Debug.Log("bgm 파일 없음 : "+soundFileName);
            return;
        }

        if(bgmPlayer.clip != bgmClipsDic[soundFileName]){
            //Debug.Log("브금변경");
            if(curChangeBgmCoroutine!=null) StopCoroutine(curChangeBgmCoroutine);
            curChangeBgmCoroutine = StartCoroutine(ChangeBgmCoroutine(soundFileName));
        }
        else{

            //Debug.Log("브금유지");
        }
    }
    public void BgmOff(){
    
        if(curChangeBgmCoroutine!=null) StopCoroutine(curChangeBgmCoroutine);
        curChangeBgmCoroutine = StartCoroutine(ChangeBgmCoroutine("off"));
    }
    IEnumerator ChangeBgmCoroutine(string soundFileName){

        float curBgmVolume = DBManager.instance.localData.bgmVolume;
        float tempDelta = changeBgmDelta / curBgmVolume;
        
        if(bgmPlayer.clip != null){

            while(bgmPlayer.volume > 0){
                bgmPlayer.volume -= tempDelta;
                yield return wait10ms;
            }
            if(soundFileName != "off"){
                bgmPlayer.clip = bgmClipsDic[soundFileName];
                bgmPlayer.Play();
            }
            //소리 감소 하면서 아예 정지
            else{
                bgmPlayer.clip = null;
                bgmPlayer.Stop();
            }
            while(bgmPlayer.volume < curBgmVolume){
                bgmPlayer.volume += tempDelta;
                yield return wait10ms;
            }
            bgmPlayer.volume = curBgmVolume;
        }
        //이전에 재생중인게 없으면 바로 시작
        else{
            if(soundFileName != "off"){

                bgmPlayer.clip = bgmClipsDic[soundFileName];
                bgmPlayer.Play();
            }
        }

    }
    public void SoundOff(){
        sfxPlayer.Stop();
    }
    public float GetSoundLength(string soundFileName){
        
        if (audioClipsDic.ContainsKey(soundFileName) == false)
        {
            Debug.LogError(soundFileName + " is not Contained audioClipsDic");
            return -1;
        }
        return audioClipsDic[soundFileName].length;
    }
    // public void FadeOutBgm(){
    //     if(curFadeInBgmCoroutine!=null) StopCoroutine(curFadeInBgmCoroutine);
    //     StartCoroutine(FadeOutBgmCoroutine());
    // }
    // IEnumerator FadeOutBgmCoroutine(){

    // }

    #region 옵션에서 볼륨조절
    public void SetVolumeSFX(float value)
    {
        value *=  DBManager.instance.sfxAdjustVal;
        DBManager.instance.localData.sfxVolume = value;
        foreach(AudioSource a in audioSources){
            if(a == bgmPlayer) continue;
            a.volume = value;
        }
        if(SceneController.instance.audioSources!=null){
            foreach(AudioSource b in SceneController.instance.audioSources){
                b.volume = value;
            }
        }
        //sfxPlayer.volume = value;// * 0.5f;
    }

    public void SetVolumeBGM(float value)
    {
        value *=  DBManager.instance.bgmAdjustVal;
        DBManager.instance.localData.bgmVolume = value;
        bgmPlayer.volume = value;// * 0.5f;
    }
    // public void SetVolumeBGM(float value)
    // {
    //     bgmPlayer.volume = value;
    // }
    #endregion

}