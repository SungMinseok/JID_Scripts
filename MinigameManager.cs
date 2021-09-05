using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{   
    public static MinigameManager instance;
    public Transform minigameParent;
    public GameObject successBtn;
    public Transform[] minigameScriptTransforms;
    //public bool minigameFlag;

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
    }
    public void SuccessMinigameTest(){
        for(int i=0;i<instance.transform.childCount;i++){
            instance.transform.GetChild(i).gameObject.SetActive(false);
        }
        success = true;
        PlayerManager.instance.isPlayingMinigame = false;
        
    }
    public void GiveReward(int gameNum){
        switch(gameNum){
            case 3 :
                InventoryManager.instance.AddItem(18);
                break;
        }
    }
    public void StartMinigame(int gameNum){
        var nowMinigame = minigameScriptTransforms[gameNum];

        if(gameNum!=2){
            if(gameNum < minigameParent.childCount && !PlayerManager.instance.isPlayingMinigame){
                
                PlayerManager.instance.isPlayingMinigame = true;
                nowMinigame.gameObject.SetActive(!nowMinigame.gameObject.activeSelf);

            }

        }
        else if(gameNum==2){
            PlayerManager.instance.MovePlayer(nowMinigame.GetComponent<Minigame2Script>().startPoint);
            SceneController.instance.SetSomeConfiner(nowMinigame.GetComponent<Minigame2Script>().mapCollider);
        }
    }
}
