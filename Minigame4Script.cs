using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System;
//Level, Stage  : Level이 상위 개념
//Level : 총 3레벨
//Stage : 레벨 당 3스테이지

//진딧물 미니게임
public class Minigame4Script : MonoBehaviour
{

    //x : 7.5  y : 4
    [Header("[Game Settings]─────────────────")]
    public float aphidLifeTime = 2f;
    public float aphidCreationCycle = 1;
    public int aphidCreationCount = 100;



    
    [Range(0.1f, 3f)] public float mapScrollSpeed = 0.1f;
    [Range(0.1f, 3f)] public float runningMadAntSpeed = 1f;
    [Range(0.1f, 3f)] public float flyingMadAntSpeed = 1.2f;
    public int runningMadAntThrowCount;
    public int flyingMadAntThrowCount;
    public float gameTimer;
    [Space]
    [Header("[Game Objects]─────────────────")]
    public GameObject[] aphids;
    public NPCScript flyingMadAnt;
    public Transform defaultPos0;
    public Transform createPos0;
    public Transform defaultPos1;
    public Transform createPos1;

    [Space]

    [Header("[Debug]─────────────────")]
    public float curMapScrollSpeed = 0 ;
    Coroutine minigameCoroutine;
    public bool isActive;

    WaitForSeconds waitAphidLifeTime;
    WaitForSeconds waitAphidCreationCycle;

    WaitForSeconds wait1s = new WaitForSeconds(1f);
    WaitForSeconds wait2s = new WaitForSeconds(2f);
    WaitForSeconds wait3s = new WaitForSeconds(3f);
    Vector3 flyingBottlePos;
    public Transform test;

    public float wInput;
    public float hInput;
    public float correctionValue;
    public bool creationFlag;

    void Start(){
        waitAphidCreationCycle = new WaitForSeconds(aphidCreationCycle);
    }
    
    private void OnEnable() {

        

        minigameCoroutine = StartCoroutine(MinigameCoroutine());

        int i = Random.Range(0,1);
    }
    // void Update(){
    //     if(!creationFlag){
    //         creationFlag = true;
            
    //     }
        
    // }    
    void FixedUpdate(){
        if(wInput!=0 || hInput!=0){
            test.Translate(new Vector2(wInput * correctionValue,hInput * correctionValue));
        }
    }
    IEnumerator MinigameCoroutine(){

        SetRandomPosition();

        for(int i=0;i<aphidCreationCount;i++){
            GameObject curAphid = aphids[Random.Range(0,aphids.Length)];

            if(curAphid.activeSelf){
                Debug.Log("AA");
                continue;
            }

            aphids[Random.Range(0,aphids.Length)].SetActive(true);
            yield return waitAphidCreationCycle;
        }

    }

    public void SetRandomPosition(){
        foreach(GameObject aphid in aphids){
            aphid.transform.localPosition = new Vector2(Random.Range(-7.5f,7.5f),Random.Range(-4f,4f));
        }
    }
}
