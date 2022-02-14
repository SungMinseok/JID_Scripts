using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomScript : MonoBehaviour
{
    public float actCoolTime = 3f;
    public string actName;
    WaitForSeconds waitCoolTime ;
    Animator animator;
    void Start(){
        animator = transform.GetComponentInParent<Animator>();
        waitCoolTime = new WaitForSeconds(actCoolTime);
        StartCoroutine(ActCoroutine());
    }
    IEnumerator ActCoroutine(){
        while(true){

            yield return waitCoolTime;
            animator.SetTrigger(actName);
        }
    }

    void OnDisable(){
        StopAllCoroutines();
    }
    void OnTriggerStay2D(Collider2D other){
        if(other.CompareTag("Player")){
            if(!PlayerManager.instance.isGameOver){
                //PlayerManager.instance.isCaught = true;
                PlayerManager.instance.isGameOver = true;
                PlayerManager.instance.canMove = false;
                PlayerManager.instance.animator.SetBool("dead0",true);
                
                UIManager.instance.SetGameOverUI(1);
                
            }
        }
    }
}
