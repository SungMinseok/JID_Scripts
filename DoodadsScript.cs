using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DoodadsType{
    Ladder,
    Cover,
    Trap,
    SlowDown,
    Bullet,
    Mushroom,
    Water,
    Bullet1, //나는 개미가 아래로 던짐
    Bullet2 //하늘에서 떨어짐
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
    public Vector2 curPos;
    public BoxCollider2D boxCol0;
    [Header("사다리로 올라가는 플랫폼들")]public Collider2D[] platformCollider;
    public bool isRope;
    
    [Header("이속 감소")]
    public float slowDownValue;
    public bool onHit;

    [Header("머시룸")]
    public int mushroomNum;
    void Start(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        //thePlayer = PlayerManager.instance;

        if(type == DoodadsType.Trap){
            
            Physics2D.IgnoreCollision(PlayerManager.instance.bodyCollider2D, boxCol0, true);
            Physics2D.IgnoreCollision(PlayerManager.instance.circleCollider2D, boxCol0, true);
           //Physics2D.IgnoreCollision(boxCol0, PlayerManager.instance.bodyCollider2D, true);
        }
    }
    void OnEnable(){
        if(type == DoodadsType.Bullet2){
            //transform.localPosition = curPos;
            Physics2D.IgnoreCollision(this.GetComponent<BoxCollider2D>(), Minigame2Script.instance.startCol, true);
            Physics2D.IgnoreCollision(this.GetComponent<BoxCollider2D>(), Minigame2Script.instance.endCol, true);

        }
            
        else if(type == DoodadsType.Bullet){
            
            Physics2D.IgnoreCollision(this.GetComponent<BoxCollider2D>(), Minigame2Script.instance.startCol, true);
            Physics2D.IgnoreCollision(this.GetComponent<BoxCollider2D>(), Minigame2Script.instance.endCol, true);
        }
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
                    
                    if(Minigame2Script.instance!=null){
                        Minigame2Script.instance.GetDamage();
                        
                    }

                    // if(!PlayerManager.instance.isGameOver){
                    //     SoundManager.instance.PlaySound("minigame_bottlecrash");
                    //     PlayerManager.instance.KillPlayer(4, "dead0");

                    // }
                }
                // else if(/*other.gameObject.CompareTag("MainGround") || */other.gameObject.CompareTag("Collider_Player")){
                //     gameObject.SetActive(false);
                // }
                break;

            case DoodadsType.Bullet1 :
                if(other.gameObject.CompareTag("Player")){
                    
                    if(Minigame2Script.instance!=null){
                        Minigame2Script.instance.GetDamage();
                        
                    }
                    // if(!PlayerManager.instance.isGameOver){
                    //     SoundManager.instance.PlaySound("minigame_bottlecrash");
                    //     PlayerManager.instance.KillPlayer(4, "dead0");

                    // }
                }
                else if(other.gameObject.CompareTag("MainGround") || other.gameObject.CompareTag("Collider_Player")){
                        SoundManager.instance.PlaySound("minigame_bottlecrash");
                    gameObject.SetActive(false);
                }
                break;
            
            case DoodadsType.Bullet2 :
                if(other.gameObject.CompareTag("Player")){
                    
                    if(Minigame2Script.instance!=null){
                        Minigame2Script.instance.GetDamage();
                        
                    }
                    // if(!PlayerManager.instance.isGameOver){
                    //     SoundManager.instance.PlaySound("minigame_bottlecrash");
                    //     PlayerManager.instance.KillPlayer(4, "dead0");

                    // }
                }                
                else if(other.gameObject.CompareTag("MainGround")){
                        SoundManager.instance.PlaySound("minigame_bottlecrash");
                    gameObject.SetActive(false);
                }
                break;
        }
    }
    void OnCollisionStay2D(Collision2D other){

        switch(type){
            case DoodadsType.Bullet :
            case DoodadsType.Bullet2 :
                if(other.gameObject.CompareTag("Collider_Player")){
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other.transform.GetComponent<Collider2D>(), true);
                    Debug.Log("3413414");
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
                    //if(!PlayerManager.instance.isGrounded && !PlayerManager.instance.isGameOver){
                    if(PlayerManager.instance.isFalling && !PlayerManager.instance.isGameOver){

                        PlayerManager.instance.KillPlayer(0, "dead0");
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
                
            case DoodadsType.Mushroom :
                if(other.CompareTag("Player")){
                    if(PlayerManager.instance.isFalling ){
                        if(mushroomNum != 4)
                            MushroomGameScript.instance.PushPiano(mushroomNum);
                        else{
                            MushroomGameScript.instance.PushMain();

                        }
                    }
                }
                break;

            case DoodadsType.Water :
                if(other.CompareTag("Player")){
                    //if(!PlayerManager.instance.isGrounded && !PlayerManager.instance.isGameOver){
                    //if(PlayerManager.instance.isFalling){
                    PlayerManager.instance.KillPlayer(0, "drowning");
                    //}
                }
                break;
            case DoodadsType.Ladder :
                if(other.CompareTag("Player")){
                    //if(!PlayerManager.instance.isGrounded && !PlayerManager.instance.isGameOver){
                    //if(PlayerManager.instance.isFalling){
                    if(isRope)
                        PlayerManager.instance.onRope = true;
                    else
                        PlayerManager.instance.onRope = false;
                    //}
                }
                break;

            default : 
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
                    //if(!PlayerManager.instance.isFalling){
                            
                        for(int i=0;i<platformCollider.Length;i++){
                                
                            Physics2D.IgnoreCollision(PlayerManager.instance.bodyCollider2D, platformCollider[i], false);
                            Physics2D.IgnoreCollision(PlayerManager.instance.circleCollider2D, platformCollider[i], false);
                        }
                    //}
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
        
        switch(type){
            case DoodadsType.Ladder :
                if(other.CompareTag("Player")){
                    for(int i=0;i<platformCollider.Length;i++){
                            
                            Physics2D.IgnoreCollision(PlayerManager.instance.bodyCollider2D, platformCollider[i], false);
                            Physics2D.IgnoreCollision(PlayerManager.instance.circleCollider2D, platformCollider[i], false);
                    }

                    PlayerManager.instance.onRope = false;
                } 
                break;
            case DoodadsType.Cover :
                if(other.CompareTag("Player")){

                    PlayerManager.instance.isHiding = false;
                    Decloak();

                }
                break;
            case DoodadsType.SlowDown :
                if(other.CompareTag("Player")){
                    if(onHit){
                        onHit = false;
                    PlayerManager.instance.isSlowDown -= slowDownValue;
                    }
    //                Debug.Log("B");
                }
                break;
        }
    }
    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);
}
