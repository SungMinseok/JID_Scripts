using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UB.Simple2dWeatherEffects.Standard;

public class LoadManager : MonoBehaviour
{
    public static LoadManager instance;
    public Image loadFader;
    public Slider slider;
    public InputField inputText;
    public D2FogsNoiseTexPE fogsNoiseTexPE;
    public GameObject[] mainBtns;

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
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void StartBtn()
    {
        // for(var i=0; i<mainBtns.Length;i++){
        //     mainBtns[i].SetActive(false);
        // }
        
        StartCoroutine(MainToGame());
    }

    IEnumerator LoadNextScene(string nextScene)
    {
        SceneManager.LoadScene("Loading");
        yield return null;

        AsyncOperation asyncScene = SceneManager.LoadSceneAsync(nextScene);
        asyncScene.allowSceneActivation = false;
        while (!asyncScene.isDone)
        {
            yield return null;
            if (asyncScene.progress >= 0.9f)
            {
                asyncScene.allowSceneActivation = true;
                LoadSceneFadeIn();
            }
        }
    }
    public IEnumerator MainToGame(){
        SceneController.instance.objects[0].GetComponent<Animator>().SetTrigger("go");
        yield return new WaitForSeconds(1f);
        loadFader.GetComponent<Animator>().SetTrigger("fadeOut");
        yield return new WaitForSeconds(1f);
        StartCoroutine(LoadNextScene("Level2"));
    }
    
    public void ResetFader(float value){
        var defaultColor = loadFader.color;
        loadFader.color = new Color(defaultColor.r,defaultColor.g,defaultColor.b,value);
    }
    public void LoadSceneFadeIn(){
        ResetFader(1);
        loadFader.GetComponent<Animator>().SetTrigger("fadeIn");
    }


    public IEnumerator FadeOut(){

        while (fogsNoiseTexPE.Density <= 4)
        {
            fogsNoiseTexPE.Density += 0.01f;
            yield return null;
        }
    }
    public IEnumerator FadeIn(){

        while (fogsNoiseTexPE.Density >= 0)
        {
            fogsNoiseTexPE.Density -= 0.04f;
            yield return null;
        }
    }

    public IEnumerator ReloadGame(){
        //SceneController.instance.objects[0].GetComponent<Animator>().SetTrigger("go");
        
        UIManager.instance.SetFadeOut();
        yield return new WaitForSeconds(1f);
        loadFader.GetComponent<Animator>().SetTrigger("fadeOut");
        yield return new WaitForSeconds(1f);
        StartCoroutine(LoadNextScene("Level2"));
    }
}