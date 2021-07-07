using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DoodadsType{
    Ladder,
    Cover,
}
public class DoodadsScript : MonoBehaviour
{
    public DoodadsType type;
    
    [Header("사다리로 올라가는 플랫폼들")]public Collider2D[] platformCollider;



    
    void OnTriggerStay2D(Collider2D other) {

        if(type == DoodadsType.Ladder){

            if(other.CompareTag("Player")){

                if(PlayerManager.instance.hInput !=0){
                    for(int i=0;i<platformCollider.Length;i++){
                            
                        Physics2D.IgnoreCollision(PlayerManager.instance.boxCollider2D, platformCollider[i], true);
                        Physics2D.IgnoreCollision(PlayerManager.instance.circleCollider2D, platformCollider[i], true);
                    }
                // Debug.Log("무시중");
                }
                
                if(PlayerManager.instance.onLadder){
                    other.transform.position = new Vector2(transform.position.x,other.transform.position.y);
                }
                else{
                    if(!PlayerManager.instance.isFalling){
                            
                        for(int i=0;i<platformCollider.Length;i++){
                                
                            Physics2D.IgnoreCollision(PlayerManager.instance.boxCollider2D, platformCollider[i], false);
                            Physics2D.IgnoreCollision(PlayerManager.instance.circleCollider2D, platformCollider[i], false);
                        }
                    }
                }
            }
        }
        else if(type == DoodadsType.Cover){
            
            if(other.CompareTag("Player")){

                PlayerManager.instance.isHiding = true;

            }
        }
        
    }
    void OnTriggerExit2D(Collider2D other) {
        
        if(type == DoodadsType.Ladder){
            if(other.CompareTag("Player")){
                for(int i=0;i<platformCollider.Length;i++){
                        
                        Physics2D.IgnoreCollision(PlayerManager.instance.boxCollider2D, platformCollider[i], false);
                        Physics2D.IgnoreCollision(PlayerManager.instance.circleCollider2D, platformCollider[i], false);
                }

            }
        }        
        else if(type == DoodadsType.Cover){
            
            if(other.CompareTag("Player")){

                PlayerManager.instance.isHiding = false;

            }
        }
    }
}
