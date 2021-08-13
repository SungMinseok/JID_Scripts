using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UB.Simple2dWeatherEffects.Standard;

public class LoadManager : MonoBehaviour
{
    
    public Slider slider;
    public InputField inputText;
    public D2FogsNoiseTexPE fogsNoiseTexPE;
    public GameObject[] mainBtns;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        //StartCoroutine(Load());
        // #if !DEV_MODE
        //         inputText.gameObject.SetActive(false);
        //         StartCoroutine(Load());
        // #else
        //         inputText.gameObject.SetActive(true);
        //         inputText.onEndEdit.AddListener(delegate{SetSaveNum();});


        // #endif
    }
    // #if DEV_MODE
    //     void SetSaveNum(){
    //         if(inputText.text != ""){
    //             SettingManager.instance.saveNum = int.Parse(inputText.text);
    //             StartCoroutine(Load());
    //         }
    //     }
    // #endif

    public void StartBtn()
    {
        for(var i=0; i<mainBtns.Length;i++){
            mainBtns[i].SetActive(false);
        }


        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        AsyncOperation asyncScene = SceneManager.LoadSceneAsync("warehouse");
        asyncScene.allowSceneActivation = false;
        float timeC = 0;
        //yield return new WaitForSeconds(0.5f);
        while (!asyncScene.isDone)
        {
            yield return null;
            //Debug.Log(asyncScene.progress);
            timeC += Time.deltaTime;
            if (asyncScene.progress >= 0.9f)
            {
                // slider.value = Mathf.Lerp(slider.value, 1, timeC);
                // if (slider.value == 1.0f)
                // {
                //while()
                while (fogsNoiseTexPE.Density <= 4)
                {
                    fogsNoiseTexPE.Density += 0.01f;
                    yield return null;
                }

                asyncScene.allowSceneActivation = true;

                // while (fogsNoiseTexPE.Density >= 0)
                // {
                //     fogsNoiseTexPE.Density -= 0.04f;
                //     yield return null;
                // }
                //}         
            }
            // else
            // {
            //     slider.value= Mathf.Lerp(slider.value, asyncScene.progress, timeC);
            //     if (slider.value >= asyncScene.progress){
            //         timeC = 0f;
            //     }
            // }
        }
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
}