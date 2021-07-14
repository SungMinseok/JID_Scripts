using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyScript : MonoBehaviour
{
    [Header ("push 애니메이션 용, 본체 애니메이터 연결")]
    public Animator animator;
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
            }
            else{
                animator.SetBool("push", false);

            }
        }


    }
    void OnCollisionExit2D(Collision2D other){
                
        if(other.gameObject.CompareTag("Box")){
            animator.SetBool("push", false);
            
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

    }
}
