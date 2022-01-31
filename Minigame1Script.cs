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
    [Header("단계별 허용 간격")]
    [Header("[Game Settings]━━━━━━━━━━━━━━━━━━━━━━━")]
    public float[] acceptableIntervals;
    [Header("단계별 속도")]
    public float[] pointerSpeedPerStage;
    [Header("최대 목숨")]
    public int maxLife;
    [Space]
    [Header("[Game Objects]━━━━━━━━━━━━━━━━━━━━━━━")]

    public RectTransform errorArea;
    public Animator mainBottle;
    public GameObject[] emptyBottles;
    public GameObject[] fullBottles;
    public Animator failBubbleAnimator;
    public Transform lifeObjectGrid;
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

    [Header("[Debug]━━━━━━━━━━━━━━━━━━━━━━━")]
    public bool isSliderUp;
    Coroutine sliderMovementCoroutine;
    Coroutine minigameCoroutine;
    public bool canSelect;
    public float randomCenterPosVar;
    public int curStage;
    public int curLevel;
    public int curLife;

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
        ResetGameSettings();
        //StartCoroutine(Wait());
        minigameCoroutine = StartCoroutine(MinigameCoroutine());
        //StartSliderMoving();
        UIManager.instance.SetHUD(false);
    }
    void OnDisable(){
        PlayerManager.instance.UnlockPlayer();

        UIManager.instance.SetHUD(true);
        StopAllCoroutines();

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
    void ResetGameSettings(){
        canSelect = false;
        errorArea.gameObject.SetActive(false);
        sliderPointer.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        for(int i=0; i<3; i++){
            emptyBottles[i].SetActive(true);
            fullBottles[i].SetActive(false);
        }
        curLife = maxLife;
        for(int i=0;i<lifeObjectGrid.childCount;i++){
            lifeObjectGrid.GetChild(i).gameObject.SetActive(false);
        }
        
        for(int i=0;i<curLife-1;i++){
            lifeObjectGrid.GetChild(i).gameObject.SetActive(true);
        }
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

        mainBottle.SetTrigger("tilt");
        canSelect = false;

        yield return new WaitForSeconds(1.8f);
        float minErrorValue = randomCenterPosVar - acceptableIntervals[curStage]/2f;
        float maxErrorValue = randomCenterPosVar + acceptableIntervals[curStage]/2f;

        
        errorArea.gameObject.SetActive(false);

        if(sliderPointer.value>=minErrorValue && sliderPointer.value<=maxErrorValue){
            if(curStage<=1){

                bubbleObjects.GetChild(curStage).GetComponent<Animator>().SetTrigger("off");
                bubbleObjects.GetChild(curStage+1).gameObject.SetActive(true);
                yield return wait500ms;

                curStage ++;
                StartCoroutine(SetStage());
            }
            else{   
                bubbleObjects.GetChild(curStage).GetComponent<Animator>().SetTrigger("off");
                bubbleObjects.GetChild(curStage+1).gameObject.SetActive(true);        
                yield return wait500ms;

                bubbleObjects.GetChild(curStage+1).GetComponent<Animator>().SetTrigger("finish");
                yield return wait1000ms;
                emptyBottles[curLevel].SetActive(false);
                fullBottles[curLevel].SetActive(true);
                yield return wait1000ms;
                bubbleObjects.GetChild(curStage+1).GetComponent<Animator>().SetTrigger("reset");
                if(++curLevel <= 2){
                    StartCoroutine(MinigameCoroutine());
                }
                else{
                    //게임성공
                    gameObject.SetActive(false);
                    MinigameManager.instance.SuccessMinigame();
                }
                
            }
        }
        else{
            yield return wait500ms;

            //스테이지 실패 처리
            bubbleObjects.GetChild(curStage).gameObject.SetActive(false);
            failBubbleAnimator.SetTrigger("off"+curStage.ToString());
            curLife --;

            // for(int i=0;i<lifeObjectGrid.childCount;i++){
            //     lifeObjectGrid.GetChild(i).gameObject.SetActive(true);
            // }
            
            // for(int i=0;i<curLife-1;i++){
            //     lifeObjectGrid.GetChild(i).gameObject.SetActive(true);
            // }



            yield return wait1000ms;

            if(curLife > 0){
                lifeObjectGrid.GetChild(curLife-1).gameObject.SetActive(false);
                StartCoroutine(MinigameCoroutine());
            }
            else{
                MinigameManager.instance.FailMinigame(-1);

            }

        }
    }    
}
