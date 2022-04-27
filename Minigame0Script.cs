using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Level, Stage  : Level이 상위 개념
public class Minigame0Script : MonoBehaviour
{
    public static Minigame0Script instance;
    [Header("게임 최대 시간")]
    [Header("[Game Settings]━━━━━━━━━━━━━━━━━━━━━━━")]
    public float maxTimerSet;
    [Header("정답 맞췄을 때 추가 시간")]
    public float timerBonus;
    [Header("정답 틀렸을 때 감소 시간")]
    public float timerPanelty;


    [Space]
    [Header("[Game Objects]━━━━━━━━━━━━━━━━━━━━━━━")]
    public Image timerGauge;
    public Image mainImage;

    
    public Sprite[] keySprites,mainSprites;
    public Transform keyArray;

    public Animator handAnimator;
    public GameObject gameOverImage;
    [Space]

    [Header("[Debug]━━━━━━━━━━━━━━━━━━━━━━━")]
    public bool isPaused;
    public float curTimer;  
    public int curGetKey;
    public int curLevel;//0~4  
    public int curStage;//0~9
    public List<int> curLevelAnswer;
    public bool curLevelFlag;

    
    void OnEnable(){
        //curTimer = maxTimerSet;

        PlayerManager.instance.LockPlayer();

        ResetGameSettings();

        StartCoroutine(MinigameCoroutine());
        UIManager.instance.SetHUD(false);

#if UNITY_EDITOR || alpha
        MinigameManager.instance.successBtn.SetActive(true);
        MinigameManager.instance.failBtn.SetActive(true);
#endif
    }
    void OnDisable(){
        if(!PlayerManager.instance.isActing){
            PlayerManager.instance.UnlockPlayer();
            UIManager.instance.SetHUD(true);
        }

        StopAllCoroutines();

#if UNITY_EDITOR || alpha
        MinigameManager.instance.successBtn.SetActive(false);
        MinigameManager.instance.failBtn.SetActive(false);
#endif
    }
    // Start is called before the first frame update
    void Awake()
    {
        instance =this;
    }

    // Update is called once per frame
    void Update()
    {
        if(curLevelFlag && !PlayerManager.instance.isGameOver){

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

        if(!PlayerManager.instance.isGameOver &&PlayerManager.instance.isPlayingMinigame && !isPaused){
            if(curTimer>0){
                curTimer -= Time.fixedDeltaTime;
            }
            else{
                //MinigameManager.instance.FailMinigame(3);
                PlayerManager.instance.isGameOver = true;
                ActivateGameOver();
                //UIManager.instance.SetGameOverUI(2);
            }
            timerGauge.fillAmount = curTimer / maxTimerSet;
        }
    }
    void ResetGameSettings(){
        //curLevelAnswer.Clear();
        curGetKey = 0;
        curLevel = 0;
        curStage = 0;
        curTimer = maxTimerSet;
        isPaused = false;
    }

    void SetRandomKeyArray(){
        for(int i=0; i<keyArray.childCount; i++){
            int randomNum = Random.Range(0,4);

            keyArray.GetChild(i).GetComponent<Animator>().SetBool("pop",false);

            keyArray.GetChild(i).GetComponent<Image>().sprite = keySprites[randomNum];
            curLevelAnswer.Add(randomNum);
            //Debug.Log(i);
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
                handAnimator.SetTrigger("act");

                curStage ++;

                mainImage.sprite = mainSprites[curLevel*11 + curStage];


                if(curStage==10){
                    SoundManager.instance.PlaySound("cut_paper");
                    curLevelFlag = false;
                    curStage = 0;
                }
                else{
                    
                    SoundManager.instance.PlaySound("button_01");
                }
            }
        }
        else{

            SoundManager.instance.PlaySound("cutting_wrongkey");
            curLevelAnswer.Clear();
            SetRandomKeyArray();
            SetStageSprite(curLevel);
            curStage = 0;
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
            //Debug.Log("111");
            curLevelFlag = true;

            curLevelAnswer.Clear();

            SetRandomKeyArray();
            SetStageSprite(i);

            yield return new WaitUntil(()=>!curLevelFlag);
            //curLevelFlag = false;

            if(i<4) yield return new WaitForSeconds(1f);

        }

        //gameObject.SetActive(false);
        
        isPaused = true;
        MinigameManager.instance.SuccessMinigame();
        
    }
    public void ActivateGameOver(){

        StartCoroutine(ActivateGameOverCoroutine());
    }

    IEnumerator ActivateGameOverCoroutine(){
        SoundManager.instance.BgmOff();
        yield return new WaitForSeconds(0.5f);
        gameOverImage.gameObject.SetActive(true);
        SoundManager.instance.PlaySound("ending_minigamefail");
        yield return new WaitForSeconds(1f);
        MinigameManager.instance.FailMinigame(3);
    }
}
