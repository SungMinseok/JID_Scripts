using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    //public bool onPatrol;
    public float patrolCoolDown;
    public bool patrolFlag;
    public int patrolInput;
    public Collider2D lastPlatform;
    public Collider2D nowPlatform;
    public Collider2D checkGroundCol;
    public bool isJumping;
    public bool jumpDelay;
    [SerializeField][Range(2f,10f)] public float speed;
    [SerializeField][Range(10f,50f)] public float jumpPower;

    Rigidbody2D rb;
    //BoxCollider2D boxCollider2D;
    CircleCollider2D circleCollider2D;
    SpriteRenderer spriteRenderer;
    public float wInput, hInput;
    public bool jumpInput, downInput;
    PlayerManager thePlayer;
    void Awake()
    {
    }

    void Start()
    {

        thePlayer = PlayerManager.instance;
        rb = GetComponent<Rigidbody2D>();
        //boxCollider2D = GetComponent<BoxCollider2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();


        
        Physics2D.IgnoreCollision(thePlayer.GetComponent<BoxCollider2D>(), circleCollider2D, true);
        Physics2D.IgnoreCollision(thePlayer.GetComponent<CircleCollider2D>(), circleCollider2D, true);
        
    }

    void Update(){

        if(!patrolFlag) StartCoroutine(SetPatrol());

        wInput = patrolInput;
        //jumpInput = Input.GetButton("Jump") ? true : false;
        //downInput = Input.GetKey(KeyCode.DownArrow) ? true : false;

        checkGroundCol = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.4f), 0.01f);

        if(checkGroundCol!=null && checkGroundCol.CompareTag("Ground")){
            isJumping = false;
            
            if(nowPlatform!=checkGroundCol){
                lastPlatform = nowPlatform;
                nowPlatform = checkGroundCol;
            }
        }
        else{
            isJumping = true;
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


        // if(getLadder){
        //     if(hInput!=0){
        //         onLadder = true;
        //     }
        // }
        // else{
        //     onLadder = false;
        // }

        // if(onLadder){
            
        //     if(jumpInput && wInput!=0){
        //         onLadder = false;
        //         StartCoroutine(GetLadderDelay());
        //         if(!jumpDelay) Jump(0.7f);
        //     }
        //     else{   
        //         isJumping = false;
        //         rb.gravityScale = 0f;
        //         rb.velocity = new Vector2(0, (speed*0.7f) * hInput  );
        //     }
        // }
        // else{
            
        //     rb.gravityScale = 10f;
        // }
        
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
            yield return new WaitUntil(()=> checkGroundCol!=null);
            jumpDelay = false;
        }
    }
    // IEnumerator GetLadderDelay(){
    //     if(!ladderDelay){  
    //         Debug.Log("33");
    //         getLadder = false;
    //         ladderDelay = true;
    //         yield return new WaitForSeconds(0.5f);
    //         ladderDelay = false;
    //     }
    // }
//     IEnumerator Fall(){
//         if(!isFalling){
//             isFalling = true;
//             Jump(0.35f);
//             Collider2D temp = nowPlatform;
//             nowPlatform.GetComponent<Collider2D>().enabled = false;
//             //Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, true);//레이어 활성화
//             //yield return new WaitUntil(()=> !Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.4f), 0.01f, 1<< LayerMask.NameToLayer("Ground")));
// Debug.Log("RB");
//             yield return new WaitUntil(()=> nowPlatform!=temp);
//             temp.GetComponent<Collider2D>().enabled = true;
//             //Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);//레이어 활성화
//             isFalling = false;
//         }
//     }
    // void OnTriggerEnter2D(Collider2D other) {
    //     if(other.CompareTag("NPC")){
    //         Debug.Log("ㅇㅇ");
    //         Physics2D.IgnoreCollision(other.GetComponent<CircleCollider2D>(), circleCollider2D, true);
    //     }
    // }

    // void OnTriggerExit2D(Collider2D other) {
    //     if(other.CompareTag("Ladder")){
    //         getLadder = false; 
    //         //rb.gravityScale = 10f;
    //     }
    // }

    void OnCollisionEnter2D(Collision2D other){
        
        if(other.gameObject.CompareTag("NPC")){
            Physics2D.IgnoreCollision(other.gameObject.GetComponent<CircleCollider2D>(), circleCollider2D, true);
        }
    }
    // // void OnCollisionExit2D(Collision2D other){
        
    //     if(other.gameObject.CompareTag("Ground")){
    //         nowPlatform = null;
    //         //Debug.Log(other.gameObject.name);
    //     }
    // }
}
