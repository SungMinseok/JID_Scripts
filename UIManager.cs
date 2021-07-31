using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    //Vector3 playerOriginPos;
    PlayerManager thePlayer;
    //public GameObject clearPanel;
    [Header("UI_States")]
    public Image dirtGauge;
    public TextMeshProUGUI honeyText;
    [Header("UI_Inventory")]
    [Header("UI_Select")]
    public GameObject ui_select;
    public Transform ui_select_grid;
    [Header("UI_Effects")]
    public Transform effects;
    public bool onEffect;

    //WaitForSeconds waitTime = new WaitForSeconds(0.5f);
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        thePlayer = PlayerManager.instance;
        //playerOriginPos = thePlayer.transform.position;
    }
    void Update(){

        if(dirtGauge.fillAmount != thePlayer.curDirtAmount / thePlayer.maxDirtAmount){
            dirtGauge.fillAmount = thePlayer.curDirtAmount / thePlayer.maxDirtAmount;
        }

        if(honeyText.text != thePlayer.curHoneyAmount.ToString()){
            honeyText.text = thePlayer.curHoneyAmount.ToString();
        }
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
