using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//플레이어 로케이션
public class LocationRader : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Location") ){
            var curLocation = other.GetComponent<Location>();
            if(curLocation.activateTargetMark 
            && !DBManager.instance.CheckTrigOver(curLocation.trigNum) 
            && DBManager.instance.CheckCompletedTrigs(curLocation.trigNum,curLocation.completedTriggerNums,printDebug:false)
            && DBManager.instance.CheckIncompletedTrigs(curLocation.trigNum,curLocation.incompletedTriggerNums,false)
            && DBManager.instance.CheckHaveItems(curLocation.trigNum,curLocation.haveItemNums,false)
            && DBManager.instance.CheckNotHaveItems(curLocation.trigNum,curLocation.notHaveItemNums,false)
            ){

                //curLocation.targetMark.GetComponent<Animator>().SetTrigger("on");
                curLocation.targetMark.GetComponent<Animator>().SetBool("activate",true);
            }
        }
        else if(other.CompareTag("NPC")) {
            if(other.GetComponent<NPCScript>().mainBody != null && other.GetComponent<NPCScript>().mainBody.TryGetComponent<BodySoundScript>(out BodySoundScript bodySoundScript)){
                bodySoundScript.isNearPlayer = true;
            }
        }
        else if(other.CompareTag("Box")) {
            if(other.TryGetComponent<BodySoundScript>(out BodySoundScript bodySoundScript)){
                bodySoundScript.isNearPlayer = true;
            }
        }
        else if(other.CompareTag("Speaker")) {
            if(other.TryGetComponent<BodySoundScript>(out BodySoundScript bodySoundScript)){
                bodySoundScript.isNearPlayer = true;
            }
        }
    }
    void OnTriggerStay2D(Collider2D other){
        if(other.CompareTag("Location") ){
            var curLocation = other.GetComponent<Location>();
            if(curLocation.activateTargetMark 
            && !PlayerManager.instance.canMove
            ){

//                Debug.Log(curLocation.trigNum + "번 트리거 느낌표 활성화");
                //curLocation.targetMark.gameObject.SetActive(true);
                //curLocation.targetMark.GetComponent<Animator>().SetTrigger("off");
                curLocation.targetMark.GetComponent<Animator>().SetBool("activate",false);
            }
        }
        else if(other.CompareTag("NPC")) {
            if(PlayerManager.instance.equipments_id[0]==-1
             && !PlayerManager.instance.isCaught && other.GetComponent<NPCScript>().isGuard){
                PlayerManager.instance.isCaught = true;
                other.GetComponent<NPCScript>().CatchPlayerAsGuard();
            }
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Location")){
            var curLocation = other.GetComponent<Location>();
            if(curLocation.activateTargetMark && curLocation.targetMark.GetComponent<RectTransform>().localScale.x>0.1f){

                //Debug.Log(curLocation.trigNum + "번 트리거 느낌표 비활성화");
                //curLocation.targetMark.gameObject.SetActive(false);
                curLocation.targetMark.GetComponent<Animator>().SetBool("activate",false);
            }
        }
        else if(other.CompareTag("NPC")) {
                //Debug.Log(other.name);

            if(other.GetComponent<NPCScript>().mainBody != null && other.GetComponent<NPCScript>().mainBody.TryGetComponent<BodySoundScript>(out BodySoundScript bodySoundScript)){
                bodySoundScript.isNearPlayer = false;
            }
        }
        else if(other.CompareTag("Box")) {
            if(other.TryGetComponent<BodySoundScript>(out BodySoundScript bodySoundScript)){
                bodySoundScript.isNearPlayer = false;
            }
        }
        else if(other.CompareTag("Speaker")) {
            if(other.TryGetComponent<BodySoundScript>(out BodySoundScript bodySoundScript)){
                bodySoundScript.isNearPlayer = false;
            }
        }
    }

    public void ResetLocationRader(){
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}

