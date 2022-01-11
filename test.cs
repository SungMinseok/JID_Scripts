using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
//Level, Stage  : Level이 상위 개념
//Level : 총 3레벨
//Stage : 레벨 당 3스테이지
public class test : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Aphid")){
        Debug.Log("AAAA");
        }
    }
}
