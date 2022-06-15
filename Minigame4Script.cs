using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
//using System;
//Level, Stage  : Level이 상위 개념
//Level : 총 3레벨
//Stage : 레벨 당 3스테이지

//진딧물 미니게임
public class Minigame4Script : MonoBehaviour
{
    public static Minigame4Script instance;

    //x : 7.5  y : 4
    [Header("[Game Settings]─────────────────")]
    public float maxTimerSet;
    public float aphidLifeTime = 2f;
    public float aphidCreationCycle = 1;
    public int aphidCreationCount = 100;
    public float miniAntSpeed;
    public float miniLuckySpeed;
    public int goalScoreToWin;


    
    [Space]
    [Header("[Game Objects]─────────────────")]
    public GameObject[] aphids;
    public Sprite[] numberSprites;
    public Transform miniLucky, miniAnt;
    public Vector2 miniAntDestination;
    public PolygonCollider2D mapCollider;
    public Transform mapViewPoint;
    public Image[] score_lucky_img, score_ant_img;
    public Image timerGauge;
    public GameObject[] leftRedDots;
    public GameObject[] rightRedDots;
    public GameObject resultBoard;
    public Text resultBoardLeftScoreText;
    public Text resultBoardRightScoreText;
    public Text resultBoardNextAlertText;
    [Space]

    [Header("[Debug]─────────────────")]
    public bool isPaused;
    public int setScore_lucky, setScore_ant;
    public int score_lucky, score_ant;
    public float curTimer;  
    public int closestAphid;
    public List<float> tempDistanceList;
    public List<int> tempAphidNumList;
    public float curMapScrollSpeed = 0 ;
    Coroutine minigameCoroutine;
    public bool isActive;

    WaitForSeconds waitAphidLifeTime;
    WaitForSeconds waitAphidCreationCycle;
    WaitUntil popUpOkayCheck = new WaitUntil(()=>MenuManager.instance.popUpOkayCheck);
    WaitUntil waitInteract = new WaitUntil(()=>PlayerManager.instance.interactInput);
    WaitForSeconds wait100ms = new WaitForSeconds(0.1f);
    WaitForSeconds wait500ms = new WaitForSeconds(0.5f);
    WaitForSeconds wait1s = new WaitForSeconds(1f);
    WaitForSeconds wait2s = new WaitForSeconds(2f);
    WaitForSeconds wait3s = new WaitForSeconds(3f);
    Vector3 flyingBottlePos;
    public Transform test;

    public float wInput;
    public float hInput;
    public float correctionValue;
    public bool creationFlag;

