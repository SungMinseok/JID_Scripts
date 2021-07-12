using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    Vector3 playerOriginPos;
    PlayerManager player;
    //public GameObject clearPanel;
    public Transform effects;
    public bool onEffect;
    //WaitForSeconds waitTime = new WaitForSeconds(0.5f);
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        player = PlayerManager.instance;
        playerOriginPos = player.transform.position;
    }

    public void ResetPos(){
        player.transform.position = playerOriginPos;
    }

    public void ActivateEffect(int num,float timer,bool bgOn = true){
        onEffect = true;
        StartCoroutine(ActivateEffectCoroutine(num, timer, bgOn));
    }
    IEnumerator ActivateEffectCoroutine(int num,float timer,bool bgOn){
        if(bgOn){
            effects.GetChild(0).gameObject.SetActive(true);
        }
        var canvasGroup = effects.GetComponent<CanvasGroup>();

        effects.GetChild(num).gameObject.SetActive(true);
        yield return new WaitForSeconds(timer);

        while (canvasGroup.alpha >= 0.11)
        {
            canvasGroup.alpha -= 0.1f;
            yield return null;
        }
        
        effects.GetChild(num).gameObject.SetActive(false);

        if(bgOn){
            effects.GetChild(0).gameObject.SetActive(false);
        }

        canvasGroup.alpha = 1;
        
        onEffect = false;
    }
}
