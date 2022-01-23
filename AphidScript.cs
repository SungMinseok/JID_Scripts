using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AphidScript : MonoBehaviour
{
    public enum UnitType{
        aphid,
        ant,
        lucky
    }
    public UnitType unitType;
    //public Minigame4Script minigame4Script;
    
    [Header("Aphid")]
    WaitForSeconds waitLifeTime = new WaitForSeconds(1f);
    Coroutine LifeCoroutine;
    [Header("Ant")]
    public bool getFlag;
    [Header("Lucky")]
    public bool canGetAphid;//진딧물 획득 가능상태




    public Transform temp0;
    

    void Awake(){
        switch(unitType){
            case UnitType.aphid :
            
                Debug.Log(unitType);
                break;
            case UnitType.ant :
                Debug.Log(unitType);
                break;
            case UnitType.lucky :
                Debug.Log(unitType);
                break;
            default:
                Debug.Log("has no unitType");
                break;

        }
    }

    void OnEnable(){
        switch(unitType){
            case UnitType.aphid :
            
                waitLifeTime = new WaitForSeconds(Minigame4Script.instance.aphidLifeTime);
                LifeCoroutine = StartCoroutine(LifeTimeCoroutine());
                break;
            case UnitType.ant :
                Debug.Log("b");
                break;
            case UnitType.lucky :
                break;
            default:
                Debug.Log("has no unitType");
                break;

        }
    }
    void OnDisable(){
        switch(unitType){
            case UnitType.aphid :
                StopCoroutine(LifeCoroutine);
                break;
            case UnitType.ant :
                break;
            case UnitType.lucky :
                break;
            default:
                Debug.Log("has no unitType");
                break;

        }
    }
    void OnTriggerStay2D(Collider2D other) {
        //Debug.Log("AAAA");
        switch(unitType){
            case UnitType.aphid :
                break;
            case UnitType.ant :
                if(other.CompareTag("Aphid")){

                    if(!getFlag){
                        getFlag = true;
                        GetAphid(other.gameObject);
                    }
                }
                break;
            case UnitType.lucky :
                if(other.CompareTag("Aphid")){

                    if(!getFlag){
                        getFlag = true;
                        GetAphid(other.gameObject);
                    }
                }


                break;
            default:
                Debug.Log("has no unitType");
                break;

        }
    }
    void OnTriggerExit2D(Collider2D other){

        Debug.Log("3");
        switch(unitType){
            case UnitType.aphid :
                break;
            case UnitType.ant :
                break;
            case UnitType.lucky :
                break;
            default:
                Debug.Log("has no unitType");
                break;

        }
    }

    IEnumerator LifeTimeCoroutine(){
        yield return waitLifeTime;
        this.gameObject.SetActive(false);
    }
    void GetAphid(GameObject target){
        target.SetActive(false);
        if(unitType ==  UnitType.lucky){

            Minigame4Script.instance.score_lucky ++ ;
        }
        else{

            Minigame4Script.instance.score_ant ++ ;
        }
        Minigame4Script.instance.SetScoreImage();
        getFlag = false;
    }


    // void Update(){
    //     Debug.Log(Vector2.Distance(gameObject.transform.position,temp0.position));
    // }



}
