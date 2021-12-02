using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Level, Stage  : Level이 상위 개념
public class Minigame0Script : MonoBehaviour
{
    [Header("게임 최대 시간")]
    [Header("[Game Settings]")]
    public float maxTimerSet;
    [Header("정답 맞췄을 때 추가 시간")]
    public float timerBonus;
    [Header("정답 틀렸을 때 감소 시간")]
    public float timerPanelty;


    public Image timerGauge;
    public Image mainImage;

    
    public Sprite[] keySprites,mainSprites;
    public Transform keyArray;

    [Space]

    [Header("[Debug]")]
    public int curGetKey;
    public float curTimer;  
    public int curLevel;//0~4  
    public int curStage;//0~9
    public List<int> curLevelAnswer;
    public bool curLevelFlag;

    
    void OnEnable(){
        //curTimer = maxTimerSet;

        PlayerManager.instance.LockPlayer();

        ResetValue();

        StartCoroutine(MinigameCoroutine());
    }
    void OnDisable(){
        PlayerManager.instance.UnlockPlayer();


    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(curLevelFlag){

            if(Input.GetButtonDown("Vertical") || Input.GetButtonDown("Horizontal") ){
                if(Input.GetAxisRaw("Vertical")<0){
                    curGetKey = 2;
                }
                else if(Input.GetAxisRaw("Vertical")>0){
                    curGetKey = 0;
                }

                if(Input.GetAxisRaw("Horizontal")<0){
                    curGetKey = 3;
                }
                else if(Input.GetAxisRaw("Horizontal")>0){
                    curGetKey = 1;
                }
                CheckCorrectKey();
            }
        }
        // else if(Input.GetButtonDown("Horizontal")){

        // }

    }
    void FixedUpdate(){
        if(curTimer>0) curTimer -= Time.fixedDeltaTime;
        timerGauge.fillAmount = curTimer / maxTimerSet;
    }
    void ResetValue(){
        //curLevelAnswer.Clear();
        curGetKey = 0;
        curLevel = 0;
        curStage = 0;
        curTimer = maxTimerSet;
    }

    void SetRandomKeyArray(){
        for(int i=0; i<keyArray.childCount; i++){
            int randomNum = Random.Range(0,4);

            keyArray.GetChild(i).GetComponent<Animator>().SetBool("pop",false);

            keyArray.GetChild(i).GetComponent<Image>().sprite = keySprites[randomNum];
            curLevelAnswer.Add(randomNum);
        }
    }
    void SetStageSprite(int levelNum){

        curLevel = levelNum;
        mainImage.sprite = mainSprites[curLevel*11];

    }
    void CheckCorrectKey(){
        //Debug.Log(curGetKey);
        //Debug.Log((int)curAnswer[curStage]);

        if(curGetKey == curLevelAnswer[curStage]){
            if(curStage<=9){


                keyArray.GetChild(curStage).GetComponent<Animator>().SetBool("pop",true);

                curStage ++;

                mainImage.sprite = mainSprites[curLevel*11 + curStage];


                if(curStage==10){
                    curLevelFlag = false;
                    curStage = 0;
                }
            }
        }
        else{

            AddTimer(-timerPanelty);
        }
    }
    void AddTimer(float amount){
        curTimer += amount;
        if(curTimer>maxTimerSet){
            curTimer=maxTimerSet;
        } 
    }
    IEnumerator MinigameCoroutine(){
        
        for(int i=0;i<5;i++){
            curLevelFlag = true;

            curLevelAnswer.Clear();

            SetRandomKeyArray();
            SetStageSprite(i);

            yield return new WaitUntil(()=>!curLevelFlag);
            //curLevelFlag = false;
            yield return new WaitForSeconds(1f);

        }

        gameObject.SetActive(false);
        
        MinigameManager.instance.SuccessMinigame();






    }

}
