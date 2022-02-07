﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DoodadsType{
    Ladder,
    Cover,
    Trap,
    SlowDown,
    Bullet,
}
public class DoodadsScript : MonoBehaviour
{
    public DoodadsType type;
    SpriteRenderer spriteRenderer;
    //Color defaultColor = new Color(1,1,1);
    //Color darkColor = new Color(0.3f,0.3f,0.3f);
    Color color;
    WaitForSeconds waitTime = new WaitForSeconds(0.01f);
    //PlayerManager thePlayer;
    
    [Header("사다리로 올라가는 플랫폼들")]public Collider2D[] platformCollider;
    
    [Header("이속 감소")]
    public float slowDownValue;
    public bool onHit;

    void Start(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        //thePlayer = PlayerManager.instance;

        
    }

    void Cloak(float _speed = 0.03f){
        StopAllCoroutines();
        StartCoroutine(CloakCoroutine(_speed));
        //spriteRenderer.color = darkColor;
        //PlayerManager.instance.SetAlpha(0.5f);
    }
    IEnumerator CloakCoroutine(float _speed){
        //SpriteRenderer playerSr = PlayerManager.instance.GetComponent<Sp
        color = spriteRenderer.color;
        while(color.a >0.5f){
            color.r -= _speed;
            color.g -= _speed;
            color.b -= _speed;
            color.a -= _speed;
            spriteRenderer.color = color;
            
            yield return waitTime;
        }
    }    
    void Decloak(float _speed = 0.03f){
        
        StopAllCoroutines();
        StartCoroutine(DecloakCoroutine(_speed));
        //spriteRenderer.color = defaultColor;
        //PlayerManager.instance.SetAlpha(1f);
    }
    IEnumerator DecloakCoroutine(float _speed){
        color = spriteRenderer.color;
        while(color.a <1f){
            color.r += _speed;
            color.g += _speed;
            color.b += _speed;
            color.a += _speed;
            spriteRenderer.color = color;
            yield return waitTime;
        }
    }
    void OnCollisionEnter2D(Collision2D other){
        
        switch(type){
            case DoodadsType.Bullet :
                if(other.gameObject.CompareTag("Player")){
                    
                    if(!PlayerManager.instance.isGameOver){

                        PlayerManager.instance.isGameOver = true;
                        PlayerManager.instance.canMove = false;
                        PlayerManager.instance.animator.SetBool("dead0",true);
                        MinigameManager.instance.FailMinigame(4);
                        //UIManager.instance.SetGameOverUI(4);
                    }
                }
                else if(other.gameObject.CompareTag("MainGround") || other.gameObject.CompareTag("Collider_Player")){
                    gameObject.SetActive(false);
                }
                break;

            
        }
    }
    void OnCollisionStay2D(Collision2D other){

        switch(type){
            case DoodadsType.Bullet :
                if(other.gameObject.CompareTag("Collider_Player")){
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other.transform.GetComponent<Collider2D>(), true);
                }
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other){

        switch(type){
            case DoodadsType.Cover :
                if(other.CompareTag("Player")){
                    Cloak();
                    PlayerManager.instance.isHiding = true;
                }
                break;

            case DoodadsType.Trap :
                if(other.CompareTag("Player")){
                    if(!PlayerManager.instance.isGrounded && !PlayerManager.instance.isGameOver){

                        PlayerManager.instance.isGameOver = true;
                        PlayerManager.instance.canMove = false;
                        PlayerManager.instance.animator.SetBool("dead0",true);
                        UIManager.instance.SetGameOverUI(0);
                    }
                }
                break;

            case DoodadsType.SlowDown :
                if(other.CompareTag("Player")){
                    if(!onHit){
                        onHit = true;
                        PlayerManager.instance.isSlowDown += slowDownValue;
                    }
                //Debug.Log("A");
                }
                break;
            // case DoodadsType.Bullet :
            //     if(other.CompareTag("Player")){
                    
            //         if(!PlayerManager.instance.isGameOver){

            //             PlayerManager.instance.isGameOver = true;
            //             PlayerManager.instance.canMove = false;
            //             PlayerManager.instance.animator.SetBool("dead0",true);
            //             UIManager.instance.SetGameOverUI(4);
            //         }
            //     }
            //     break;

            
        }

        // if(type == DoodadsType.Cover){
            
        //     if(other.CompareTag("Player")){
        //         Cloak();
        //         PlayerManager.instance.isHiding = true;
        //     }
        // }
        // else if(type == DoodadsType.Trap){
        //     if(other.CompareTag("Player")){
        //         if(!PlayerManager.instance.isGrounded && !PlayerManager.instance.isGameOver){

        //             PlayerManager.instance.isGameOver = true;
        //             PlayerManager.instance.canMove = false;
        //             PlayerManager.instance.animator.SetBool("dead0",true);
        //             UIManager.instance.SetGameOverUI(0);
        //         }
        //     }
        // }
        // else if(type == DoodadsType.SlowDown){
        //     if(other.CompareTag("Player")){
        //         if(!onHit){
        //             onHit = true;
        //             PlayerManager.instance.isSlowDown += slowDownValue;
        //         }
        //        //Debug.Log("A");
        //     }
        // }
        
    }
    
    void OnTriggerStay2D(Collider2D other) {

        if(type == DoodadsType.Ladder){

            if(other.CompareTag("Player")){

                if(PlayerManager.instance.hInput !=0){
                    for(int i=0;i<platformCollider.Length;i++){
                            
                        Physics2D.IgnoreCollision(PlayerManager.instance.bodyCollider2D, platformCollider[i], true);
                        Physics2D.IgnoreCollision(PlayerManager.instance.circleCollider2D, platformCollider[i], true);
                        //DM(platformCollider[i].name);
                    }
                // Debug.Log("무시중");
                }
                
                if(PlayerManager.instance.onLadder){
                    other.transform.parent.position = new Vector2(transform.position.x,other.transform.parent.position.y);
                }
                else{
                    if(!PlayerManager.instance.isFalling){
                            
                        for(int i=0;i<platformCollider.Length;i++){
                                
                            Physics2D.IgnoreCollision(PlayerManager.instance.bodyCollider2D, platformCollider[i], false);
                            Physics2D.IgnoreCollision(PlayerManager.instance.circleCollider2D, platformCollider[i], false);
                        }
                    }
                }
            }
        }
        
        // else if(type == DoodadsType.Cover){
            
        //     if(other.CompareTag("Player")){

        //         PlayerManager.instance.isHiding = true;
                

        //     }
        // }
        
    }
    void OnTriggerExit2D(Collider2D other) {
        
        if(type == DoodadsType.Ladder){
            if(other.CompareTag("Player")){
                for(int i=0;i<platformCollider.Length;i++){
                        
                        Physics2D.IgnoreCollision(PlayerManager.instance.bodyCollider2D, platformCollider[i], false);
                        Physics2D.IgnoreCollision(PlayerManager.instance.circleCollider2D, platformCollider[i], false);
                }

            }
        }        
        else if(type == DoodadsType.Cover){
            
            if(other.CompareTag("Player")){

                PlayerManager.instance.isHiding = false;
                Decloak();

            }
        }
        else if(type == DoodadsType.SlowDown){
            if(other.CompareTag("Player")){
                if(onHit){
                    onHit = false;
                PlayerManager.instance.isSlowDown -= slowDownValue;
                }
//                Debug.Log("B");
            }
        }
    }
    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);
}
