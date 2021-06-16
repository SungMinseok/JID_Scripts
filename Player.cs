using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GroundState{
    none,onGround,onPassableGround,onAscent,onRope
}
public class Player : MovingObject
{

    //기본값
    public int hp;
    public int curHp;
    [SerializeField][Range(2f,10f)]
    public float speed;
    public float runSpeed;
    [SerializeField][Range(10f,30f)]
    public float jumpPower;


    //상태값
    public GroundState groundState;
    public bool canMove;
    public bool isWalking;
    public bool isJumping;
    public bool isFalling;
    public bool isClimbingLadder;
    public bool isClimbingRope;
    public bool getRope;
    //이동
    //달리기
    //포복
    //점프
    //2단점프
    //사다리 타기
    //아이템 줍기
    int playerLayer, groundLayer, passableGroundLayer, ascentLayer;
    //키 입력
    float wInput,hInput;
    bool jumpInput, downInput;
    public bool jumpDelay, climbDelay;
    void Start(){
        rb = GetComponent<Rigidbody2D>();

        playerLayer = LayerMask.NameToLayer("Player");
        groundLayer = LayerMask.NameToLayer("Ground");
        passableGroundLayer = LayerMask.NameToLayer("PassableGround");
        ascentLayer = LayerMask.NameToLayer("Ascent");
    }
    void Update(){
        wInput = Input.GetAxisRaw("Horizontal");
        hInput = Input.GetAxisRaw("Vertical");

        // if(Input.GetButtonDown("Jump")){
        //     jumpInput = true;
        // }
        // else{
        //     jumpInput = false;
        // }
        jumpInput = Input.GetButton("Jump") ? true : false;
        downInput = Input.GetKey(KeyCode.DownArrow) ? true : false;
    }
    void FixedUpdate(){

        if(canMove){
            //이동
            if(wInput!=0){
                isWalking = true;
                Walk();
            }
            else{
                isWalking = false;
            }

            //점프
            CheckGround();
            SetGravity();
            if(!isFalling&&!isClimbingRope) SetLayer();

            if(jumpInput && !jumpDelay){
                if(downInput && !isFalling && !isJumping && groundState!=GroundState.onGround){
                    StartCoroutine(Fall());

                }
                else if(!isJumping){
                    if(isWalking) Jump();
                    else if(rb.velocity.y <= 0) Jump();
                }

            } 
            
            //줄타기
            if(hInput!=0){
                if(getRope){
                    isClimbingRope = true;
                    isJumping = false;
                    //rb.gravityScale = 0f;
                    transform.Translate(new Vector2(0,hInput) * 1.2f* Time.deltaTime);
                }
                else{
                    isClimbingRope = false;
                }
            }

            // if(isClimbingRope){
            //     rb.gravityScale = 0f;
            //     isJumping = false;
            //     if(isJumping){
            //         isClimbingRope = false;
            //     } 
                
            // }
            // else{
            //     rb.gravityScale = 10f;
            // }

            //강제 예외처리
            //if(isJumping) rb.gravityScale = 10f;
            // if(isClimbingRope && hInput !=0 && wInput != 0){
            //     hInput =0;
            //     Debug.Log("RR");
            // }

            if(!getRope) isClimbingRope = false;
        }

      //  IgnoreLayer_Walk();
    }


    void Walk(){
        rb.velocity = new Vector2(speed * wInput  , rb.velocity.y);
        //rb.AddForce(Vector2.right * wInput *speed , ForceMode2D.Impulse);
        //Vector3 wInputToVector = new Vector3(wInput,0);
         //transform.position += wInputToVector * speed * Time.deltaTime;

        // Debug.Log(rb.velocity.y);
    }


    void Jump(){
        if(isClimbingRope && wInput == 0){
            return;
        }
        
        StartCoroutine(ClimbDelay());
        //if(isClimbingRope){
            isClimbingRope = false;
        //}


        rb.velocity = Vector2.zero;
        //jumpInput = false;
        //rb.velocity = Vector2.up * jumpPower;
        rb.AddForce(Vector2.up*jumpPower,ForceMode2D.Impulse);


        StartCoroutine(JumpDelay());
        Debug.Log("Jump");
    }
    IEnumerator JumpDelay(){
        jumpDelay = true;
        yield return new WaitForSeconds(0.4f);
        
        jumpDelay = false;
    }
    IEnumerator ClimbDelay(){
        climbDelay = true;
        yield return new WaitForSeconds(0.3f);
        
        climbDelay = false;
    }
    

