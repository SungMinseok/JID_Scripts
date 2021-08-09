using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
//Level, Stage  : Level이 상위 개념
//Level : 총 3레벨
//Stage : 레벨 당 3스테이지
public class Minigame3Script : MonoBehaviour
{
    [Header("허용 간격(0~1)")]
    [Header("[Game Settings]")]
    public float[] acceptableIntervals;
    public float[] pointerSpeedPerStage;
    public RectTransform errorArea;
    public Animator mainBottle;
    public GameObject[] emptyBottles;
    public GameObject[] fullBottles;
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
    public bool isSliderUp;
    Coroutine sliderMovementCoroutine;

    [Space]

    [Header("Debug")]
    Coroutine minigameCoroutine;
    public bool canSelect;
    public float randomCenterPosVar;
    public int curStage;
    public int curLevel;




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
        yield return new WaitForSeconds(1f);
        curStage = 0;
        StartCoroutine(SetStage());
        // canSelect = true;
        // sliderPointer.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        // SetErrorArea(0);
    }
    void SetLevel(){
        for(int i=0;i<bubbleObjects.childCount;i++){
            bubbleObjects.GetChild(i).gameObject.SetActive(false);
        }

        bubbleObjects.GetChild(0).gameObject.SetActive(true);
        //sliderPointer.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }
    IEnumerator SetStage(){
        canSelect = true;
        sliderPointer.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
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
        yield return new WaitForSeconds(1f);
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
        yield return new WaitForSeconds(3f);
        float minErrorValue = randomCenterPosVar - acceptableIntervals[curStage]/2f;
        float maxErrorValue = randomCenterPosVar + acceptableIntervals[curStage]/2f;

        
        errorArea.gameObject.SetActive(false);
        //StartCoroutine(Wait());
        if(sliderPointer.value>=minErrorValue && sliderPointer.value<=maxErrorValue){
            if(curStage<=1){
                Debug.Log("1");
                bubbleObjects.GetChild(curStage).GetComponent<Animator>().SetTrigger("off");
                bubbleObjects.GetChild(curStage+1).gameObject.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                //curStage = curStage + 1;
                curStage ++;
                StartCoroutine(SetStage());
            }
            else{   
                Debug.Log("2");
                bubbleObjects.GetChild(curStage).GetComponent<Animator>().SetTrigger("off");
                bubbleObjects.GetChild(curStage+1).gameObject.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                bubbleObjects.GetChild(curStage+1).GetComponent<Animator>().SetTrigger("finish");
                yield return new WaitForSeconds(1f);
                emptyBottles[curLevel].SetActive(false);
                fullBottles[curLevel].SetActive(true);
                yield return new WaitForSeconds(1f);
                bubbleObjects.GetChild(curStage+1).GetComponent<Animator>().SetTrigger("reset");
                //curLevel ++;
                if(++curLevel <= 2){
                Debug.Log("3");
                    StartCoroutine(MinigameCoroutine());
                }
                else{
                Debug.Log("4");
                    gameObject.SetActive(false);
                }
                
            }
            //curStage ++;
            //bubbleObjects.GetChild(curStage).gameObject.SetActive(true);
            
        }
        else{
                Debug.Log("5");
            bubbleObjects.GetChild(curStage).GetComponent<Animator>().SetTrigger("fail");
            yield return new WaitForSeconds(1f);
            StartCoroutine(MinigameCoroutine());
        }
    }    
    // void CheckPointerPostion_Debug(int curStage){
    //     // switch(curLevel){
    //     //     case 0 : 
    //     //         break;
            
    //     // }
    //     Debug.Log(sliderPointer.value);

    //     float minErrorValue = randomCenterPosVar - acceptableIntervals[curStage]/2f;
    //     float maxErrorValue = randomCenterPosVar + acceptableIntervals[curStage]/2f;
    //     //StartCoroutine(Wait());
    //     if(sliderPointer.value>=minErrorValue && sliderPointer.value<=maxErrorValue){
    //         Debug.Log("성공");
    //     }
    //     else{
    //         Debug.Log("실패");

    //     }
    // }
}
