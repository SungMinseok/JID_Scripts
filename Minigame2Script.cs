﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
//Level, Stage  : Level이 상위 개념
//Level : 총 3레벨
//Stage : 레벨 당 3스테이지
public class Minigame2Script : MonoBehaviour
{
    public static Minigame2Script instance;
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
    [Range(1f, 4f)] public float flyingBottleSpeed = 2f;
    [Range(8f, 15f)] public float throwingBottleSpeed = 10f;
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
    
    void Awake(){
        instance = this;
    }
    
    private void OnEnable() {
        //canSelect = true;
        //FirstSetGame();
        //StartCoroutine(Wait());
        minigameCoroutine = StartCoroutine(MinigameCoroutine());

        flyingBottlePos = flyingBottle.localPosition;
        //StartSliderMoving();
        UIManager.instance.SetHUD(false);
    }
    void OnDisable(){
        UIManager.instance.SetHUD(true);
        StopAllCoroutines();
    }
    void Update(){
        if(isActive){
            for(var i=0; i<spriteSets.Length; i++){
                spriteSets[i].Translate(new Vector2(-1f * curMapScrollSpeed,0));
                if(spriteSets[i].localPosition.x <= -5f){
                    spriteSets[i].localPosition = new Vector2(spriteSets[i].localPosition.x+29.88f,spriteSets[i].localPosition.y);
                }
            }

            
            // flyingBottle.localPosition = new Vector3(flyingBottle.localPosition.x + flyingBottleSpeed,0,0);
            // if(flyingBottle.localPosition.x >= flyingBottlePos.x){
            //     flyingBottle.localPosition = new Vector3(flyingBottlePos.x-29.88f,flyingBottlePos.y,flyingBottlePos.z);
            // }
        }
    }    
    IEnumerator MinigameCoroutine(){

        PlayerManager.instance.transform.position = startPoint.position;
        SceneController.instance.SetSomeConfiner(mapCollider);
        SceneController.instance.virtualCamera.Follow = null;

        isActive = true;

        PlayerManager.instance.isForcedRun = true;


        while(curMapScrollSpeed < mapScrollSpeed){
            curMapScrollSpeed += 0.005f;
            yield return null;
        }


        //뛰다니는 미친 개미

#region RunningMode
        yield return new WaitForSeconds(1f);

        runningMadAnt.transform.localPosition = createPos0.localPosition;

        while(runningMadAnt.transform.localPosition.x < defaultPos0.localPosition.x){
            runningMadAnt.transform.localPosition += Vector3.right * runningMadAntSpeed * Time.deltaTime  ;
            yield return null;
        }
        
        //소주병 n 번 날림
        for(int i=0;i<runningMadAntThrowCount;i++){
            runningMadAnt.mainBody.GetComponent<Animator>().SetTrigger("throw");
            yield return wait3s;
        }

        while(runningMadAnt.transform.localPosition.x > createPos0.localPosition.x){
            runningMadAnt.transform.localPosition += Vector3.left * runningMadAntSpeed * Time.deltaTime  ;
            yield return null;
        }
#endregion
#region FlyingMode
        //날으는 미친 개미 (기본등장)
        
        while(flyingMadAnt.transform.localPosition.x < defaultPos1.localPosition.x){
            flyingMadAnt.transform.localPosition += Vector3.right * flyingMadAntSpeed * Time.deltaTime  ;
            yield return null;
        }
        //NPCSkillScript.cs에서 관리
        //소주병 n 번 날림
        for(int i=0;i<flyingMadAntThrowCount;i++){
            var movingObject = flyingMadAnt;
            var targetPos = Vector2.zero;

            //Debug.Log(i + "번째 ," + targetPos+" / "+movingObject.transform.position);

            while(!flyingMadAnt.mainBody.GetComponent<NPCSkillScript>().raderFlag ){
                //flyingMadAnt.transform.localPosition += Vector3.right * flyingMadAntSpeed * Time.deltaTime  ;
                targetPos = new Vector2(PlayerManager.instance.playerBody.transform.position.x,movingObject.transform.position.y);
                movingObject.transform.position = Vector3.MoveTowards(movingObject.transform.position, targetPos, 7f * Time.deltaTime);

                yield return null;
            }




            flyingMadAnt.mainBody.GetComponent<Animator>().SetTrigger("throw_immy");
            yield return new WaitForSeconds(3f);
        }

        while(flyingMadAnt.transform.localPosition.x > createPos1.localPosition.x){
            flyingMadAnt.transform.localPosition += Vector3.left * flyingMadAntSpeed * Time.deltaTime  ;
            yield return null;
        }
#endregion
        //yield return new WaitForSeconds(gameTimer);

        isActive = false;

        //우측 이동 로케이션 활성화
        exitLocation.isLocked = false;

        
        DBManager.instance.TrigOver(1002);
        MinigameManager.instance.SuccessMinigame();
        PlayerManager.instance.isForcedRun = false;

        

        
        
    }
}
