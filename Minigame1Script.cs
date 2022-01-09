using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
//Level, Stage  : Level이 상위 개념
//Level : 총 3레벨
//Stage : 레벨 당 3스테이지
public class Minigame1Script : MonoBehaviour
{
    [Header("단계별 허용 간격(0.3/0.2/0.1)")]
    [Header("Game Settings──────────")]
    public float[] acceptableIntervals;
    [Header("단계별 속도(0.04/0.045/0.05)")]
    public float[] pointerSpeedPerStage;
    
    [Header("Objects──────────")]

    public RectTransform errorArea;
    public Animator mainBottle;
    public GameObject[] emptyBottles;
    public GameObject[] fullBottles;
    public Animator failBubbleAnimator;
    //public float errorAreaDefaultWidth;
    
    // [Header("1 번째 간격")]
    // public float acceptableInterval1;
    // [Header("2 번째 간격")]
    // public float acceptableInterval2;
    // [Header("정답 맞췄을 때 추가 시간")]
    // public float timerBonus;
    // [Header("정답 틀렸을 때 감소 시간")]
    // public float timerPanelty;


    // public Image timerGauge;
    // public Image mainImage;

    
    // public Sprite[] keySprites,mainSprites;
    // public Transform keyArray;



    public Transform bubbleObjects;
    public Slider sliderPointer;

    [Space]

    [Header("Debug──────────")]
    public bool isSliderUp;
    Coroutine sliderMovementCoroutine;
    Coroutine minigameCoroutine;
    public bool canSelect;
    public float randomCenterPosVar;
    public int curStage;
    public int curLevel;

    WaitForSeconds wait1000ms = new WaitForSeconds(1f);
    WaitForSeconds wait500ms = new WaitForSeconds(0.5f);


    // public int curGetKey;
    // public float curTimer;  
    // public int curLevel;//0~4  
    // public int curStage;//0~9
    // public List<int> curLevelAnswer;
    // public bool curLevelFlag;
    private void OnEnable() {
        //canSelect = true;
        FirstSetGame();
        //StartCoroutine(Wait());
        minigameCoroutine = StartCoroutine(MinigameCoroutine());
        //StartSliderMoving();
    }
    IEnumerator MinigameCoroutine(){
        //bubbleObjects.GetChild(0).gameObject.SetActive(true);
        SetLevel();
        yield return wait1000ms;
        curStage = 0;
        StartCoroutine(SetStage());
        // canSelect = true;
        // sliderPointer.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        // SetErrorArea(0);
    }
    void SetLevel(){
        for(int i=0;i<4;i++){
            bubbleObjects.GetChild(i).gameObject.SetActive(false);
        }

        bubbleObjects.GetChild(0).gameObject.SetActive(true);
        //sliderPointer.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }
    IEnumerator SetStage(){
        canSelect = true;
        sliderPointer.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        yield return wait1000ms;
        SetErrorArea();
    }
    void SetErrorArea(){
        errorArea.gameObject.SetActive(true);
        errorArea.localScale = new Vector2(acceptableIntervals[curStage],1);
        var temp = UnityEngine.Random.Range(0f+acceptableIntervals[curStage]/2f,1f-acceptableIntervals[curStage]/2f);
        randomCenterPosVar = (float)(Math.Truncate(temp*100)/100);

        //Debug.Log("randomCenterPos : " + randomCenterPosVar);
        //Debug.Log("randomCenterPos 실제 : " + temp*556f);
        errorArea.anchoredPosition = new Vector2(randomCenterPosVar*556f,0);
        errorArea.gameObject.SetActive(true);
        
        Debug.Log("성공 값 : " + (randomCenterPosVar - acceptableIntervals[curStage]/2f) +", " + (randomCenterPosVar + acceptableIntervals[curStage]/2f));
    }
    IEnumerator Wait(){

        canSelect = false;
        yield return wait1000ms;
        sliderPointer.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(2.5f);
    }  

    void FirstSetGame(){
        canSelect = false;
        errorArea.gameObject.SetActive(false);
        sliderPointer.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        for(int i=0; i<3; i++){
            emptyBottles[i].SetActive(true);
            fullBottles[i].SetActive(false);
        }
    }

    void Update()
    {
        if(canSelect){
            if(Input.GetButtonDown("Interact_OnlyKey")){
                StartCoroutine(CheckPointerPostion());
            }
        }
    }
    private void FixedUpdate() {
        if(canSelect){
            //sliderPointer.value
            if(isSliderUp){                
                if(sliderPointer.value<0.99f && sliderPointer.value >0.01f){
                    sliderPointer.value += pointerSpeedPerStage[curStage];
                //Debug.Log("상승중 : "+ sliderPointer.value);
                    //yield return null;
                }
                else{
                    isSliderUp = false;
                    sliderPointer.value -= pointerSpeedPerStage[curStage];
                }
            }
            else{
                if(sliderPointer.value<0.99f && sliderPointer.value >0.01f){
                    sliderPointer.value -= pointerSpeedPerStage[curStage];
                //Debug.Log("하강중 : "+ sliderPointer.value);
                    //yield return null;
                }
                else{
                    isSliderUp = true;
                    sliderPointer.value += pointerSpeedPerStage[curStage];
                }
            }
        }
    }
    IEnumerator CheckPointerPostion(){
        // switch(curLevel){
        //     case 0 : 
        //         break;
        
       // }
        mainBottle.SetTrigger("tilt");
        canSelect = false;
        //sliderPointer.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(1.8f);
        float minErrorValue = randomCenterPosVar - acceptableIntervals[curStage]/2f;
        float maxErrorValue = randomCenterPosVar + acceptableIntervals[curStage]/2f;

        
        errorArea.gameObject.SetActive(false);
        //StartCoroutine(Wait());
        if(sliderPointer.value>=minErrorValue && sliderPointer.value<=maxErrorValue){
            if(curStage<=1){
                //Debug.Log("1");
                bubbleObjects.GetChild(curStage).GetComponent<Animator>().SetTrigger("off");
                bubbleObjects.GetChild(curStage+1).gameObject.SetActive(true);
        yield return wait500ms;
                //curStage = curStage + 1;
                curStage ++;
                StartCoroutine(SetStage());
            }
            else{   
                //Debug.Log("2");
                bubbleObjects.GetChild(curStage).GetComponent<Animator>().SetTrigger("off");
                bubbleObjects.GetChild(curStage+1).gameObject.SetActive(true);        
                yield return wait500ms;

                bubbleObjects.GetChild(curStage+1).GetComponent<Animator>().SetTrigger("finish");
        yield return wait1000ms;
                emptyBottles[curLevel].SetActive(false);
                fullBottles[curLevel].SetActive(true);
        yield return wait1000ms;
                bubbleObjects.GetChild(curStage+1).GetComponent<Animator>().SetTrigger("reset");
                //curLevel ++;
                if(++curLevel <= 2){
                //Debug.Log("3");
                    StartCoroutine(MinigameCoroutine());
                }
                else{
                //Debug.Log("4");

                //게임 성공 success
                    gameObject.SetActive(false);
                }
                
            }
        }
        else{
        yield return wait500ms;
            bubbleObjects.GetChild(curStage).gameObject.SetActive(false);
            failBubbleAnimator.SetTrigger("off"+curStage.ToString());
        yield return wait1000ms;
            StartCoroutine(MinigameCoroutine());
        }
    }    
}
