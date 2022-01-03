using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AphidScript : MonoBehaviour
{
    public Minigame4Script minigame4Script;
    public Transform temp0;
    
    WaitForSeconds waitLifeTime = new WaitForSeconds(1f);
    Coroutine LifeCoroutine;

    

    void OnEnable(){
        waitLifeTime = new WaitForSeconds(minigame4Script.aphidLifeTime);

        LifeCoroutine = StartCoroutine(LifeTimeCoroutine());
    }
    void OnDisable(){
        StopCoroutine(LifeCoroutine);
    }

    IEnumerator LifeTimeCoroutine(){
        yield return waitLifeTime;
        this.gameObject.SetActive(false);
    }

    // void Update(){
    //     Debug.Log(Vector2.Distance(gameObject.transform.position,temp0.position));
    // }



}
