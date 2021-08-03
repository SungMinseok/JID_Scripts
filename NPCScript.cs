using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCScript : MonoBehaviour
{
    
    [Header("자동 이동 모드")]
    public bool onJYD;
    public float JYDCoolDown;
    bool JYDFlag;
    [Header("정찰 모드")]
    public bool onPatrol;
    public bool patrolFlag;
    public float patrolInterval;
    public Transform startPos,desPos;
    //public float waitTime;
    public Transform rader;
    float raderFlipX;
    [Header("플레이어와 충돌 무시")]
    public bool noCollision;
    [Header("랜덤 대화 설정")]
    public bool onRandomDialogue;
    bool randomDialogueFlag;
    public float dialogueDuration;
    public int dialogueInterval;
    WaitForSeconds waitTime0, waitTime1, waitTime2;
    public Dialogue[] dialogues;
    public Coroutine randomDialogueCrt;

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

        waitTime0 = new WaitForSeconds(dialogueInterval-0.5f);
        waitTime1 = new WaitForSeconds(dialogueInterval);
        waitTime2 = new WaitForSeconds(dialogueInterval+0.5f);
        // waitTime3 = new WaitForSeconds(randialogueDuration);

        if(dialogues !=null){

            if(TextLoader.instance!=null){
                LoadText();
            }
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
            //animator.SetBool("jump", false);

        }
        else
        {

            //animator.SetBool("jump", true);
        }
        if(!noFlip){

            if (wSet != 0)   
            {
                animator.SetBool("walk", true);
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
            else{
                animator.SetBool("walk", false);

            }
        }

        if(onRandomDialogue){
            if(!randomDialogueFlag){
                randomDialogueFlag = true;
                StartCoroutine(SetRandomDialogueCoroutine());
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

    IEnumerator SetRandomDialogueCoroutine(){

        DialogueManager.instance.SetRandomDialogue_NPC(dialogues[0].sentences[Random.Range(0,dialogues[0].sentences.Length)],this.transform,dialogueDuration,dialogueInterval);
        //randomDialogueCrt = DialogueManager.instance.StartCoroutine(SetRandomDialogueCoroutine_(dialogues[0].sentences[Random.Range(0,dialogues[0].sentences.Length)],this.transform,dialogueDuration,dialogueInterval));

        int temp = Random.Range(0,3);
        if(temp==0) yield return waitTime0;
        else if(temp==1) yield return waitTime1;
        else yield return waitTime2;

        randomDialogueFlag = false;

    }

    void OnCollisionEnter2D(Collision2D other){
        
        if(other.gameObject.CompareTag("NPC")){
            Physics2D.IgnoreCollision(other.gameObject.GetComponent<CircleCollider2D>(), circleCollider2D, true);
        }
        // if(other.gameObject.CompareTag("Player")){
        //     Physics2D.IgnoreCollision(other.gameObject.GetComponent<CircleCollider2D>(), circleCollider2D, true);
        // }
    }    
    void OnTriggerStay2D(Collider2D other) {
        if(onPatrol){
            
            if((other.transform==startPos || other.transform==desPos)&& onTriggerCol!= other.transform){
                
                onTriggerCol = other.transform;
            }

            if(other.CompareTag("Player")){
                if(!PlayerManager.instance.isCaught){

                    if(PlayerManager.instance.isHiding){
                        //DM(gameObject.name+"의 레이더 내부 진입했지만 발각되지 않음");
                    }
                    else{
                        PlayerManager.instance.isCaught = true;
                        GetInRader();
                        //DM(gameObject.name+"의 레이더 내부 진입하어 발각됨");
                        animator.SetTrigger("found");
                        wSet = 0;
                    }
                }
            }

        }
        else{
                
            // if(!(other.CompareTag("MainGround")||other.CompareTag("Ground"))){
                
            //     onTriggerCol = other.transform;
            // }
 
        }

    }
    public void GetInRader(){
        for(int i=0; i<dialogues.Length; i++){
            if(dialogues[i].comment == "발견"){
                //StopCoroutine(randomDialogueCrt);
                DialogueManager.instance.StopRandomDialogue_NPC(randomDialogueCrt);
                onRandomDialogue = false;
                DialogueManager.instance.SetDialogue_NPC(dialogues[i]);
                break;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other) {

        onTriggerCol = null;

    }
 
    public void LoadText(){
        
        if(dialogues!=null){
            for(int i=0; i<dialogues.Length;i++){
                for(int j=0; j<dialogues[i].sentences.Length;j++){
                    int temp = int.Parse(dialogues[i].sentences[j]);
                    dialogues[i].sentences[j] = TextLoader.instance.ApplyText(temp);
                }
            }
        }
    }
    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);
}
