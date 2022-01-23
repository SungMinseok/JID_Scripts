﻿using System.Collections;
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
    public int goalScore;


    
    [Space]
    [Header("[Game Objects]─────────────────")]
    public GameObject[] aphids;
    public Sprite[] numberSprites;
    public Transform miniLucky, miniAnt;
    public Vector2 miniAntDestination;
    public PolygonCollider2D mapCollider;
    public Transform mapViewPoint;
    public int score_lucky, score_ant;
    public Image[] score_lucky_img, score_ant_img;
    public Image timerGauge;
    [Space]

    [Header("[Debug]─────────────────")]
    public bool isPaused;
    public float curTimer;  
    public int closestAphid;
    public List<float> tempDistanceList;
    public List<int> tempAphidNumList;
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

    void Awake(){
        instance = this;
    }
    void Start(){
        waitAphidCreationCycle = new WaitForSeconds(aphidCreationCycle);
    }
    void ResetGameSettings(){
        curTimer = maxTimerSet;
    }
    void FixedUpdate(){

        if(!PlayerManager.instance.isGameOver &&PlayerManager.instance.isPlayingMinigame && !isPaused){
            if(curTimer>0){
                curTimer -= Time.fixedDeltaTime;
            }
            else{
                MinigameManager.instance.SuccessMinigame();
                //MinigameManager.instance.FailMinigame();
                //UIManager.instance.SetGameOverUI(2);
            }
            timerGauge.fillAmount = curTimer / maxTimerSet;
        }
    }
    private void OnEnable() {
        PlayerManager.instance.LockPlayer();
        ResetGameSettings();

        minigameCoroutine = StartCoroutine(MinigameCoroutine());

        SceneController.instance.SetSomeConfiner(mapCollider);
        SceneController.instance.virtualCamera.Follow = mapViewPoint;

        UIManager.instance.SetHUD(false);
    }
    void OnDisable(){
        
        UIManager.instance.SetHUD(true);
        StopAllCoroutines();
    }
    void Update(){
        
        wInput = Input.GetAxisRaw("Horizontal");
        hInput = Input.GetAxisRaw("Vertical");

        
        if(wInput!=0 || hInput!=0){
            miniLucky.Translate(new Vector2(wInput * miniLuckySpeed,hInput * miniLuckySpeed));
        }

        if(miniAntDestination != Vector2.zero){
            miniAnt.position = Vector2.MoveTowards(miniAnt.position,miniAntDestination,miniAntSpeed);
        }
    }
    IEnumerator MinigameCoroutine(){

        SetRandomPosition();

        for(int i=0;i<aphidCreationCount;i++){
            GameObject curAphid = aphids[Random.Range(0,aphids.Length)];

            if(curAphid.activeSelf){
               // Debug.Log("AA");
                continue;
            }

            aphids[Random.Range(0,aphids.Length)].SetActive(true);
            CalculateDistance();
            yield return waitAphidCreationCycle;
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
