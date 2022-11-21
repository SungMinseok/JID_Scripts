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
    public Select failSelect;


    
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
    public float curProbability;




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
        SoundManager.instance.PlaySound("roulette_button");

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

        DBManager.instance.curData.roulettePlayCount += 1;

    }
    public void PushStopBtn(){
        SoundManager.instance.PlaySound("roulette_button");
        stopBtn.gameObject.SetActive(false);
        stop = true;
        startBtn.gameObject.SetActive(true);
    }
    IEnumerator RouletteCoroutine(){
        startBtn.interactable = false;
        SoundManager.instance.PlayLoopSound("roulette_win_half");

        for(int i=0;i<3;i++){
            rouletteImages[i].GetComponent<Animator>().enabled = true;
            rouletteImages[i].GetComponent<Animator>().SetBool("rotate",true);
        }
        yield return waitStop;
        stop = false;

        int resultValue = Random.Range(0,10000);
        bool isSuccessed = resultValue <= 10000*probability*(1+PlayerManager.instance.stat.luckBonus) ? true : false;
        Debug.Log("보정확률 : "+10000*probability*(1+PlayerManager.instance.stat.luckBonus));


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
            SoundManager.instance.StopLoopSound();
            SoundManager.instance.PlaySound("roulette_check_0"+Random.Range(1,4));
            rouletteImages[i].GetComponent<Animator>().SetBool("rotate",false);
            rouletteImages[i].GetComponent<Animator>().enabled = false;
            rouletteImages[i].sprite = rouletteSprites[order[i]];
            //Debug.Log(order[i]);
            yield return wait500ms;
        }

        if(isSuccessed){
            SoundManager.instance.PlaySound("roulette_yay");
            resultEffectAnimator.SetBool("success",true);

            curStage ++ ;

            if(curStage == 2){
                SteamAchievement.instance.ApplyAchievements(3);
            }
            else if(curStage == 5){
                SteamAchievement.instance.ApplyAchievements(2);

            }

            for(int i=0;i<3;i++){
                fruitBursts[i].SetActive(true);
            }
            nextHoneyText.text = (firstGetAmount*Mathf.Pow(multipleAmount,curStage)).ToString();
        }
        else{
            SoundManager.instance.PlaySound("roulette_fail");
            resultEffectAnimator.SetBool("fail",true);
            curStage = 0;
            nextHoneyText.text = "0";
        }
        
        yield return wait1000ms;
        string[] tempStrings = new string[]{};

        if(isSuccessed){
            tempStrings = new string[]{(int.Parse(nextHoneyText.text)*2).ToString(),nextHoneyText.text};
            SelectManager.instance.SetSelect(goStopSelect, tempStrings);
        }
        else{
            tempStrings = new string[]{(firstGetAmount*multipleAmount).ToString(),""};
            SelectManager.instance.SetSelect(failSelect, tempStrings);
        }
        

        yield return waitSelecting;
        UIManager.instance.ClearStringArray(tempStrings);
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
            if(isSuccessed){
                DBManager.instance.curData.curHoneyAmount += int.Parse(nextHoneyText.text);
                
            }
            
           
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
        MinigameManager.instance.ExitMinigame();
        if(!PlayerManager.instance.isActing){
        PlayerManager.instance.UnlockPlayer();

        }
        
        UIManager.instance.SetHUD(true);
        StopAllCoroutines();
    }
}
