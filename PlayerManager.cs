using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public static PlayerManager instance;
    [Header("Set Status")]
    [SerializeField] [Range(2f, 10f)] public float speed;
    [SerializeField] [Range(10f, 50f)] public float jumpPower;
    [SerializeField] [Range(1f, 20f)] public float gravityScale;

    [Header("Input Check")]
    public float wInput;
    public float hInput;
    public bool jumpInput, downInput, interactInput;
    [Space]
    public float wSet;
    [Header("States")]
    public bool isTalking;
    public bool isActing;
    public bool canMove;
    public bool isGrounded;
    public bool isJumping;
    public bool jumpDelay;
    public bool getLadder;
    public bool ladderDelay;
    public bool onLadder;
    public bool isFalling;
    //public string[] fallExceptList;
    //public bool fallDelay;//지형 겹쳐있을 때 내려가지지 않는 현상 방지
    public bool isHiding;
    public bool isCaught;
    public Transform playerBody;
    public SpriteRenderer[] spriteRenderers;
    Color tempColor = new Color(1,1,1,1);
    Vector2 defaultScale;
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider2D;
    public CircleCollider2D circleCollider2D;
    //SpriteRenderer spriteRenderer;

    [SerializeField] LayerMask groundLayer;
    Vector2 footPos;
    float footRadius;



    public DebugManager d;
    public Animator animator;

    public Transform dialogueHolder;
    [Header("Debugging")]
    public Collider2D lastPlatform;
    public Transform onTriggerCol;//onTrigger 콜라이더
    void Awake()
    {
        Application.targetFrameRate = 60;
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

    void Start()
    {

        //rb = GetComponent<Rigidbody2D>();

        //animator = GetComponent<Animator>();
        //spriteRenderer = GetComponent<SpriteRenderer>();

        d = DebugManager.instance;
        defaultScale = playerBody.transform.localScale;

        canMove = true;

        // Physics2D.IgnoreCollision(ObjectController.instance.npcs[0].gameObject.GetComponent<CircleCollider2D>(), circleCollider2D, true);
        // Physics2D.IgnoreCollision(ObjectController.instance.npcs[0].gameObject.GetComponent<CircleCollider2D>(), boxCollider2D, true);
    }


    void Update()
    {
        if (canMove)
        {
            wSet = 0;
            wInput = Input.GetAxisRaw("Horizontal");
            hInput = Input.GetAxisRaw("Vertical");
            jumpInput = Input.GetButton("Jump") ? true : false;
            interactInput = Input.GetButton("Interact") ? true : false;





        }
        else{

            wInput = 0;
            jumpInput = false;
        }

        if (isGrounded)
        {
            isJumping = false;
            animator.SetBool("jump", false);

            if(hInput<0){
                
                animator.SetBool("down", true);
            }
            else{
                animator.SetBool("down", false);

            }

        }
        else
        {

            animator.SetBool("jump", true);
        }

//좌우 이동 중
        if (wInput != 0 || wSet != 0)   
        {
            animator.SetBool("run", true);
            if (wInput > 0|| wSet > 0)
            {
                //spriteRenderer.flipX = false;
                playerBody.localScale = new Vector2(defaultScale.x, defaultScale.y);
            }
            else if (wInput < 0|| wSet < 0)
            {
                //spriteRenderer.flipX = true;
                playerBody.localScale = new Vector2(-defaultScale.x, defaultScale.y);
            }
        }
        else
        {
            
            animator.SetBool("run", false);
            
            if(interactInput){
                //animator.SetTrigger("shovelling");
                animator.SetBool("shovelling1", true);
            }
            else{
                animator.SetBool("shovelling1", false);

            }
        }

        
    }

    void FixedUpdate()
    {

        footPos = new Vector2(transform.position.x, (circleCollider2D.bounds.min.y));
        footRadius = 0.05f;

        isGrounded = Physics2D.OverlapCircle(footPos, footRadius, groundLayer);
        //nowPlatform = Physics2D.OverlapCircle(footPos, footRadius, groundLayer);

        //if(canMove){
        if (wInput != 0)
        {
            rb.velocity = new Vector2(speed * wInput, rb.velocity.y);

        }
        else if (wSet != 0)
        {
            rb.velocity = new Vector2(speed * wSet, rb.velocity.y);

        }

        if (jumpInput && !isJumping)
        {

            if (!jumpDelay && !onLadder)
            {
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
            
            rb.gravityScale = gravityScale;
        }


    }
    void Jump(float multiple = 1)
    {
        Debug.Log(multiple);
        StartCoroutine(JumpDelay());
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * (jumpPower * multiple), ForceMode2D.Impulse);
    }
    IEnumerator JumpDelay()
    {
        if (!jumpDelay)
        {

            jumpDelay = true;
            //yield return new WaitForSeconds(0.5f);

            yield return new WaitForSeconds(0.2f);
            yield return new WaitUntil(() => isGrounded);
            jumpDelay = false;
        }
    }
    IEnumerator GetLadderDelay()
    {
        if (!ladderDelay)
        {
            Debug.Log("33");
            getLadder = false;
            ladderDelay = true;
            yield return new WaitForSeconds(0.5f);
            ladderDelay = false;
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            if (!ladderDelay) getLadder = true;
        }

        else if (other.CompareTag("Item"))
        {
            //UIManager.instance.clearPanel.SetActive(true);
        }

        else if (other.CompareTag("Cover"))
        {
            Debug.Log(other.gameObject.name);
            other.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            getLadder = false;
        }

        else if (other.CompareTag("Cover"))
        {
            other.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }
    void OnCollisionEnter2D(Collision2D other){
        
        // if(other.gameObject.CompareTag("NPC")){
        //     Physics2D.IgnoreCollision(other.gameObject.GetComponent<CircleCollider2D>(), circleCollider2D, true);
        //     Physics2D.IgnoreCollision(other.gameObject.GetComponent<CircleCollider2D>(), boxCollider2D, true);
        // }
    }
    // void OnCollisionStay2D(Collision2D other)
    // {
    // }
    // void OnCollisionExit2D(Collision2D other)
    // {

    // }

    public void Look(string direction){
        switch(direction){
            case "left" : 
                //spriteRenderer.flipX = true;
                playerBody.localScale = new Vector2(-defaultScale.x, defaultScale.y);
                break;
            case "right" : 
                //spriteRenderer.flipX = false;
                playerBody.localScale = new Vector2(defaultScale.x, defaultScale.y);
                break;
        }
    }
    public void SetAlpha(float amt){
        
        tempColor.a = amt;
        for(int i=0;i<spriteRenderers.Length;i++){
            spriteRenderers[i].color = tempColor;
        }
    }
}