    void Awake(){
        instance = this;
    }
    void Start(){
        waitAphidCreationCycle = new WaitForSeconds(aphidCreationCycle);
    }
    void ResetGameSettings(){
        isPaused = true;
        curTimer = maxTimerSet;
        setScore_lucky=0;
        setScore_ant=0;

        leftRedDots[0].SetActive(false);
        leftRedDots[1].SetActive(false);
        rightRedDots[0].SetActive(false);
        rightRedDots[1].SetActive(false);
    }
    void FixedUpdate(){

        if(!PlayerManager.instance.isGameOver &&PlayerManager.instance.isPlayingMinigame && !isPaused){
            if(curTimer>0){
                curTimer -= Time.fixedDeltaTime;
            }
            else{
                isPaused = true;
                // if(score_ant>score_lucky){
                //     setScore_ant ++;
                // }
                // else{

                // }




                // MinigameManager.instance.SuccessMinigame();
            }
            timerGauge.fillAmount = curTimer / maxTimerSet;
        }
    }
    void GameSet(){
        
    }
    private void OnEnable() {
        PlayerManager.instance.LockPlayer();
        ResetGameSettings();

        minigameCoroutine = StartCoroutine(MinigameCoroutine());

        SceneController.instance.SetSomeConfiner(mapCollider,true);
        SceneController.instance.virtualCamera.Follow = mapViewPoint;

        UIManager.instance.SetHUD(false);
    }
    void OnDisable(){
        SceneController.instance.SetConfiner(DBManager.instance.curData.curMapNum);
        SceneController.instance.virtualCamera.Follow = PlayerManager.instance.transform;
        
        UIManager.instance.SetHUD(true);
        StopAllCoroutines();
    }
    void Update(){
        
        if(!isPaused){

            wInput = Input.GetAxisRaw("Horizontal");
            hInput = Input.GetAxisRaw("Vertical");

            
            if(wInput!=0 || hInput!=0){
                miniLucky.Translate(new Vector2(wInput * miniLuckySpeed,hInput * miniLuckySpeed));
                miniLucky.GetComponent<SpriteRenderer>().flipX = wInput<0 ? true : false;
                miniLucky.GetComponent<Animator>().SetFloat("speed",1f);
            }
            else{
                miniLucky.GetComponent<Animator>().SetFloat("speed",0);

            }

            if(miniAntDestination != Vector2.zero){
                miniAnt.position = Vector2.MoveTowards(miniAnt.position,miniAntDestination,miniAntSpeed);
                miniAnt.GetComponent<SpriteRenderer>().flipX = miniAntDestination.x<miniAnt.position.x ? true : false;
                miniAnt.GetComponent<Animator>().SetFloat("speed",1f);
            }
            else{
                miniAnt.GetComponent<Animator>().SetFloat("speed",0);
            }
        }
    }
    IEnumerator MinigameCoroutine(){
        
        // MenuManager.instance.OpenPopUpPanel_SetStringByIndex("30","9");
        // yield return popUpOkayCheck;
        MinigameManager.instance.OpenGuide(159,160);
        yield return new WaitUntil(()=>!MinigameManager.instance.waitGuidePass);

        while(setScore_ant < goalScoreToWin && setScore_lucky < goalScoreToWin){
            


            curTimer = maxTimerSet;
            score_ant=0;
            score_lucky = 0;
            SetScoreImage();
            isPaused = false;

            SetRandomPosition();

//게임 진행 ( 진딧물 생성 )

            //Coroutine StageCoroutine = StartCoroutine(CreateAphids());
            //for(int i=0;i<aphidCreationCount;i++){
            while(curTimer>0 && !isPaused){
                GameObject curAphid = aphids[Random.Range(0,aphids.Length)];

                if(curAphid.activeSelf){
                // Debug.Log("AA");
                    continue;
                }

                aphids[Random.Range(0,aphids.Length)].SetActive(true);
                SoundManager.instance.PlaySound("generating_aphid_0"+Random.Range(1,4));
                CalculateDistance();
                yield return waitAphidCreationCycle;
            }

//결과
            if(score_ant>score_lucky){
                leftRedDots[setScore_ant].SetActive(true);
                setScore_ant ++;
                // MenuManager.instance.OpenPopUpPanel_SetStringByIndex("32","9");
                // yield return popUpOkayCheck;
            }
            else if(score_ant<score_lucky){
                rightRedDots[setScore_lucky].SetActive(true);
                setScore_lucky ++;
                // MenuManager.instance.OpenPopUpPanel_SetStringByIndex("31","9");
                // yield return popUpOkayCheck;
            }
            // else{
            //     MenuManager.instance.OpenPopUpPanel_SetStringByIndex("35","9");
            //     yield return popUpOkayCheck;
            // }
            

            resultBoardLeftScoreText.text = setScore_ant.ToString();
            resultBoardRightScoreText.text = setScore_lucky.ToString();
            resultBoardNextAlertText.text = GameInputManager.ReadKey("Interact") + CSVReader.instance.GetIndexToString(84,"sysmsg");
            resultBoard.SetActive(true);
            yield return waitInteract;
            resultBoard.SetActive(false);

            
        }
        yield return wait100ms;

        if(setScore_lucky == goalScoreToWin){
            MenuManager.instance.OpenPopUpPanel_SetStringByIndex("33","0");
            yield return popUpOkayCheck;
            MinigameManager.instance.SuccessMinigame();
        }
        else{
            MenuManager.instance.OpenPopUpPanel_SetStringByIndex("34","0");
            yield return popUpOkayCheck;
            MinigameManager.instance.FailMinigame();
        }

    }

    public void SetRandomPosition(){
        foreach(GameObject aphid in aphids){
            aphid.transform.localPosition = new Vector2(Random.Range(-7.5f,7.5f),Random.Range(-4f,2.5f));
        }
    }

    public void CalculateDistance(){
        // tempDistanceList.Clear();
        // tempAphidNumList.Clear();
        Transform closestAphid = null;
        float closestDistance = 100f;

        for(int i=0;i<aphids.Length;i++){
            if(!aphids[i].activeSelf) continue;
            float tempDistance = Vector2.Distance(miniAnt.position,aphids[i].transform.position);
            if(closestDistance>tempDistance){
                closestDistance = tempDistance;
                closestAphid = aphids[i].transform;
            }
            // tempDistanceList.Add(Vector2.Distance(miniAnt.position,aphids[i].transform.position));
            // tempAphidNumList.Add(i);
        }

        miniAntDestination = new Vector2(closestAphid.position.x,closestAphid.position.y);


        //miniAntDestination = new Vector2(closestAphid)
    }

    public void SetScoreImage(){
        score_lucky_img[0].sprite = numberSprites[score_lucky / 10];
        score_lucky_img[1].sprite = numberSprites[score_lucky % 10];
        score_ant_img[0].sprite = numberSprites[score_ant / 10];
        score_ant_img[1].sprite = numberSprites[score_ant % 10];
    }
}
