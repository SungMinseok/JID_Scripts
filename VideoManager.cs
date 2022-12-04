using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public static VideoManager instance;
    public VideoPlayer videoPlayer;
    public RenderTexture texture;
    public GameObject videoRenderer;
    public bool isPlayingVideo;
    public bool canSkip;
    //WaitForSeconds waitTime = new WaitForSeconds(0.1f);
    
    void Awake(){
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
    void Start(){
        //StartCoroutine(IntroCoroutine());
    }

    public void PlayVideo(VideoClip curVideo, bool isLooping = false, float delayTime = 0f, bool needClear = true, float volume = 1f, float playSpeed = 1f, bool isSkippable = false){
        if(isSkippable){
            canSkip = true;
        }
        //Debug.Log("1");
        if(needClear)
            ClearOutRenderTexture();
        StartCoroutine(PlayVideoCoroutine(curVideo, isLooping,delayTime,volume,playSpeed));

        isPlayingVideo = true;
    }

    IEnumerator PlayVideoCoroutine(VideoClip curVideo, bool isLooping, float delayTime, float volume,float playSpeed){
        videoPlayer.playbackSpeed = playSpeed;
        
        
        if(delayTime == 0){
            yield return null;
        }
        else{
            yield return new WaitForSeconds(delayTime);

        }
//        Debug.Log("A");

        if(isLooping){
            videoPlayer.isLooping = true;
        }
        else{
            videoPlayer.isLooping = false;
        }
        videoPlayer.clip = curVideo;

        videoPlayer.SetDirectAudioVolume(0, volume * DBManager.instance.localData.bgmVolume);

        videoPlayer.Prepare();
        WaitForSeconds waitTime = new WaitForSeconds(0.1f);
        while(!videoPlayer.isPrepared){
            yield return waitTime;
        }
        videoPlayer.Play();
        videoRenderer.gameObject.SetActive(true);
        //LoadManager.instance.loadFader.gameObject.SetActive(false);

//        Debug.Log("B");
        // while(videoPlayer.isPlaying){
        //     yield return waitTime;
        // }
        yield return new WaitUntil(()=>!videoPlayer.isPlaying);
        
        
        VideoIsEnd();
    }
    public void StopVideo(){
        Debug.Log("stopVideo");
        videoPlayer.Stop();
        videoRenderer.gameObject.SetActive(false);
    }


    public void ClearOutRenderTexture()
    {
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = texture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = rt;
    }
    public string GetPlayingVideoName(){
//        Debug.Log(videoPlayer.clip.ToString());
        return videoPlayer.clip.ToString();
    }
    public void SkipPlayingVideo(){
        Debug.Log("skip");
        VideoIsEnd();//VideoManager.instance.isPlayingVideo = false;
    }
    // IEnumerator IntroCoroutine(){

    // }
    public void VideoIsEnd(){
        Debug.Log("VideoIsEnd");
        isPlayingVideo = false;

    }
    void Update(){
        if(canSkip && Input.GetKeyDown(KeyCode.Escape)){
            canSkip = false;
            VideoIsEnd();
        }
    }
}