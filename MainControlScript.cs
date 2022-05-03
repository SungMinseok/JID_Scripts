using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MainControlScript : MonoBehaviour
{
    
    public VideoClip[] videoClips;
    //public RenderTexture texture;
    public GameObject mainBtns;
    public GameObject demoImage;
    public bool canSkip;
    public string titleVideoName;
    WaitForSeconds wait100ms = new WaitForSeconds(0.1f);
    WaitForSeconds wait1000ms = new WaitForSeconds(1f);

    void Start(){
        StartCoroutine(IntroCoroutine());

        titleVideoName = videoClips[0].ToString();
    }

    IEnumerator IntroCoroutine(){
        
#if demo
        demoImage.SetActive(true);
#endif

            //yield return wait1000ms;

        // while(DBManager.instance==null){
        //     yield return wait100ms;
        // }

        MenuManager.instance.SetResolutionByValue(DBManager.instance.localData.resolutionValue);
        MenuManager.instance.SetFrameRateByValue(DBManager.instance.localData.frameRateValue);

        if(!LoadManager.instance.checkFirstRun){
            LoadManager.instance.checkFirstRun = true;
            VideoManager.instance.PlayVideo(videoClips[0], volume : 0.5f);
            yield return new WaitUntil(()=>!VideoManager.instance.isPlayingVideo);
        }


        mainBtns.SetActive(true);
        //collectionBtn.SetActive(true);

        VideoManager.instance.PlayVideo(videoClips[1], true, needClear: false);

    }


    public void PushStartBtn(){
        LoadManager.instance.MainToGame();
    }
    public void PushGameExitBtn(){
        Application.Quit();
    }
    void Update(){
        //if(VideoManager.instance.GetPlayingVideoName()==titleVideoName){

            if(Input.anyKeyDown && VideoManager.instance != null && VideoManager.instance.GetPlayingVideoName()==titleVideoName){
                VideoManager.instance.SkipPlayingVideo();
            }
        //}
    }
}
