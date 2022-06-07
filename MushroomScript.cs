using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomScript : MonoBehaviour
{
    public float actCoolTime0;//2.429
    public float actCoolTime1;//대기시간
    public string actName;
    WaitForSeconds waitCoolTime0, waitCoolTime1 ;
    WaitForSeconds mushroomCoolTime ;
    Animator animator;

    bool actFlag;
    void Start(){
        animator = transform.GetComponentInParent<Animator>();
        waitCoolTime0 = new WaitForSeconds(actCoolTime0);
        waitCoolTime1 = new WaitForSeconds(actCoolTime1);
                //Debug.Log("1");
    }
    void Update(){
        if(DBManager.instance.curData.curMapNum == 14){
            if(!actFlag && !PlayerManager.instance.isGameOver && !DBManager.instance.CheckTrigOver(60)){
                actFlag = true;
                StartCoroutine(ActCoroutine());
            }
        }
    }
    IEnumerator ActCoroutine(){
            //if(DBManager.instance.curData.curMapNum == 14){
                SoundManager.instance.PlaySound("mushroom_beforewhip");
            //}
            //mushroomCoolTime = new WaitForSeconds(10f);
            //yield return  new WaitForSeconds(10f);
            yield return waitCoolTime0;

            animator.SetTrigger(actName);
            yield return waitCoolTime1;
            actFlag= false;
            //yield return mushroomCoolTime;
            
    }

    void OnDisable(){
        StopAllCoroutines();
    }
    void OnCollisionEnter2D(Collision2D other){
        if(other.transform.CompareTag("Player")){
            if(!PlayerManager.instance.isGameOver){
                //PlayerManager.instance.isCaught = true;
                PlayerManager.instance.isGameOver = true;
                SceneController.instance.CameraView(null);
                var rb = PlayerManager.instance.GetComponent<Rigidbody2D>();
                PlayerManager.instance.bodyCollider2D.gameObject.SetActive(false);
                PlayerManager.instance.circleCollider2D.gameObject.SetActive(false);
                rb.gravityScale = 0;
                rb.AddForce(new Vector2(100000,-100000));

                PlayerManager.instance.canMove = false;
                PlayerManager.instance.animator.SetBool("dead0",true);
                
                UIManager.instance.SetGameOverUI(5);
                
            }
        }
    }
}
