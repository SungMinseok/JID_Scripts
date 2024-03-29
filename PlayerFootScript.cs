﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootScript : MonoBehaviour
{
    public PlayerManager thePlayer;
    public bool canJumpFromLadderFlag;
    // [Header ("push 애니메이션 용, 본체 애니메이터 연결")]
    // public Animator animator;
    // void OnCollisionStay2D(Collision2D other){
    //     // // foreach(ContactPoint2D contact in other.contacts){
    //     // //     var colName = contact.collider.name;

    //     // //     if(colName!="BodyCol"){
                
    //     //         if(other.gameObject.CompareTag("Ground")||other.gameObject.CompareTag("Box")){
    //     //             checkGroundCol = other.gameObject.GetComponent<Collider2D>();
    //     //         }
    //     // //     }
    //     // // }
    //     if(other.gameObject.CompareTag("Box")){
    //         if(animator.GetBool("run")){
    //             animator.SetBool("push", true);
    //         }
    //         else{
    //             animator.SetBool("push", false);

    //         }
    //     }


    // }
    // void OnCollisionExit2D(Collision2D other){
                
    //     if(other.gameObject.CompareTag("Box")){
    //         animator.SetBool("push", false);
            
    //     }
    //         // if(other.gameObject.CompareTag("Ground")||other.gameObject.CompareTag("Box")){
    //         //     checkGroundCol = null;
    //         // }
    // }    
    
    void OnTriggerEnter2D(Collider2D other) {

        PlayerManager.instance.onTriggerCol = other.transform;
 
    }

    void OnCollisionEnter2D(Collision2D other){
        
        if(other.gameObject.CompareTag("Collider_NPC")){
            Physics2D.IgnoreCollision(other.gameObject.GetComponent<BoxCollider2D>(), thePlayer.circleCollider2D, true);
            Physics2D.IgnoreCollision(other.gameObject.GetComponent<BoxCollider2D>(), thePlayer.bodyCollider2D, true);
        }
    }
    void OnCollisionStay2D(Collision2D other){
        //if (!other.gameObject.CompareTag("MainGround"))
            PlayerManager.instance.footCol = other.transform.GetComponent<Collider2D>();

    }
    void OnCollisionExit2D(Collision2D other){
        PlayerManager.instance.footCol = null;
    }
    void OnTriggerStay2D(Collider2D other){
        
        if (other.CompareTag("Ladder"))
        {
            if (!thePlayer.ladderDelay) thePlayer.getLadder = true;
            if(!canJumpFromLadderFlag && !thePlayer.canJumpFromLadder){
                canJumpFromLadderFlag = true;

                //점프 불가인 사다리일 경우, 점프 불가(ex. 맵 이동+내려가는 사다리)
                if(!other.GetComponent<DoodadsScript>().isUnableToJump)
                    Invoke("SetCanJumpFromLadder",0.3f);
            }
        }
        else if (other.CompareTag("OrderDestination"))
        {
            thePlayer.orderDestinationCol = other.transform;
        }
    }
    void SetCanJumpFromLadder(){
        thePlayer.canJumpFromLadder = true;
    }

    void OnTriggerExit2D(Collider2D other) {

        PlayerManager.instance.onTriggerCol = null;

        if (other.CompareTag("Ladder"))
        {
            thePlayer.getLadder = false;
            canJumpFromLadderFlag = false;
            thePlayer.canJumpFromLadder = false;
        }
        else if (other.CompareTag("OrderDestination"))
        {
            thePlayer.orderDestinationCol = null;
        }
        // else if (other.CompareTag("Dirt"))
        // {
        //     thePlayer.getDirt = false;
        //     //UIManager.instance.clearPanel.SetActive(true);
        // }

        // else if (other.CompareTag("Cover"))
        // {
        //     other.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        // }
    }
    // void OnCollisionEnter2D(Collision2D other){
        
    //     // if(other.gameObject.CompareTag("NPC")){
    //     //     Physics2D.IgnoreCollision(other.gameObject.GetComponent<CircleCollider2D>(), circleCollider2D, true);
    //     //     Physics2D.IgnoreCollision(other.gameObject.GetComponent<CircleCollider2D>(), boxCollider2D, true);
    //     // }
        
    //     if(other.gameObject.CompareTag("Collider_NPC")){
    //         Physics2D.IgnoreCollision(other.gameObject.GetComponent<BoxCollider2D>(), thePlayer.circleCollider2D, true);
    //         Physics2D.IgnoreCollision(other.gameObject.GetComponent<BoxCollider2D>(), thePlayer.boxCollider2D, true);
    //     }
    // }
    //     void OnTriggerStay2D(Collider2D other)
    // {
    //     if (other.CompareTag("Ladder"))
    //     {
    //         if (!thePlayer.ladderDelay) thePlayer.getLadder = true;
    //     }

    //     else if (other.CompareTag("Dirt"))
    //     {
    //         thePlayer.getDirt = true;
    //         thePlayer.dirtTarget = other.GetComponent<DirtScript>();
    //         //UIManager.instance.clearPanel.SetActive(true);
    //     }

    //     else if (other.CompareTag("Cover"))
    //     {
    //         Debug.Log(other.gameObject.name);
    //         other.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
    //     }

    // }
}
