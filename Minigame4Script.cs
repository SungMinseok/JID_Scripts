using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
//Level, Stage  : Level이 상위 개념
//Level : 총 3레벨
//Stage : 레벨 당 3스테이지
public class Minigame4Script : MonoBehaviour
{
    public Transform[] spriteSets;
    public bool isActive;
    [Range(0.1f, 10f)] public float mapScrollSpeed = 1f;
    public PolygonCollider2D mapCollider;
    public 
    void Update(){
        if(isActive){
            for(var i=0; i<spriteSets.Length; i++){
                spriteSets[i].Translate(new Vector2(-0.1f * mapScrollSpeed,0));
                if(spriteSets[i].localPosition.x <= -5f){
                    spriteSets[i].localPosition = new Vector2(spriteSets[i].localPosition.x+29.88f,spriteSets[i].localPosition.y);
                }
            }
        }
    }
}
