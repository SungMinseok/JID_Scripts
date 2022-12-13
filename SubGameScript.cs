using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// 로메슈제 생성 스크립트
/// </summary>

public class SubGameScript : MonoBehaviour
{
    public static SubGameScript instance;

    //x : 7.5  y : 4
    // [Header("[Game Settings]─────────────────")]
    // [Tooltip("0~1")]
    // public float probability;
    // public int firstGetAmount = 100;
    // public float multipleAmount = 2;
    // public Select goStopSelect;
    // public Select failSelect;



    [Space]
    [Header("[Game Objects]─────────────────")]
    public GameObject[] lomeDrops;


    // [Space]

    // [Header("[Debug]─────────────────")]
    // public bool stop;
    // public int curStage;
    // public float curProbability;




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
        if(DBManager.instance != null){
            if(DBManager.instance.CheckTrigOver(22)||DBManager.instance.CheckTrigOver(27)){
                int tempCount = 3 - InventoryManager.instance.GetItemAmount(55);
                if(tempCount>0) CreateDrops(tempCount);
            }
        
        }
    }
    
    void ResetGameSettings(){

    }

    public void CreateDrops(int dropCount = 3){
        // foreach(GameObject a in lomeDrops){
        //     DBManager.instance.curData.smallRomeActivateCount++;
        //     a.SetActive(true);
        // }
        for (int i = 0; i < dropCount;i++){
            lomeDrops[i].SetActive(true);
            //DBManager.instance.curData.smallRomeActivateCount = dropCount;

        }
    }

}
