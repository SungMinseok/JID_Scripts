using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{   
    public static MinigameManager instance;
    public GameObject successBtn;
    //public bool minigameFlag;

    public bool success, fail;

    void Awake(){
        instance = this;
    }
    void Start()
    {

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
}
