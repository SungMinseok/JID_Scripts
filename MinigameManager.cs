using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{   
    public static MinigameManager instance;
    public int nowMinigameNum;
    public Transform minigameParent;
    public GameObject successBtn;
    public GameObject failBtn;
    public Transform[] minigameScriptTransforms;
    //public bool minigameFlag;
    
    WaitForSeconds wait1000ms = new WaitForSeconds(1f);
    [Space]
    [Header("[Game Objects]━━━━━━━━━━━━━━━━━━━")]
    public GameObject minigameGuidePopUp;
    public Text guideMainText;
    public Text guideSubText;
    public string[] minigameBgmFileName;
    [Header("[Debug]━━━━━━━━━━━━━━━━━━━")]
    public bool waitGuidePass;

    public bool success, fail;

    void Awake(){
        instance = this;
    }
    void Start()
    {
        // for(int i=0;i<minigameParent.childCount;i++){
        //     //if(i==minigameParent.childCount-1) break;
        //     minigameScriptTransforms[i] = minigameParent.GetChild(i);
        // }
    }
    
//#if UNITY_EDITOR || alpha
    void Update(){
        if(waitGuidePass){
            
            if ( PlayerManager.instance.interactInput )
            {
                CloseGuide();
            }
        }
    }
//#endif
    public void SuccessMinigame(){
        success = true;
        Debug.Log(nowMinigameNum + "번 미니게임 종료 : 성공");
        SoundManager.instance.PlaySound("minigame_complete");
        StartCoroutine(FinishMinigameCoroutine());
    }
    //-1이면 setgameoverui 실행 x
    public void FailMinigame(int gameOverSpriteNum = -1){
        fail = true;
        Debug.Log(nowMinigameNum + "번 미니게임 종료 : 실패");
        if(gameOverSpriteNum != -1){
            UIManager.instance.SetGameOverUI(gameOverSpriteNum);
        }
        else{
            StartCoroutine(FinishMinigameCoroutine());
        }
        
    }
    public void SuccessMinigameTest(){
        // for(int i=0;i<instance.transform.childCount;i++){
        //     instance.transform.GetChild(i).gameObject.SetActive(false);
        // }
        // //success = true;
        // PlayerManager.instance.isPlayingMinigame = false;
        
        MinigameManager.instance.successBtn.SetActive(false);
        MinigameManager.instance.failBtn.SetActive(false);
        success = true;
        Debug.Log(nowMinigameNum + "번 미니게임 강제 성공 (테스트)");
        if(nowMinigameNum == 2){
            Minigame2Script.instance.Success();
        }
        else{
            StartCoroutine(FinishMinigameCoroutine());

        }
    }
    public void FailMinigameTest(){
        
        MinigameManager.instance.successBtn.SetActive(false);
        MinigameManager.instance.failBtn.SetActive(false);
        if(nowMinigameNum == 0){
            Minigame0Script.instance.ActivateGameOver();
        }
        else{

            // for(int i=0;i<instance.transform.childCount;i++){
            //     instance.transform.GetChild(i).gameObject.SetActive(false);
            // }
            //success = true;
            PlayerManager.instance.isPlayingMinigame = false;
            fail = true;
        }

            Debug.Log(nowMinigameNum + "번 미니게임 강제 실패 (테스트)");
            
            StartCoroutine(FinishMinigameCoroutine());
    }
    public void GiveReward(int gameNum){
        switch(gameNum){
            case 3 :
                InventoryManager.instance.AddItem(18);
                break;
        }
    }
    public void StartMinigame(int gameNum){

        PlayerManager.instance.isPlayingMinigame = true;
        // if(nowMinigameNum == 2){
        //     PlayerManager.instance.canMove = false;
        // }

        Debug.Log(gameNum + "번 미니게임 시작");

        StartCoroutine(StartMinigameCoroutine(gameNum));


    }

    IEnumerator StartMinigameCoroutine(int gameNum){
        ResetMinigameResult();
        nowMinigameNum = gameNum;

        SoundManager.instance.ChangeBgm(minigameBgmFileName[gameNum]);

        LoadManager.instance.FadeOut();
        yield return wait1000ms;
        var nowMinigame = minigameScriptTransforms[gameNum];

        if(gameNum < minigameParent.childCount){// && !PlayerManager.instance.isPlayingMinigame){
            
            if(nowMinigameNum == 4){
                PlayerManager.instance.canMove = false;
            }
            nowMinigame.gameObject.SetActive(true);

        }
        LoadManager.instance.FadeIn();
    }
    void FinishMinigame(int gameOverSpriteNum){

    }
    IEnumerator FinishMinigameCoroutine(){
        SoundManager.instance.SetBgmByMapNum(DBManager.instance.curData.curMapNum);


        LoadManager.instance.FadeOut();
        yield return wait1000ms;
        yield return wait1000ms;
        if(nowMinigameNum == 4){
            yield return wait1000ms;

        }
        var nowMinigame = minigameScriptTransforms[nowMinigameNum];
        nowMinigame.gameObject.SetActive(false);
        LoadManager.instance.FadeIn();
        nowMinigameNum = -1;
        PlayerManager.instance.isPlayingMinigame = false;
        //ResetMinigameResult();
    }
    public void ResetMinigameResult(){
        success = false;
        fail = false;
    }
    public void OpenGuide(int mainSysMsgIndex, int subSysMsgIndex){
        MenuManager.instance.ApplyFont(guideMainText);
        MenuManager.instance.ApplyFont(guideSubText);
        guideMainText.text = CSVReader.instance.GetIndexToString(mainSysMsgIndex,"sysmsg");
        guideSubText.text = CSVReader.instance.GetIndexToString(subSysMsgIndex,"sysmsg");
        minigameGuidePopUp.SetActive(true);
        waitGuidePass = true;
    }
    public void CloseGuide(){
        minigameGuidePopUp.SetActive(false);
        waitGuidePass = false;
    }
}
