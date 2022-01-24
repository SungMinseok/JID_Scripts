using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (CircleCollider2D))]
public class NPCScript : MonoBehaviour
{
    
    [Header("Set Animator")]
    public bool haveWalk = false;
    public bool haveTalk = false;
    [Header("Setting")]
    //public bool canMove;
    //public bool canTalk;
    [Tooltip("스프라이트 스킨아닐 시 체크")]
    public bool isSpriteRenderer;
    [Header("Sub things")]
    public Transform talkCanvas;
    public Transform interactiveMark;
    public Transform mainBody;
    
    [Header("Status")]
    [SerializeField][Range(1f,10f)] public float speed = 2f;
    
    [Header("배회 모드")]
    public bool onJYD;
    public float JYDCoolDown;
    [Tooltip("체크 시 멈추는 경우 제외")]
    public bool isNonStop;
    public bool isPaused;
    bool JYDFlag;
    public bool lookPlayer;
    [Header("감시 모드")]
    public bool onPatrol;
    public bool patrolFlag;
    public float patrolInterval;
    public Transform startPos,desPos;
    public Transform rader;
    float raderFlipX;
    [Header("미친 개미 모드")]
    public bool onMadAnt;
    public GameObject[] bullets;
    public Vector2 bulletsPos;
    [Header("플레이어와 충돌 무시")]
    public bool noCollision = true;
    [Header("랜덤 대화 설정")]
    //아무때나 랜덤 대화 활성화
    public bool alwaysRandomDialogue;
    //플레이어 근처에서만 랜덤 대화 활성화
    //public bool onlyNearPlayerRandomDialogue;
    //public bool activateRandomDialogue;
    bool randomDialogueFlag;
    public float dialogueDuration;
    public int dialogueInterval;
    WaitForSeconds waitTime0, waitTime1, waitTime2;
    public Dialogue[] dialogues;
    public Coroutine randomDialogueCrt;

    [Header("flip 사용 안함")]
    public bool noFlip;
    [Space]
    [Header("Debug ───────────────────")]
    public bool onRandomDialogue;
    public bool pauseRandomDialogue;
    
    public int patrolInput;
    public Collider2D lastPlatform;
    public bool isGrounded;
    public bool isJumping;
    public bool jumpDelay;
    public Transform onTriggerCol;
    Vector2 defaultScale;

    Rigidbody2D rb;
    CircleCollider2D circleCollider2D;
    public SpriteRenderer spriteRenderer;
    public float wSet, hInput;
    public bool jumpInput, downInput;
    PlayerManager thePlayer;
    public Animator animator;
    float defaultScaleX;
    float defaultTalkCanvasHolderPosX;
    void Start()
    {
        if(haveTalk) talkCanvas = transform.GetChild(0).GetChild(0);
        if((haveWalk || !isSpriteRenderer)&&transform.childCount>=2) mainBody = transform.GetChild(1);

        thePlayer = PlayerManager.instance;
        rb = GetComponent<Rigidbody2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        //if(mainBody!=null) spriteRenderer = mainBody.GetComponent<SpriteRenderer>();
        if(isSpriteRenderer) spriteRenderer = mainBody.GetComponent<SpriteRenderer>();
        //else spriteRenderer = GetComponent<SpriteRenderer>();
        if(mainBody!=null) animator = mainBody.GetComponent<Animator>();
        //if(isSpriteSkin) animator = mainBody.GetComponent<Animator>();
        else animator = GetComponent<Animator>();
        defaultTalkCanvasHolderPosX = talkCanvas.GetComponent<RectTransform>().localPosition.x;

        if(noCollision){
                
            Physics2D.IgnoreCollision(thePlayer.bodyCollider2D, circleCollider2D, true);
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

            if(CSVReader.instance!=null){
                LoadText();
            }
        }
        //기본세팅
        gameObject.tag = "NPC";

        //RigidBody2D 기본 세팅
        if(rb!=null){

            rb.drag = 10;
            rb.gravityScale = 10;
            rb.freezeRotation = true;
        }

        //Sub things 세팅
        if(talkCanvas !=null ){
            //talkCanvas = talkCanvas.GetChild(0);
            talkCanvas.gameObject.SetActive(false);
        }

        if(interactiveMark!=null) interactiveMark = interactiveMark.GetChild(0);

        if(onMadAnt){
            bulletsPos = bullets[0].transform.localPosition;
            // for(int i=0 ; i<3; i++){
            //     ThrowDownBullets();
            // }
        }
        
        if(mainBody!=null) defaultScale = mainBody.transform.localScale;
        else defaultScale = transform.localScale;

        //랜덤대화
        if(alwaysRandomDialogue){
            onRandomDialogue = true;
        }
    }

