using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public static PlayerManager instance;
    public Collider2D lastPlatform;
    public Collider2D nowPlatform;
    public Collider2D checkGroundCol;
    public bool isGrounded;
    public bool isJumping;
    public bool jumpDelay;
    public bool getLadder;
    public bool ladderDelay;
    public bool onLadder;
    public bool isFalling;
    public string[] fallExceptList;
    public bool fallDelay;//지형 겹쳐있을 때 내려가지지 않는 현상 방지
    public bool isHiding;
    [SerializeField][Range(2f,10f)] public float speed;
    [SerializeField][Range(10f,50f)] public float jumpPower;

    Rigidbody2D rb;
    [SerializeField] BoxCollider2D boxCollider2D;
    [SerializeField]CircleCollider2D circleCollider2D;
    SpriteRenderer spriteRenderer;
    public float wInput, hInput;
    public bool jumpInput, downInput;

    [SerializeField] LayerMask groundLayer;
    Vector2 footPos;
    float footRadius;


    
    public DebugManager d;
    Animator animator;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();

        //boxCollider2D = GetComponent<BoxCollider2D>();
        //circleCollider2D = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        d= DebugManager.instance;

    }


    void Update(){
        wInput = Input.GetAxisRaw("Horizontal");
        hInput = Input.GetAxisRaw("Vertical");
        jumpInput = Input.GetButton("Jump") ? true : false;
        // downInput = Input.GetKey(KeyCode.DownArrow) ? true : false;

        //checkGroundCol = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.4f), 0.01f);
        //if(checkGroundCol!=null && (checkGroundCol.CompareTag("MainGround")||checkGroundCol.CompareTag("Ground")||checkGroundCol.CompareTag("Box"))){
        if(isGrounded){
            isJumping = false;
            animator.SetBool("jump", false);
            
            if(nowPlatform!=checkGroundCol){
                lastPlatform = nowPlatform;
                nowPlatform = checkGroundCol;
            }
        }
        else{

            animator.SetBool("jump", true);
        }
        // else{
        //     if(!onLadder) isJumping = true;
        // }
        if(wInput!=0){
            animator.SetBool("run", true);
            if(wInput>0){
                spriteRenderer.flipX = false;
            }
            else if(wInput<0){
                spriteRenderer.flipX = true;
            }
        }
        else{
            animator.SetBool("run",false);
        }
    }

    
    // bool CheckGroundCol(string exceptionName){       //true : 실행허용  false : 실행거부
    //     Debug.Log("Checkgroundcol 실행");
    //     switch(exceptionName){
    //         case "FallExcept" :         
    //             if(checkGroundCol!=null){
    //                 for(int i=0; i<fallExceptList.Length;i++){
    //                     if(checkGroundCol.CompareTag(fallExceptList[i])){
    //                         //Debug.Log("아래로 뛸 수 없음");
    //                         d.PrintDebug("아래로 뛸 수 없음");
    //                         return false;
    //                     }
    //                 }
    //             }
    //             break;

    //     }

    //     return true;
    // }
    void FixedUpdate(){

        footPos = new Vector2(transform.position.x,(circleCollider2D.bounds.min.y));
        footRadius = 0.05f;

        isGrounded = Physics2D.OverlapCircle(footPos, footRadius, groundLayer);
        nowPlatform = Physics2D.OverlapCircle(footPos, footRadius, groundLayer);


        if(wInput!=0){
            rb.velocity = new Vector2(speed * wInput  , rb.velocity.y);

        }

        if(jumpInput && !isJumping){
                
            // if(downInput  && CheckGroundCol("FallExcept")){
            //     if(!isFalling && !isJumping){
            //         StartCoroutine(Fall());
            //     }
            // }
            // else 
            if(!jumpDelay && !onLadder){
                Jump();
            }

            

        }


        if(getLadder){
            if(hInput!=0){
                onLadder = true;
            }
        }
        else{
            onLadder = false;
        }

        if(onLadder){
            
            if(jumpInput && wInput!=0){
                onLadder = false;
                StartCoroutine(GetLadderDelay());
                if(!jumpDelay) Jump(0.7f);
            }
            else{   
                isJumping = false;
                rb.gravityScale = 0f;
                rb.velocity = new Vector2(0, (speed*0.7f) * hInput  );
            }
        }
        else{
            
            rb.gravityScale = 10f;
        }
        
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
            yield return new WaitUntil(()=> isGrounded);
            jumpDelay = false;
        }
    }
    IEnumerator GetLadderDelay(){
        if(!ladderDelay){  
            Debug.Log("33");
            getLadder = false;
            ladderDelay = true;
            yield return new WaitForSeconds(0.5f);
            ladderDelay = false;
        }
    }
    IEnumerator Fall(){
        if(!isFalling){
            isFalling = true;
            StartCoroutine(FallDelay());
            Collider2D temp = nowPlatform;
            
            //nowPlatform.GetComponent<Collider2D>().enabled = false;
            //Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, true);//레이어 활성화
            
            Physics2D.IgnoreCollision(boxCollider2D, temp, true);
            Physics2D.IgnoreCollision(circleCollider2D, temp, true);
            Jump(0.35f);
            //yield return new WaitUntil(()=> !Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.4f), 0.01f, 1<< LayerMask.NameToLayer("Ground")));
//Debug.Log("RB");
            yield return new WaitUntil(()=> nowPlatform!=temp);
            //temp.GetComponent<Collider2D>().enabled = true;
            Physics2D.IgnoreCollision(boxCollider2D, temp, false);
            Physics2D.IgnoreCollision(circleCollider2D, temp, false);
            //Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);//레이어 활성화
            isFalling = false;
        }
    }
    IEnumerator FallDelay(){
        if(!fallDelay){
                
            fallDelay = true;
            yield return new WaitForSeconds(0.2f);
            //yield return new WaitUntil(()=> checkGroundCol!=null);
            fallDelay = false;
        }
    }
    void OnTriggerStay2D(Collider2D other) {
        if(other.CompareTag("Ladder")){
            if(!ladderDelay) getLadder = true;
        }
        
        if(other.CompareTag("Item")){
            UIManager.instance.clearPanel.SetActive(true);
        }
        
        if(other.CompareTag("Cover")){
            Debug.Log(other.gameObject.name);
            //var coverColor = other.gameObject.GetComponent<SpriteRenderer>().color;
            //coverColor = new Color(coverColor.r,coverColor.g,coverColor.b,coverColor.a*0.5f);
            other.gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0.5f);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Ladder")){
            getLadder = false; 
            //rb.gravityScale = 10f;
        }
        
        if(other.CompareTag("Cover")){
            //Color coverColor = other.GetComponent<SpriteRenderer>().color;
            //other.GetComponent<SpriteRenderer>().color = new Color(1,coverColor.g,coverColor.b,1);
            
            other.gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
        }
    }


    void OnCollisionStay2D(Collision2D other){
        // // foreach(ContactPoint2D contact in other.contacts){
        // //     var colName = contact.collider.name;

        // //     if(colName!="BodyCol"){
                
        //         if(other.gameObject.CompareTag("Ground")||other.gameObject.CompareTag("Box")){
        //             checkGroundCol = other.gameObject.GetComponent<Collider2D>();
        //         }
        // //     }
        // // }

    }
    void OnCollisionExit2D(Collision2D other){
                
            // if(other.gameObject.CompareTag("Ground")||other.gameObject.CompareTag("Box")){
            //     checkGroundCol = null;
            // }
    }
}
