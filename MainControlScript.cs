using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class MainControlScript : MonoBehaviour
{
    
    public VideoClip[] videoClips;
    public VideoClip splashClip;
    //public RenderTexture texture;
    public GameObject mainBtns;
    public GameObject demoImage;
    public GameObject creditBtn;
    public bool splashFlag;
    public bool canSkip;
    public string titleVideoName;
    public string splashVideoName;
    WaitForSeconds wait100ms = new WaitForSeconds(0.1f);
    WaitForSeconds wait500ms = new WaitForSeconds(0.5f);
    WaitForSeconds wait1000ms = new WaitForSeconds(1f);
    WaitForSeconds waitCreditVideo;

    void Start(){


        StartCoroutine(IntroCoroutine());

        titleVideoName = videoClips[0].ToString();
        splashVideoName = splashClip.ToString();

        mainBtns.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(()=>MenuManager.instance.OpenPanel("load"));
        mainBtns.transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(()=>MenuManager.instance.OpenPanel("collection"));
        mainBtns.transform.GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(()=>MenuManager.instance.OpenPanel("setting"));
        mainBtns.transform.GetChild(0).GetChild(4).GetComponent<Button>().onClick.AddListener(()=>MenuManager.instance.OpenPanel("language"));
        //mainBtns.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(()=>MenuManager.instance.OpenPanel("collection"));
        
        waitCreditVideo = new WaitForSeconds((float)videoClips[2].length);

        if(DBManager.instance.GetClearedEndingCollectionID(7)==-1){
            creditBtn.SetActive(false);
        }
    }

    IEnumerator IntroCoroutine(){
        
#if demo
        demoImage.SetActive(true);
#endif
        //LoadManager.instance.LoadScene("Menu");

        //yield return wait1000ms;
            //yield return wait1000ms;

        // while(DBManager.instance==null){
        //     yield return wait100ms;
        // }
        yield return new WaitUntil(()=>MenuManager.instance);
        // Debug.Log

        MenuManager.instance.SetResolutionByValue(DBManager.instance.localData.resolutionValue);
        MenuManager.instance.SetFrameRateByValue(DBManager.instance.localData.frameRateValue);

        if(!LoadManager.instance.checkFirstRun){
            LoadManager.instance.checkFirstRun = true;
            splashFlag = true;
        //LoadManager.instance.loadFader.gameObject.SetActive(true);
            StartCoroutine(SplashCoroutine());
            yield return new WaitUntil(()=>!splashFlag);
            //LoadManager.instance.ResetFader(0f);

            // VideoManager.instance.PlayVideo(splashClip, volume : 0.5f);
            // yield return new WaitUntil(()=>!VideoManager.instance.isPlayingVideo);
SoundManager.instance.PlayBGM("jelly in the dark");

            VideoManager.instance.PlayVideo(videoClips[0], volume : 0.5f);
            yield return new WaitUntil(()=>!VideoManager.instance.isPlayingVideo);
        }

SoundManager.instance.ChangeBgm("jelly in the dark");
        mainBtns.SetActive(true);
        //collectionBtn.SetActive(true);

        VideoManager.instance.PlayVideo(videoClips[1], true, needClear: false);
        yield return new WaitForSeconds(0.1f);
        VideoManager.instance.isPlayingVideo = true;
        //Debug.Log(VideoManager.instance.isPlayingVideo);
    }
    IEnumerator SplashCoroutine(){
        LoadManager.instance.ResetFader(1f);
        LoadManager.instance.loadFader.gameObject.SetActive(true);
        LoadManager.instance.FadeIn();
        VideoManager.instance.PlayVideo(splashClip, volume : 0.5f);
        yield return new WaitUntil(()=>!VideoManager.instance.isPlayingVideo);
        LoadManager.instance.FadeOut();
        yield return new WaitForSeconds(0.9f);
        VideoManager.instance.ClearOutRenderTexture();
        LoadManager.instance.loadFader.gameObject.SetActive(false);
        splashFlag = false;

    }

    public void PushStartBtn(){
        LoadManager.instance.MainToGame();
    }
    public void PushGameExitBtn(){
        Application.Quit();
    }
    void Update(){
        //if(VideoManager.instance.GetPlayingVideoName()==titleVideoName){

            if(Input.anyKeyDown && VideoManager.instance != null 
            && (VideoManager.instance.GetPlayingVideoName()==titleVideoName
            /* || VideoManager.instance.GetPlayingVideoName()==splashVideoName */)
            && VideoManager.instance.isPlayingVideo
            ){
                Debug.Log("35355");
                VideoManager.instance.isPlayingVideo = false;
                //VideoManager.instance.SkipPlayingVideo();
            }
        //}
    }
    public void PlayCreditVideo(){
        StartCoroutine(PlayCreditVideoCoroutine());

    }
    IEnumerator PlayCreditVideoCoroutine(){
        
        LoadManager.instance.FadeOut();
        yield return wait1000ms;
        VideoManager.instance.StopVideo();
        yield return wait500ms;
        SoundManager.instance.BgmOff();
        mainBtns.gameObject.SetActive(false);
        LoadManager.instance.FadeIn();
        VideoManager.instance.PlayVideo(videoClips[2], volume : 0.5f, isSkippable:true);
        VideoManager.instance.isPlayingVideo = true;
        //yield return waitCreditVideo;
        yield return new WaitUntil(()=>!VideoManager.instance.isPlayingVideo);
        LoadManager.instance.FadeOut();
        yield return wait1000ms;
        VideoManager.instance.StopVideo();
        yield return wait500ms;
        VideoManager.instance.PlayVideo(videoClips[1], volume : 0.5f);
        SoundManager.instance.ChangeBgm("jelly in the dark");
        mainBtns.gameObject.SetActive(true);
        LoadManager.instance.FadeIn();
    }
}
