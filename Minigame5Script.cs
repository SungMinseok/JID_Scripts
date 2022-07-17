using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class Minigame5Script : MonoBehaviour
{
    public static Minigame5Script instance;

    //x : 7.5  y : 4
    [Header("[Game Settings]─────────────────")]
    [Tooltip("0~1")]
    public float probability;
    public int firstGetAmount = 100;
    public float multipleAmount = 2;
    public Select goStopSelect;


    
    [Space]
    [Header("[Game Objects]─────────────────")]
    public Image[] rouletteImages;
    public Sprite[] rouletteSprites;
    public Button startBtn;
    public Button stopBtn;
    public TextMeshProUGUI currentHoneyText;
    public TextMeshProUGUI nextHoneyText;
    public Animator resultEffectAnimator;
    public GameObject[] fruitBursts;

    [Space]

    [Header("[Debug]─────────────────")]
    public bool stop;
    public int curStage;




    WaitForSeconds wait1000ms = new WaitForSeconds(1f);
    WaitForSeconds wait500ms = new WaitForSeconds(0.5f);
    WaitForSeconds wait250ms = new WaitForSeconds(0.25f);

    WaitUntil waitSelecting = new WaitUntil(()=>!PlayerManager.instance.isSelecting);
    WaitUntil waitStop = new WaitUntil(()=>Minigame5Script.instance.stop);


    void Awake(){
        instance = this;
    }
    void Start(){
        //waitAphidCreationCycle = new WaitForSeconds(aphidCreationCycle);
    }
    void ResetGameSettings(){
        curStage = 0;
        UpdateCurHoneyAmount();
        nextHoneyText.text = "0";
        //nextHoneyText.text = firstGetAmount.ToString();
        stopBtn.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(true);
        startBtn.interactable = true;
        for(int i=0;i<3;i++){
            fruitBursts[i].SetActive(false);
        }
    }
    public void UpdateCurHoneyAmount(){
        currentHoneyText.text = DBManager.instance.curData.curHoneyAmount.ToString();
    }
    public void StartRoulette(){
        if(curStage == 0){
           
            if(DBManager.instance.curData.curHoneyAmount>=firstGetAmount){
                DBManager.instance.curData.curHoneyAmount -= firstGetAmount;
                nextHoneyText.text = firstGetAmount.ToString();
                UpdateCurHoneyAmount();

                startBtn.gameObject.SetActive(false);
                stopBtn.gameObject.SetActive(true);
                    
                StopAllCoroutines();
                StartCoroutine(RouletteCoroutine());
            }
            else{
                MenuManager.instance.OpenPopUpPanel_OneAnswer("10");
            }
 
        }
        else{
        
            startBtn.gameObject.SetActive(false);
            stopBtn.gameObject.SetActive(true);
                
            StopAllCoroutines();
            StartCoroutine(RouletteCoroutine());
        }



    }
    public void PushStopBtn(){
        stopBtn.gameObject.SetActive(false);
        stop = true;
        startBtn.gameObject.SetActive(true);
    }
    IEnumerator RouletteCoroutine(){
        startBtn.interactable = false;

        for(int i=0;i<3;i++){
            rouletteImages[i].GetComponent<Animator>().enabled = true;
            rouletteImages[i].GetComponent<Animator>().SetBool("rotate",true);
        }
        yield return waitStop;
        stop = false;

        int resultValue = Random.Range(0,10000);
        bool isSuccessed = resultValue <= 10000*probability ? true : false;


        int[] order = new int[3]{0,0,0};
        if(isSuccessed){
            int temp = Random.Range(0,3);
            order = new int[3]{temp,temp,temp};
            //Debug.Log(resultValue*0.01f+" , " + isSuccessed+" : " + order[0] +","+ order[1] +","+ order[2]);
        }
        else{
            while((order[0]==order[1]&&order[1]==order[2])){
                order = new int[3]{Random.Range(0,3),Random.Range(0,3),Random.Range(0,3)};
                //Debug.Log("333");
            }
            //Debug.Log(resultValue*0.01f+" , " + isSuccessed+" : " + order[0] +","+ order[1] +","+ order[2]);
        }


        yield return wait1000ms;

        for(int i=0;i<3;i++){
            rouletteImages[i].GetComponent<Animator>().SetBool("rotate",false);
            rouletteImages[i].GetComponent<Animator>().enabled = false;
            rouletteImages[i].sprite = rouletteSprites[order[i]];
            //Debug.Log(order[i]);
            yield return wait500ms;
        }

        if(isSuccessed){
            resultEffectAnimator.SetBool("success",true);
            curStage ++;
            for(int i=0;i<3;i++){
                fruitBursts[i].SetActive(true);
            }
        }
        else{
            resultEffectAnimator.SetBool("fail",true);
            curStage = 0;
        }
        nextHoneyText.text = (firstGetAmount*Mathf.Pow(multipleAmount,curStage)).ToString();
        
        yield return wait1000ms;
        string[] tempStrings = new string[]{(int.Parse(nextHoneyText.text)*2).ToString(),nextHoneyText.text};
        SelectManager.instance.SetSelect(goStopSelect, tempStrings);
        yield return waitSelecting;
        resultEffectAnimator.SetBool("success",false);
        resultEffectAnimator.SetBool("fail",false);
        
        for(int i=0;i<3;i++){
            fruitBursts[i].SetActive(false);
        }
        if(SelectManager.instance.GetSelect()==0){
            //curStage ++ ;
            //nextHoneyText.text = (firstGetAmount*curStage*multipleAmount).ToString();
            StartRoulette();
        }
        else if(SelectManager.instance.GetSelect()==1){
            DBManager.instance.curData.curHoneyAmount += int.Parse(nextHoneyText.text);
           
            ResetGameSettings();
        }

    }
    private void OnEnable() {
        PlayerManager.instance.LockPlayer();
        ResetGameSettings();


        //checkedMushroom = false;
        //minigameCoroutine = StartCoroutine(MinigameCoroutine());

        //SceneController.instance.SetSomeConfiner(mapCollider,true);
        //SceneController.instance.virtualCamera.Follow = mapViewPoint;

        UIManager.instance.SetHUD(false);
    }
    void OnDisable(){
        PlayerManager.instance.UnlockPlayer();
        
        UIManager.instance.SetHUD(true);
        StopAllCoroutines();
    }
}
