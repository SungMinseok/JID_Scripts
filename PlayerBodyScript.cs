using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyScript : MonoBehaviour
{
    public PlayerManager thePlayer;
    [Header ("push 애니메이션 용, 본체 애니메이터 연결")]
    public Animator animator;
    public bool cantPushSoundFlag;
    void OnCollisionStay2D(Collision2D other){
        // // foreach(ContactPoint2D contact in other.contacts){
        // //     var colName = contact.collider.name;

        // //     if(colName!="BodyCol"){
                
        //         if(other.gameObject.CompareTag("Ground")||other.gameObject.CompareTag("Box")){
        //             checkGroundCol = other.gameObject.GetComponent<Collider2D>();
        //         }
        // //     }
        // // }
        if(other.gameObject.CompareTag("Box")){
            if(animator.GetBool("run")){
                animator.SetBool("push", true);
                if(other.transform.TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody2D)){
                    if(rigidbody2D.mass >= 3000 && !cantPushSoundFlag){
                        cantPushSoundFlag = true;
                        SoundManager.instance.PlaySound("pot_touch");
                    }
                }
            }
            else{
                animator.SetBool("push", false);
                cantPushSoundFlag = false;

            }
        }


    }
    void OnCollisionExit2D(Collision2D other){
                
        if(other.gameObject.CompareTag("Box")){
            animator.SetBool("push", false);
            cantPushSoundFlag = false;
            
        }
            // if(other.gameObject.CompareTag("Ground")||other.gameObject.CompareTag("Box")){
            //     checkGroundCol = null;
            // }
    }    
    
    void OnTriggerEnter2D(Collider2D other) {

        PlayerManager.instance.onTriggerCol = other.transform;
 
    }

    void OnTriggerExit2D(Collider2D other) {

        PlayerManager.instance.onTriggerCol = null;

        if (other.CompareTag("Ladder"))
        {
            thePlayer.getLadder = false;
        }
        else if (other.CompareTag("Dirt"))
        {
            thePlayer.getDirt = false;
            thePlayer.animator.SetBool("shoveling1", false);
            thePlayer.dirtTarget = null;
            //UIManager.instance.clearPanel.SetActive(true);
        }
        else if (other.CompareTag("Icicle"))
        {
            thePlayer.getIcicle = false;
            thePlayer.animator.SetBool("icebreak", false);
            thePlayer.dirtTarget = null;
            //UIManager.instance.clearPanel.SetActive(true);
        }

        else if (other.CompareTag("Cover"))
        {
            other.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }
    void OnCollisionEnter2D(Collision2D other){
        
        // if(other.gameObject.CompareTag("NPC")){
        //     Physics2D.IgnoreCollision(other.gameObject.GetComponent<CircleCollider2D>(), circleCollider2D, true);
        //     Physics2D.IgnoreCollision(other.gameObject.GetComponent<CircleCollider2D>(), boxCollider2D, true);
        // }
        
        if(other.gameObject.CompareTag("Collider_NPC")){
            Physics2D.IgnoreCollision(other.gameObject.GetComponent<BoxCollider2D>(), thePlayer.circleCollider2D, true);
            Physics2D.IgnoreCollision(other.gameObject.GetComponent<BoxCollider2D>(), thePlayer.bodyCollider2D, true);
        }
    }
        void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            if (!thePlayer.ladderDelay) thePlayer.getLadder = true;
        }

        else if (other.CompareTag("Dirt"))
        {
            var tempGetObject = other.GetComponent<DirtScript>();
            if(tempGetObject.curHp>0) thePlayer.getDirt = true;
            thePlayer.dirtTarget = tempGetObject;
            //UIManager.instance.clearPanel.SetActive(true);
        }

        else if (other.CompareTag("Icicle"))
        {
            var tempGetObject = other.GetComponent<DirtScript>();
            if(tempGetObject.curHp>0) thePlayer.getIcicle = true;
            thePlayer.dirtTarget = tempGetObject;
            //UIManager.instance.clearPanel.SetActive(true);
        }
        else if (other.CompareTag("Cover"))
        {
            Debug.Log(other.gameObject.name);
            other.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        }


    }
}
