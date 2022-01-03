using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
//Level, Stage  : Level이 상위 개념
//Level : 총 3레벨
//Stage : 레벨 당 3스테이지

//냉동굴 미니게임

public class Minigame3Script : MonoBehaviour
{
    [Header("[Game Settings]─────────────────")]
    [Range(0.1f, 3f)] public float mapScrollSpeed = 0.1f;
    [Range(0.1f, 3f)] public float runningMadAntSpeed = 1f;
    [Range(0.1f, 3f)] public float flyingMadAntSpeed = 1.2f;
    public int runningMadAntThrowCount;
    public int flyingMadAntThrowCount;
    public float gameTimer;
    public GameObject destinationMap;
    public Location exitLocation;
    public Transform flyingBottle;
    [Range(0.1f, 3f)] public float flyingBottleSpeed = 0.2f;
    [Space]
    [Header("[Game Objects]─────────────────")]
    public NPCScript runningMadAnt;
    public NPCScript flyingMadAnt;
    public Transform defaultPos0;
    public Transform createPos0;
    public Transform defaultPos1;
    public Transform createPos1;

    [Space]

    public Transform[] spriteSets;
    public Transform startPoint;
    public PolygonCollider2D mapCollider;
    [Space]

    [Header("[Debug]─────────────────")]
    public float curMapScrollSpeed = 0 ;
    Coroutine minigameCoroutine;
    public bool isActive;

    WaitForSeconds wait1s = new WaitForSeconds(1f);
    WaitForSeconds wait2s = new WaitForSeconds(2f);
    WaitForSeconds wait3s = new WaitForSeconds(3f);
    Vector3 flyingBottlePos;
    public Transform test;

    public float wInput;
    public float hInput;
    public float correctionValue;
    
    private void OnEnable() {
        minigameCoroutine = StartCoroutine(MinigameCoroutine());
    }
    void Update(){
        
        wInput = Input.GetAxisRaw("Horizontal");
        hInput = Input.GetAxisRaw("Vertical");

        
    }    
    void FixedUpdate(){
        if(wInput!=0 || hInput!=0){
            test.Translate(new Vector2(wInput * correctionValue,hInput * correctionValue));
        }
    }
    IEnumerator MinigameCoroutine(){
        yield return null;
        
        
    }
}
