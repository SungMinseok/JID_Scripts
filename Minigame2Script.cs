using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
//Level, Stage  : Level이 상위 개념
//Level : 총 3레벨
//Stage : 레벨 당 3스테이지
public class Minigame2Script : MonoBehaviour
{
    [Header("[Game Settings]")]
    [Range(0.1f, 10f)] public float mapScrollSpeed = 0.1f;
    public float gameTimer;
    public GameObject destinationMap;
    public Location exitLocation;
    public Transform flyingBottle;
    [Range(0.1f, 10f)] public float flyingBottleSpeed = 0.2f;

    [Space]

    [Header("[Debug]")]
    Coroutine minigameCoroutine;
    public Transform[] spriteSets;
    public Transform startPoint;
    public bool isActive;
    public PolygonCollider2D mapCollider;
    Vector3 flyingBottlePos;

    
    private void OnEnable() {
        //canSelect = true;
        //FirstSetGame();
        //StartCoroutine(Wait());
        minigameCoroutine = StartCoroutine(MinigameCoroutine());

        flyingBottlePos = flyingBottle.localPosition;
        //StartSliderMoving();
    }
    void Update(){
        if(isActive){
            for(var i=0; i<spriteSets.Length; i++){
                spriteSets[i].Translate(new Vector2(-1f * mapScrollSpeed,0));
                if(spriteSets[i].localPosition.x <= -5f){
                    spriteSets[i].localPosition = new Vector2(spriteSets[i].localPosition.x+29.88f,spriteSets[i].localPosition.y);
                }
            }

            
            //flyingBottle.Translate(new Vector3(1f * flyingBottleSpeed,0,0));
            flyingBottle.localPosition = new Vector3(flyingBottle.localPosition.x + flyingBottleSpeed,0,0);
            if(flyingBottle.localPosition.x >= flyingBottlePos.x){
                flyingBottle.localPosition = new Vector3(flyingBottlePos.x-29.88f,flyingBottlePos.y,flyingBottlePos.z);
            }
        }
    }    
    IEnumerator MinigameCoroutine(){

        isActive = true;

        yield return new WaitForSeconds(gameTimer);

        isActive = false;

        //우측 이동 로케이션 활성화
        exitLocation.isLocked = false;
    }
}
