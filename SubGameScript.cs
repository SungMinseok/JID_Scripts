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
    }
    
    void ResetGameSettings(){

    }

    public void CreateDrops(){
        foreach(GameObject a in lomeDrops){
            a.SetActive(true);
        }
    }

}