    IEnumerator Fall(){

        isFalling = true;
        //isJumping = true;

        if(groundState==GroundState.onPassableGround){//오르막만 통과
            isJumping = true;
            Physics2D.IgnoreLayerCollision(playerLayer, passableGroundLayer, true);//레이어 무시
           
        }
        else if(groundState==GroundState.onAscent){//패써블그라운드만 통과
            isJumping = true;
            Physics2D.IgnoreLayerCollision(playerLayer, ascentLayer, true);//레이어 무시
        }

        
        yield return new WaitUntil(()=>groundState==GroundState.none);
        yield return new WaitUntil(()=>groundState!=GroundState.none);
        isFalling = false;
        //isJumping = false;
    }

    void ClimbRope(){
    }

    void CheckGround(){        
        if(Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.4f), 0.01f, 1<< LayerMask.NameToLayer("Ground"))){
            groundState=GroundState.onGround;
        }
        else if(Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.4f), 0.1f, 1<< LayerMask.NameToLayer("Ascent"))){
            groundState=GroundState.onAscent;
        }
        else if(Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.4f), 0.01f, 1<< LayerMask.NameToLayer("PassableGround"))){
            groundState=GroundState.onPassableGround;
        }        
        else if(Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.4f), 0.01f, 1<< LayerMask.NameToLayer("Rope"))){
            groundState=GroundState.onRope;
        } 

        else{
            groundState=GroundState.none;
        }

        //Debug.Log(groundState);
    }
    void SetLayer(){
        if(groundState==GroundState.onRope){
            isJumping = false;
            Physics2D.IgnoreLayerCollision(playerLayer, ascentLayer, true);//레이어 무시
            Physics2D.IgnoreLayerCollision(playerLayer, passableGroundLayer, true);//레이어 무시
//rb.gravityScale = 0f;
        }
        else if(groundState==GroundState.onGround){//오르막만 통과
            isJumping = false;
            Physics2D.IgnoreLayerCollision(playerLayer, passableGroundLayer, true);//레이어 활성화
            Physics2D.IgnoreLayerCollision(playerLayer, ascentLayer, true);//레이어 무시
        }
        else if(groundState==GroundState.onPassableGround){//오르막만 통과
            isJumping = false;
            if(isClimbingRope){

                Physics2D.IgnoreLayerCollision(playerLayer, passableGroundLayer, true);//레이어 활성화
                Physics2D.IgnoreLayerCollision(playerLayer, ascentLayer, true);//레이어 활성화
            }
            else{

                Physics2D.IgnoreLayerCollision(playerLayer, passableGroundLayer, false);//레이어 활성화
                Physics2D.IgnoreLayerCollision(playerLayer, ascentLayer, false);//레이어 활성화
            }
        }
        else if(groundState==GroundState.onAscent){//패써블그라운드만 통과
            isJumping = false;
            Physics2D.IgnoreLayerCollision(playerLayer, passableGroundLayer, true);//레이어 무시
            Physics2D.IgnoreLayerCollision(playerLayer, ascentLayer, false);//레이어 활성화
        }

        else if(groundState==GroundState.none){//오르막만 통과
             isJumping = true;
        //     Physics2D.IgnoreLayerCollision(playerLayer, passableGroundLayer, true);//레이어 활성화
        //     Physics2D.IgnoreLayerCollision(playerLayer, ascentLayer, true);//레이어 무시
        }
    }
    void SetGravity(){
        // if(groundState==GroundState.none){
        //     rb.gravityScale = 10f;
        // }
        // else 
        if(isClimbingRope){
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
        }
        else if(!isClimbingRope){
            rb.gravityScale = 10f;
        }
    }

    void OnTriggerStay2D(Collider2D other) {
        if(other.tag == "Rope"){
            getRope = true;
            //rb.gravityScale = 0f;
            if(isClimbingRope ){
                if(groundState==GroundState.onGround && (wInput!=0 || (hInput!=0&&wInput!=0))){
                    isClimbingRope = false;
                }
                else if(!climbDelay){

                    transform.position = new Vector2(other.transform.position.x,transform.position.y);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other){
        
        if(other.tag == "Rope"){
            getRope = false;
            //rb.gravityScale = 10f;
        }
    }
}
