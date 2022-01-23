using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{   
    public static MinigameManager instance;
    public int nowMinigameNum;
    public Transform minigameParent;
    public GameObject successBtn;
    public Transform[] minigameScriptTransforms;
    //public bool minigameFlag;
    
    WaitForSeconds wait1000ms = new WaitForSeconds(1f);

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
    
#if UNITY_EDITOR || alpha
    void Update(){
        if(PlayerManager.instance.isPlayingMinigame){
            successBtn.SetActive(true);
        }
        else{
            successBtn.SetActive(false);

        }
    }
#endif
    public void SuccessMinigame(){
        success = true;
        PlayerManager.instance.isPlayingMinigame = false;
        Debug.Log(nowMinigameNum + "번 미니게임 종료 : 성공");
    }
    //-1이면 setgameoverui 실행 x
    public void FailMinigame(int gameOverSpriteNum = -1){
        Debug.Log(nowMinigameNum + "번 미니게임 종료 : 실패");
        minigameScriptTransforms[nowMinigameNum].gameObject.SetActive(false);
        PlayerManager.instance.isPlayingMinigame = false;

        if(gameOverSpriteNum != -1){
            UIManager.instance.SetGameOverUI(gameOverSpriteNum);
        }
    }
    public void SuccessMinigameTest(){
        for(int i=0;i<instance.transform.childCount;i++){
            instance.transform.GetChild(i).gameObject.SetActive(false);
        }
        success = true;
        PlayerManager.instance.isPlayingMinigame = false;
        Debug.Log(nowMinigameNum + "번 미니게임 종료 : 성공(테스트)");

        // if(nowMinigameNum == 2){
        //     TriggerScript.instance.Action(16);
        // }
        
    }
    public void GiveReward(int gameNum){
        switch(gameNum){
            case 3 :
                InventoryManager.instance.AddItem(18);
                break;
        }
    }
    public void StartMinigame(int gameNum){


        Debug.Log(gameNum + "번 미니게임 시작");

        StartCoroutine(StartMinigameCoroutine(gameNum));


    }

    IEnumerator StartMinigameCoroutine(int gameNum){

        nowMinigameNum = gameNum;

        LoadManager.instance.FadeOut();
        yield return wait1000ms;


        var nowMinigame = minigameScriptTransforms[gameNum];

        if(gameNum < minigameParent.childCount && !PlayerManager.instance.isPlayingMinigame){
            
            PlayerManager.instance.isPlayingMinigame = true;

            if(nowMinigameNum == 4){
                PlayerManager.instance.canMove = false;
            }



            //nowMinigame.gameObject.SetActive(!nowMinigame.gameObject.activeSelf);
            nowMinigame.gameObject.SetActive(true);

        }

        
        LoadManager.instance.FadeIn();
    }
}
