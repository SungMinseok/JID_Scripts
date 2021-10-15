using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//플레이어 로케이션
public class LocationRader : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Location") ){
            var curLocation = other.GetComponent<Location>();
            if(curLocation.activateTargetMark && !DBManager.instance.CheckTrigOver(curLocation.trigNum) 
            && DBManager.instance.CheckCompletedTrigs(curLocation.trigNum,curLocation.completedTriggerNums)){

                Debug.Log(curLocation.trigNum + "번 트리거 느낌표 활성화");
                //curLocation.targetMark.gameObject.SetActive(true);
                curLocation.targetMark.GetComponent<Animator>().SetTrigger("on");
            }
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Location")){
            var curLocation = other.GetComponent<Location>();
            if(curLocation.activateTargetMark && curLocation.targetMark.GetComponent<RectTransform>().localScale.x>0.1f){

                //Debug.Log(curLocation.trigNum + "번 트리거 느낌표 비활성화");
                //curLocation.targetMark.gameObject.SetActive(false);
                curLocation.targetMark.GetComponent<Animator>().SetTrigger("off");
            }
        }
    }

    public void ResetLocationRader(){
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}