    void Update(){

        if(onJYD){
            wSet = patrolInput;
            if(!JYDFlag && !isPaused){
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
                if(haveWalk&&animator!=null) animator.SetBool("walk", true);
                if(wSet>0){
                    if(isSpriteRenderer){
                        spriteRenderer.flipX = false;
                    }
                    else{
                        mainBody.localScale = new Vector2(defaultScale.x, defaultScale.y);
                    }
                    if(rader!=null){
                        rader.transform.localScale = new Vector2(raderFlipX , rader.transform.localScale.y);
                    }
                    //transform.localScale = new Vector2(defaultScaleX, transform.localScale.y);
                }
                else if(wSet<0){
                    if(isSpriteRenderer){
                        spriteRenderer.flipX = true;
                    }
                    else{

                        mainBody.localScale = new Vector2(-defaultScale.x, defaultScale.y);
                    }
                    if(rader!=null){
                        rader.transform.localScale = new Vector2(raderFlipX * -1 , rader.transform.localScale.y);
                    }
                    //transform.localScale = new Vector2(defaultScaleX * -1, transform.localScale.y);
                }
            }
            else{
                
                if(haveWalk&&animator!=null)  animator.SetBool("walk", false);

            }
        }
#region 랜덤대화 관련 설정
        if(onRandomDialogue && !alwaysRandomDialogue){

            if(PlayerManager.instance.isActing || LoadManager.instance.reloadScene){
                if(!pauseRandomDialogue){
                    pauseRandomDialogue = true;
                    DialogueManager.instance.StopRandomDialogue_NPC(randomDialogueCrt);
                    talkCanvas.gameObject.SetActive(false);
                }
            }
            else{
                if(pauseRandomDialogue){
                    pauseRandomDialogue = false;
                }
            }
        }

        if(onRandomDialogue && !pauseRandomDialogue){
            if(!randomDialogueFlag){
                randomDialogueFlag = true;
                StartCoroutine(SetRandomDialogueCoroutine());
            }
        }
#endregion
        if(talkCanvas.gameObject.activeSelf){
            // isPaused = true;
            // patrolInput = 0;
            if(haveTalk&&animator!=null) animator.SetBool("talk", true);
            if(interactiveMark!=null) interactiveMark.gameObject.SetActive(false);
        }
        else{
            // isPaused = false;
            if(haveTalk&&animator!=null) animator.SetBool("talk", false);
        }
    }

    void FixedUpdate(){
        if(wSet!=0 && rb!=null){
            
            rb.velocity = new Vector2(speed * wSet  , rb.velocity.y);

        }

        // if(jumpInput){
        //     if(!isJumping && !jumpDelay){
        //         Jump();
        //     }
        // }

        
    }
    public void PauseNPC(){
        patrolInput = 0;
        isPaused = true;
    }
    public void UnpauseNPC(){
        
        isPaused = false;
    }

    IEnumerator SetJYD(){
        //if(!patrolFlag){
            JYDFlag = true;  
            if(!isNonStop){
                patrolInput = Random.Range(0,3) - 1;//0 좌 1 정지 2 우
            }
            else{
                int ranNum = Random.Range(0,2);
                patrolInput = ranNum == 1 ? -1 : 1;
            }
            yield return new WaitForSeconds(JYDCoolDown);
            JYDFlag = false;  

        //}
    }




    // void Jump(float multiple = 1){
    //     Debug.Log(multiple);
    //     StartCoroutine(JumpDelay());
    //     rb.velocity = Vector2.zero;
    //     rb.AddForce(Vector2.up*(jumpPower*multiple),ForceMode2D.Impulse);
    // }
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
        
        else if(other.gameObject.CompareTag("Collider_Player")){
            Physics2D.IgnoreCollision(other.gameObject.GetComponent<BoxCollider2D>(), circleCollider2D, true);
        }
        
        else if(other.gameObject.CompareTag("Collider_NPC")){
//            Debug.Log("AA");
            patrolInput = patrolInput == 1? -1 : 1;
            //Physics2D.IgnoreCollision(other.gameObject.GetComponent<BoxCollider2D>(), circleCollider2D, true);
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
                        if(animator!=null) animator.SetTrigger("found");
                        wSet = 0;
                        
                        UIManager.instance.SetGameOverUI(1);
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
                    //dialogues[i].sentences[j] = TextLoader.instance.ApplyText(temp);
                    dialogues[i].sentences[j] = CSVReader.instance.GetIndexToString(temp,"dialogue");
                }
            }
        }
    }
    public void ThrowDownBullets(){
        for(int i=0;i<bullets.Length;i++){

            bullets[i].transform.localPosition = bulletsPos;
            bullets[i].SetActive(true);
            bullets[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(i-1,0) * (2), ForceMode2D.Impulse);
        }
    }
    public void ThrowRightBullets(){
        for(int i=0;i<bullets.Length;i++){

            bullets[i].transform.localPosition = bulletsPos;
            bullets[i].SetActive(true);
            bullets[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(1,0) * (1), ForceMode2D.Impulse);
        }
    }
    public void Look(string direction){
        if(mainBody == null) return;

        switch(direction){
            case "left" : 
                //wSet = -1;
                if(spriteRenderer!=null) spriteRenderer.flipX = true;
               // mainBody.localScale = new Vector2(-defaultScale.x, defaultScale.y);
                break;
            case "right" : 
                //wSet = 1;
                if(spriteRenderer!=null) spriteRenderer.flipX = false;
                //mainBody.localScale = new Vector2(defaultScale.x, defaultScale.y);
                break;
        }
        SetTalkCanvasDirection();
        PlayerManager.instance.SetTalkCanvasDirection();
    }
    public void SetTalkCanvasDirection(){
        if(talkCanvas!=null){
            var v = 1;

            if(spriteRenderer!=null){
                v = spriteRenderer.flipX ? -1 : 1;
            }
            // else{
            //     var v = 1;
            // }


            var tempRect = talkCanvas.GetComponent<RectTransform>();
            tempRect.localPosition = new Vector2(defaultTalkCanvasHolderPosX * v , tempRect.localPosition.y);
            tempRect.localScale = new Vector2(v, tempRect.localScale.y);
            
            var tempRect1 = talkCanvas.GetChild(0).GetComponent<RectTransform>();
            tempRect1.localScale = new Vector2(v, tempRect.localScale.y);
        }
    }
    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);
}
