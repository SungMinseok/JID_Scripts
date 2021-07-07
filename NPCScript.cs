using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    [Header("자동 이동 모드")]
    public bool onJYD;
    public float JYDCoolDown;
    bool JYDFlag;
    [Header("정찰 모드")]
    public bool onPatrol;
    public Transform startPos,desPos;
    //public float waitTime;
    public Transform rader;
    float raderFlipX;
    [Header("플레이어와 충돌 무시")]
    public bool noCollision;
    [Header("flip 사용 안함")]
    public bool noFlip;
    
    public int patrolInput;
    public Collider2D lastPlatform;
    public bool isGrounded;
    public bool isJumping;
    public bool jumpDelay;
    [SerializeField][Range(2f,10f)] public float speed;
    [SerializeField][Range(10f,50f)] public float jumpPower;
    public Transform onTriggerCol;

    Rigidbody2D rb;
    CircleCollider2D circleCollider2D;
    SpriteRenderer spriteRenderer;
    public float wSet, hInput;
    public bool jumpInput, downInput;
    PlayerManager thePlayer;
    public Animator animator;
    float defaultScaleX;
    void Start()
    {
        thePlayer = PlayerManager.instance;
        rb = GetComponent<Rigidbody2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        defaultScaleX = transform.localScale.x;


        if(noCollision){
                
            Physics2D.IgnoreCollision(thePlayer.boxCollider2D, circleCollider2D, true);
            Physics2D.IgnoreCollision(thePlayer.circleCollider2D, circleCollider2D, true);
        }
        
        if(rader!=null){
            raderFlipX = rader.transform.localScale.x;
        }
    }

    void Update(){

        if(onJYD){
            wSet = patrolInput;
            if(!JYDFlag){
                StartCoroutine(SetJYD());
            }
        } 
        else if(onPatrol){
            
        }

        //jumpInput = Input.GetButton("Jump") ? true : false;
        //downInput = Input.GetKey(KeyCode.DownArrow) ? true : false;

        //checkGroundCol = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.4f), 0.01f);

        if (isGrounded)
        {
            isJumping = false;
            animator.SetBool("jump", false);

        }
        else
        {

            animator.SetBool("jump", true);
        }
        if(!noFlip){

            if(wSet>0){
                spriteRenderer.flipX = false;
                if(rader!=null){
                    rader.transform.localScale = new Vector2(raderFlipX , rader.transform.localScale.y);
                }
                //transform.localScale = new Vector2(defaultScaleX, transform.localScale.y);
            }
            else if(wSet<0){
                spriteRenderer.flipX = true;
                if(rader!=null){
                    rader.transform.localScale = new Vector2(raderFlipX * -1 , rader.transform.localScale.y);
                }
                //transform.localScale = new Vector2(defaultScaleX * -1, transform.localScale.y);
            }
        }
    }

    void FixedUpdate(){
        if(wSet!=0){
            rb.velocity = new Vector2(speed * wSet  , rb.velocity.y);

        }

        if(jumpInput){
            if(!isJumping && !jumpDelay){
                Jump();
            }
        }

        
    }

    IEnumerator SetJYD(){
        //if(!patrolFlag){
            JYDFlag = true;  
            patrolInput = Random.Range(0,3) - 1;//0 좌 1 정지 2 우
            yield return new WaitForSeconds(JYDCoolDown);
            JYDFlag = false;  

        //}
    }




    void Jump(float multiple = 1){
        Debug.Log(multiple);
        StartCoroutine(JumpDelay());
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up*(jumpPower*multiple),ForceMode2D.Impulse);
    }
    IEnumerator JumpDelay(){
        if(!jumpDelay){
                
            jumpDelay = true;
            //yield return new WaitForSeconds(0.5f);
            
            yield return new WaitForSeconds(0.2f);
            yield return new WaitUntil(() => isGrounded);
            jumpDelay = false;
        }
    }

    void OnCollisionEnter2D(Collision2D other){
        
        if(other.gameObject.CompareTag("NPC")){
            Physics2D.IgnoreCollision(other.gameObject.GetComponent<CircleCollider2D>(), circleCollider2D, true);
        }
        // if(other.gameObject.CompareTag("Player")){
        //     Physics2D.IgnoreCollision(other.gameObject.GetComponent<CircleCollider2D>(), circleCollider2D, true);
        // }
    }    
    void OnTriggerEnter2D(Collider2D other) {
        if(onPatrol){
            
            if(other.transform==startPos || other.transform==desPos){
                
                onTriggerCol = other.transform;
            }

            if(other.CompareTag("Player")){
                if(PlayerManager.instance.isHiding){
                    DM(gameObject.name+"의 레이더 내부 진입했지만 발각되지 않음");

                }
                else{
                    
                    DM(gameObject.name+"의 레이더 내부 진입하어 발각됨");
                    animator.SetTrigger("found");
                    wSet = 0;
                    
                }
            }

        }
        else{
                
            if(!(other.CompareTag("MainGround")||other.CompareTag("Ground"))){
                
                onTriggerCol = other.transform;
            }
 
        }

    }

    void OnTriggerExit2D(Collider2D other) {

        onTriggerCol = null;

    }
 
    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);
}
