using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    //public bool onPatrol;
    [Header("자동 이동")]
    public float patrolCoolDown;
    bool patrolFlag;
    public int patrolInput;
    public Collider2D lastPlatform;
    public bool isGrounded;
    public bool isJumping;
    public bool jumpDelay;
    [SerializeField][Range(2f,10f)] public float speed;
    [SerializeField][Range(10f,50f)] public float jumpPower;

    Rigidbody2D rb;
    CircleCollider2D circleCollider2D;
    SpriteRenderer spriteRenderer;
    public float wInput, hInput;
    public bool jumpInput, downInput;
    PlayerManager thePlayer;
    public Animator animator;
    void Start()
    {
        thePlayer = PlayerManager.instance;
        rb = GetComponent<Rigidbody2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();


        
        Physics2D.IgnoreCollision(thePlayer.GetComponent<BoxCollider2D>(), circleCollider2D, true);
        Physics2D.IgnoreCollision(thePlayer.GetComponent<CircleCollider2D>(), circleCollider2D, true);
        
    }

    void Update(){

        if(!patrolFlag) StartCoroutine(SetPatrol());

        wInput = patrolInput;
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

        if(wInput>0){
            spriteRenderer.flipX = false;
        }
        else if(wInput<0){
            spriteRenderer.flipX = true;

        }
    }

    void FixedUpdate(){
        if(wInput!=0){
            rb.velocity = new Vector2(speed * wInput  , rb.velocity.y);

        }

        if(jumpInput){
            if(!isJumping && !jumpDelay){
                Jump();
            }

            

        }

        
    }

    IEnumerator SetPatrol(){
        //if(!patrolFlag){
            patrolFlag = true;  
            patrolInput = Random.Range(0,3) - 1;//0 좌 1 정지 2 우
            yield return new WaitForSeconds(patrolCoolDown);
            patrolFlag = false;  

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
    }
 
}
