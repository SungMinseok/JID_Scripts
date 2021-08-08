using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame3Script : MonoBehaviour
{
    [Header("게임 최대 시간")]
    [Header("[Game Settings]")]
    // public float maxTimerSet;
    // [Header("정답 맞췄을 때 추가 시간")]
    // public float timerBonus;
    // [Header("정답 틀렸을 때 감소 시간")]
    // public float timerPanelty;


    // public Image timerGauge;
    // public Image mainImage;

    
    // public Sprite[] keySprites,mainSprites;
    // public Transform keyArray;




    public Slider sliderPointer;
    public bool isSliderUp;
    Coroutine sliderMovementCoroutine;

    [Space]

    [Header("Debug")]
    public bool canSelect;





    // public int curGetKey;
    // public float curTimer;  
    // public int curLevel;//0~4  
    // public int curStage;//0~9
    // public List<int> curLevelAnswer;
    // public bool curLevelFlag;
    private void OnEnable() {
        canSelect = true;
        //StartSliderMoving();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate() {
        if(canSelect){
            //sliderPointer.value
            if(isSliderUp){
                Debug.Log("상승중");
                if(sliderPointer.value<=1f && sliderPointer.value >=0){
                    sliderPointer.value += 0.01f;
                    //yield return null;
                }
                else{
                    isSliderUp = !isSliderUp;
                }
            }
            else{
                Debug.Log("하강중");
                if(sliderPointer.value<=1f && sliderPointer.value >=0){
                    sliderPointer.value -= 0.01f;
                    //yield return null;
                }
                else{
                    isSliderUp = !isSliderUp;
                }
            }
        }
    }
    void StartSliderMoving(){
        sliderMovementCoroutine = StartCoroutine(SliderUpAndDown());
    }
    IEnumerator SliderUpAndDown(){
        sliderPointer.value = Random.Range(0f,1f);
        isSliderUp = Random.Range(0,2) == 1 ? true : false;

        if(canSelect){
            if(isSliderUp){
                Debug.Log("상승중");
                if(sliderPointer.value<1f && sliderPointer.value >=0){
                    sliderPointer.value += 0.01f;
                    yield return null;
                }
                else{
                    isSliderUp = !isSliderUp;
                }
            }
            else{
                Debug.Log("하강중");
                if(sliderPointer.value<1f && sliderPointer.value >=0){
                    sliderPointer.value -= 0.01f;
                    yield return null;
                }
                else{
                    isSliderUp = !isSliderUp;
                }
            }
        }
    }
}
