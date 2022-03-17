using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public static PlayerManager instance;
    [Header("Current Status")]
    public float curSpeed;
    public float curDirtAmount;
    public float curHoneyAmount;
    [Header("Current Equipment : Helmet/Armor/Weapon")]
    public int[] equipments;
    //public byte armorNum;
    //public uint weaponNum;
    [Header("Set Default Status")]
    [SerializeField] [Range(2f, 10f)] public float speed;
    [SerializeField] [Range(10f, 50f)] public float jumpPower;
    [SerializeField] [Range(1f, 20f)] public float gravityScale;
    public float maxDirtAmount;
    public float maxHoneyAmount;

    [Header("Wearable Objects")]
    public SpriteRenderer[] helmets;
    public SpriteRenderer[] wearables;
    Coroutine animationCoroutine;
    WaitForSeconds animDelay0 = new WaitForSeconds(0.833f);
    WaitForSeconds animDelay1 = new WaitForSeconds(0.82f);
    [Header("Input Check")]
    public float wInput;
    public float hInput;
    public bool jumpInput, downInput, interactInput;
    [Space]
    public float wSet;
    [Header("States────────────────────")]
    public bool canMove;
    public bool isMoving;
    public bool isTalking;
    public bool isSelecting;
    public bool isPlayingMinigame;
    public bool isActing;   //트리거 로케이션 onTriggerStay 체크용 bool 값
    public bool isGrounded;
    public bool isJumping;
    public bool jumpDelay;
    public bool getLadder;
    public bool canJumpFromLadder;
    public bool ladderDelay;
    public bool onLadder;
    public bool isFalling;
    public bool isHiding;   //구조물에 숨은 상태 (경비개미에게 발각되지 않음)
    public bool isCaught;
    public bool isGameOver;
    public bool getDirt;    //플레이어가 흙더미 로케이션에 닿아있는 것 체크 (흙 팔 수 있는 상태)
    public bool getIcicle;    //플레이어가 고드름 로케이션에 닿아있는 것 체크 (흙 팔 수 있는 상태)
    public bool digFlag;
    public bool isWaitingInteract;
    public bool isForcedRun;    //강제로 달리기 애니메이션 실행 (미니게임 2)
    public float isSlowDown;
    public bool jumpDownFlag;
    public bool isInvincible;   //on일 경우 흙 소모 안됨(테스트용)
    public bool transferDelay;  //텔레포트 시 딜레이(바로 이동 방지)
    public bool isShopping;
    [Header("────────────────────────────")]
    public float delayTime_WaitingInteract;
    public float delayTime_Jump;
    public float delayTime_JumpDown;
    public float delayTime_GetLadder;
    [Header("────────────────────────────")]
    public Transform talkCanvas;
    public LocationRader locationRader;
    float defaultTalkCanvasHolderPosX;
    [Header("────────────────────────────")]
    public DirtScript dirtTarget;
    public Transform playerBody;
    public SpriteRenderer[] spriteRenderers;
    public SpriteRenderer[] spriteRenderers_back;
    Color tempColor = new Color(1,1,1,1);
    Vector2 defaultScale;
    public Rigidbody2D rb;
    public Collider2D bodyCollider2D;
    public Collider2D circleCollider2D;
    //SpriteRenderer spriteRenderer;

    [SerializeField] LayerMask groundLayer;
    Vector2 footPos;
    float footRadius;



    public DebugManager d;
    [Header("Animation─────────────────────")]
    public Animator animator;
    public Animator animator_back;

    //public Transform dialogueHolder;
    [Header("Debug─────────────────────")]
    //public Collider2D lastPlatform;
    public Transform onTriggerCol;//onTrigger 콜라이더
    public Transform orderDestinationCol;
    public Collider2D footCol;
    public GameObject curEquipment;
    public GameObject[] tempHelmet;
    [Header("ETC─────────────────────")]
    public GameObject vignette;
    public GameObject darkerVignette;
    public GameObject[] redEyes;
    WaitForSeconds wait1000ms = new WaitForSeconds(1);
    WaitForSeconds wait500ms = new WaitForSeconds(0.5f);
    WaitForSeconds wait125ms = new WaitForSeconds(0.125f);
    Coroutine jumpDelayCoroutine;
    Coroutine getLadderDelayCoroutine;
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

        equipments = new int[3]{-1,-1,-1};
    }
    public AnimationClip animation0;
    Color hideColor = new Color(1,1,1,0);
    Color unhideColor = new Color(1,1,1,1);
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();

        //animator = GetComponent<Animator>();
        //spriteRenderer = GetComponent<SpriteRenderer>();

        d = DebugManager.instance;
        defaultScale = playerBody.transform.localScale;

        canMove = true;

        talkCanvas = transform.GetChild(0).GetChild(0);
        talkCanvas.gameObject.SetActive(false);
        defaultTalkCanvasHolderPosX = talkCanvas.GetComponent<RectTransform>().localPosition.x;

        // Physics2D.IgnoreCollision(ObjectController.instance.npcs[0].gameObject.GetComponent<CircleCollider2D>(), circleCollider2D, true);
        // Physics2D.IgnoreCollision(ObjectController.instance.npcs[0].gameObject.GetComponent<CircleCollider2D>(), boxCollider2D, true);
    }


    void Update()
    {
        if(isGameOver){
            canMove = false;
        }
        if (canMove)
        {
            wSet = 0;
            wInput = Input.GetAxisRaw("Horizontal");
            hInput = Input.GetAxisRaw("Vertical");
            //jumpInput = Input.GetButton("Jump") ? true : false;
            jumpInput = Input.GetButton("Jump") ? true : false;
            interactInput = Input.GetButton("Interact_OnlyKey") ? true : false;

            if(DBManager.instance.curData.curDirtAmount<=0 && canMove){
                canMove = false;
                StartCoroutine(DepletedDirt());
            }

            //가만히 있어도 OnTriggerStay2D 발동하게 함
            rb.AddForce(Vector2.zero);
            
            //움직이는 중 체크
            isMoving = rb.velocity != Vector2.zero ? true : false;
        }
        else{

            wInput = 0;
            hInput = 0;
            jumpInput = false;
        }


        // case 1 : 가만히 있거나 걸을 때(공중에 떠 있지 않을 때)
        if (isGrounded)
        {
            isJumping = false;
            if(isFalling){
                isFalling = false;
                SoundManager.instance.PlaySound("lucky_land");
            }

            animator.SetBool("jump", false);

            if(hInput<0 && !getLadder){
                
                animator.SetBool("down", true);
                //bodyCollider2D.gameObject.SetActive(false);
                bodyCollider2D.enabled = false;

                if(jumpInput && !jumpDownFlag){
                    jumpDownFlag = true;
                    if(footCol!=null) StartCoroutine(JumpDown());
                }
            }
            else{
                animator.SetBool("down", false);
                //bodyCollider2D.gameObject.SetActive(true);
                bodyCollider2D.enabled = true;

            }

        }

        // case 2 : 공중에 떠 있을 때
        else
        {
            //점프안하고 추락 시 점프 가능한 것 방지
            if(!jumpDelay && !onLadder) Jump(0);
            animator.SetBool("jump", true);
            animator.SetBool("down", false);

            isFalling = rb.velocity.y < 0 ? true : false;

        }

        // case 3 : 좌우 이동 중
        if ((wInput != 0 || wSet != 0 || isForcedRun) )   
        {
            if(!isForcedRun) animator.SetBool("run", true);
            if (wInput > 0|| wSet > 0)
            {
                //spriteRenderer.flipX = false;
                playerBody.localScale = new Vector2(defaultScale.x, defaultScale.y);
            }
            else if (wInput < 0|| wSet < 0)
            {
                //spriteRenderer.flipX = true;
                if(!isForcedRun)
                    playerBody.localScale = new Vector2(-defaultScale.x, defaultScale.y);
            }
        }
        // case 4 : 좌우로 이동 중이지 않을 때
        else
        {
            
            animator.SetBool("run", false);
            
            if(interactInput){
                //InteractAction();
                if(getDirt){
                    if(equipments[2]==21){

                    //if()
                        //interactInput = false;
                        animator.SetBool("shoveling1", true);
                        //if(!animator.GetCurrentAnimatorStateInfo(0).IsName("shoveling")){
                        if(!digFlag){
                            digFlag = true;
                            if(animationCoroutine!=null) StopCoroutine(animationCoroutine);
    //                        Debug.Log("진행중이 아니여서 시작");
                            animationCoroutine = StartCoroutine(CheckAnimationState(0));
                            dirtTarget.GetDug();
                        }
                    }
                }
                else if(getIcicle){
                    if(equipments[2]==26){
                        animator.SetBool("icebreak", true);
                        if(!digFlag){
                            digFlag = true;
                            if(animationCoroutine!=null) StopCoroutine(animationCoroutine);
                            animationCoroutine = StartCoroutine(CheckAnimationState(1));
                            dirtTarget.GetDug();
                        }
                    }
                }
                //wearables0.gameObject.SetActive(true);
            }
            else{
                animator.SetBool("shoveling1", false);
                animator.SetBool("icebreak", false);
                //wearables0.gameObject.SetActive(false);

            }
        }

        
    }

    void FixedUpdate()
    {

        footPos = new Vector2(transform.position.x, (circleCollider2D.bounds.min.y));
        footRadius = 0.075f;

        isGrounded = Physics2D.OverlapCircle(footPos, footRadius, groundLayer);

//이속 제어
        curSpeed = speed * (1-isSlowDown);
        animator.SetFloat("walkSpeed", 1-isSlowDown);

        //nowPlatform = Physics2D.OverlapCircle(footPos, footRadius, groundLayer);

        //if(canMove){
        if (wInput != 0)
        {
            if(!animator.GetBool("down")){

                rb.velocity = new Vector2(curSpeed * wInput, rb.velocity.y);
            }
            // else if(isForcedRun){
            //     if(animator.GetBool("down")){
            //         rb.velocity = new Vector2(curSpeed * -1 * 0.5f, rb.velocity.y);
            //     }
            // }

        }
        else if (wSet != 0)
        {
            if(!animator.GetBool("down")){
                rb.velocity = new Vector2(curSpeed * wSet, rb.velocity.y);
            }
        }

        if (jumpInput && !isJumping && hInput>=0)
        {
            if (!jumpDelay && !onLadder)
            {
                //jumpDelay = true;
        //Debug.Log("jumpDelay : " + jumpDelay);
                Jump();
        //jumpDelay = true;
            }

        }

        if(getLadder){
            if(hInput!=0){
                onLadder = true;
                jumpDelay = false;
                
            }
        }
        else{
            onLadder = false;
        }

        if(onLadder){
            
            if(jumpInput && wInput!=0 && canJumpFromLadder){
                onLadder = false;
                
                //if(!jumpDelay){
                    if(getLadderDelayCoroutine!=null) StopCoroutine(getLadderDelayCoroutine);
                    getLadderDelayCoroutine = StartCoroutine(GetLadderDelay());
                    //jumpDelay = true;
        Debug.Log("jumpDelay : " + jumpDelay);
                    Jump();
                //}
            }
            else{   
                isJumping = false;
                rb.gravityScale = 0f;
                rb.velocity = new Vector2(0, (curSpeed*0.7f) * hInput  );
                // animator.gameObject.SetActive(false);
                // animator_back.gameObject.SetActive(true);
                ToggleBodyMode(0);
            }

                
            if(hInput!=0){
                animator_back.speed = 1;
            }
            else{

                animator_back.speed = 0;
            }

        }
        else{
            
            rb.gravityScale = gravityScale;
            ToggleBodyMode(1);
            //animator.gameObject.SetActive(true);
            //animator_back.gameObject.SetActive(false);
        }

//[미니게임 2] - 강제 달리기 모션 + 포복 시 뒤로 이동
        if(isForcedRun){
            animator.SetBool("runForced", true);

            if(animator.GetBool("down")){
                rb.velocity = new Vector2(curSpeed * -1 * 0.7f, rb.velocity.y);
            }

            if(isSlowDown > 0){
                rb.velocity = new Vector2(curSpeed * -1 * 2, rb.velocity.y);

            }
        }
        else if(!isForcedRun){
            animator.SetBool("runForced", false);
        }

//경비개미에게 잡혔을 시 이동 불가
        if(isCaught){
            canMove = false;
        }

    }
    void ToggleBodyMode(int modeNum){
        //tempColor.a = 0;
        if(modeNum == 0 ){
            for(int i=0; i<spriteRenderers.Length;i++){
                spriteRenderers[i].color = hideColor;
            }
            for(int i=0; i<spriteRenderers_back.Length;i++){
                spriteRenderers_back[i].color = unhideColor;
            }
        }
        else{

            for(int i=0; i<spriteRenderers.Length;i++){
                spriteRenderers[i].color = unhideColor;
            }
            for(int i=0; i<spriteRenderers_back.Length;i++){
                spriteRenderers_back[i].color = hideColor;
            }
        }
    }
    void Jump(float multiple = 1)
    {
        //if(jumpDelay) return;
        //isJumping = true;
        //Debug.Log("jumpDelay : " + jumpDelay);
        jumpDelay = true;
//        Debug.Log(multiple);
        if(jumpDelayCoroutine!=null) StopCoroutine(jumpDelayCoroutine);
        jumpDelayCoroutine = StartCoroutine(JumpDelay());
        rb.velocity = Vector2.zero;
        //Debug.Log("Jump");
        rb.AddForce(Vector2.up * (jumpPower * multiple), ForceMode2D.Impulse);
        if(multiple!=0) SoundManager.instance.PlaySound("LuckyJump");
    }
    void JumpFromLadder(float multiple = 1)
    {
        //if(jumpDelay) return;

        jumpDelay = true;
//        Debug.Log(multiple);
        if(jumpDelayCoroutine!=null) StopCoroutine(jumpDelayCoroutine);
        jumpDelayCoroutine = StartCoroutine(JumpDelay());
        rb.velocity = Vector2.zero;
        Debug.Log("JumpFromLadder");
        rb.AddForce(Vector2.up * (jumpPower * multiple), ForceMode2D.Impulse);
        if(multiple!=0) SoundManager.instance.PlaySound("LuckyJump");
    }
    IEnumerator JumpDelay()
    {
        //if (!jumpDelay)
        //{

        //    jumpDelay = true;
            //yield return new WaitForSeconds(0.5f);
            //Debug.Log("딜레이시작");

            yield return new WaitForSeconds(delayTime_Jump);
            //if(!onLadder)
                yield return new WaitUntil(() => isGrounded);
            //else
            //    jumpDelay = false;
            jumpDelay = false;
            //Debug.Log("딜레이끝");
        //}
    }
    IEnumerator GetLadderDelay()
    {
        if (!ladderDelay)
        {
//            Debug.Log("33");
            getLadder = false;
            ladderDelay = true;
            //jumpDelay = true;
            yield return new WaitForSeconds(delayTime_GetLadder);
            ladderDelay = false;
            //jumpDelay = false;
        }
    }

    IEnumerator JumpDown(){
        var tempCollider = footCol;
            //Debug.Log("222");

        if(tempCollider.gameObject.CompareTag("MainGround")){
            //Debug.Log("333");
            jumpDownFlag = false;
            yield break;
        }
            //Debug.Log("444");

        Jump(0.1f);
        //if(tempCollider!=null){

            Physics2D.IgnoreCollision(PlayerManager.instance.bodyCollider2D, tempCollider, true);
            Physics2D.IgnoreCollision(PlayerManager.instance.circleCollider2D, tempCollider, true);
        //}
        yield return new WaitForSeconds(delayTime_JumpDown);

        yield return new WaitUntil(()=>footCol!=null);

        //if(tempCollider!=null){

        
            Physics2D.IgnoreCollision(PlayerManager.instance.bodyCollider2D, tempCollider, false);
            Physics2D.IgnoreCollision(PlayerManager.instance.circleCollider2D, tempCollider, false);
        //}

        jumpDownFlag = false;
    }
    // void OnTriggerStay2D(Collider2D other)
    // {
    //     if (other.CompareTag("Ladder"))
    //     {
    //         if (!ladderDelay) getLadder = true;
    //     }

    //     else if (other.CompareTag("Dirt"))
    //     {
    //         getDirt = true;
    //         dirtTarget = other.GetComponent<DirtScript>();
    //         //UIManager.instance.clearPanel.SetActive(true);
    //     }

    //     else if (other.CompareTag("Cover"))
    //     {
    //         Debug.Log(other.gameObject.name);
    //         other.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
    //     }

    // }

    // void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag("Ladder"))
    //     {
    //         getLadder = false;
    //     }
    //     else if (other.CompareTag("Dirt"))
    //     {
    //         getDirt = false;
    //         //UIManager.instance.clearPanel.SetActive(true);
    //     }

    //     else if (other.CompareTag("Cover"))
    //     {
    //         other.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    //     }
    // }
    // void OnCollisionEnter2D(Collision2D other){
        
        
    //     if(other.gameObject.CompareTag("Collider_NPC")){
    //         Physics2D.IgnoreCollision(other.gameObject.GetComponent<BoxCollider2D>(), circleCollider2D, true);
    //         Physics2D.IgnoreCollision(other.gameObject.GetComponent<BoxCollider2D>(), boxCollider2D, true);
    //     }
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
    IEnumerator CheckAnimationState(int animNum){
        SpriteRenderer _spriteRenderer ;

        switch(animNum){
            case 0 : 
                _spriteRenderer = wearables[0];
                break;
            case 1 : 
                _spriteRenderer = wearables[1];
                break;
            default :
                _spriteRenderer = null;
                break;
        }
        if(_spriteRenderer != null) _spriteRenderer.gameObject.SetActive(true);

        switch(animNum){
            case 0 : 
                yield return animDelay0;
                break;
            case 1 : 
                yield return animDelay1;
                break;
            default :
                yield return null;
                break;
        }
        
        if(_spriteRenderer != null) _spriteRenderer.gameObject.SetActive(false);

        switch(animNum){
            case 0 : 
            case 1 : 
                digFlag = false;
                break;
            default :
                break;
        }



//        Debug.Log("종료");
    }


    public void RevivePlayer(){
        PlayerManager.instance.canMove = true;
        PlayerManager.instance.animator.SetBool("dead0", false);

        PlayerManager.instance.isCaught = false;
        PlayerManager.instance.isGameOver = false;
        PlayerManager.instance.isForcedRun = false;
        
    }

    public void LockPlayer(){
        PlayerManager.instance.canMove = false;
    }
    public void UnlockPlayer(){
        PlayerManager.instance.canMove = true;
        PlayerManager.instance.isActing = false;
    }
    public void MovePlayer(Transform destination){
        PlayerManager.instance.transform.position = destination.position;
    }
    public void ActivateWaitInteract(float waitTime = 1f){
        isWaitingInteract = true;
        Invoke("DeactivateWaitInteract",waitTime);
    }
    public void DeactivateWaitInteract(){
        isWaitingInteract = false;
    }
    public void SetEquipment(byte type, int itemID){// 0:기타, 1:헬멧, 2:옷, 3:무기, 4.소모품
        switch(itemID){
            //개미탈
            case 2 :
                tempHelmet[0] = helmets[0].gameObject;
                tempHelmet[1] = helmets[1].gameObject;
                //curEquipment = helmets[0].gameObject;
                break;
        }

        switch(type){
            case 1:
                if(equipments[0]!=itemID){
                    equipments[0] = itemID;
                    for(int i=0;i<2;i++){
                        tempHelmet[i].SetActive(true);
                    }
                    Debug.Log("헬멧 착용, itemID : " + itemID + ", itemName : "+ DBManager.instance.cache_ItemDataList[itemID].name);
                }
                else{
                    equipments[0] = -1;
                    for(int i=0;i<2;i++){
                        tempHelmet[i].SetActive(false);
                    }
                    Debug.Log("헬멧 착용 해제, itemID : " + itemID + ", itemName : "+ DBManager.instance.cache_ItemDataList[itemID].name);
                }
                break;            
            case 3:
                if(equipments[2]!=itemID){
                    equipments[2] = itemID;
                    //curEquipment.gameObject.SetActive(true);
                    Debug.Log("무기 착용, itemID : " + itemID + ", itemName : "+ DBManager.instance.cache_ItemDataList[itemID].name);
                }
                else{
                    equipments[2] = -1;
                    //curEquipment.gameObject.SetActive(false);
                    Debug.Log("무기 착용 해제, itemID : " + itemID + ", itemName : "+ DBManager.instance.cache_ItemDataList[itemID].name);
                }
                break;



                // switch(itemID){
                //     case 2:
                //         if(equipments[0]!=itemID){
                //             equipments[0] = itemID;
                //             helmets[0].gameObject.SetActive(true);
                //             Debug.Log("헬멧 착용, itemID : " + itemID + ", itemName : "+ DBManager.instance.itemDataList[itemID].itemName);
                //         }
                //         else{
                //             equipments[0] = -1;
                //             helmets[0].gameObject.SetActive(false);
                //             Debug.Log("헬멧 착용 해제, itemID : " + itemID + ", itemName : "+ DBManager.instance.itemDataList[itemID].itemName);
                //         }
                        
                //         break;
                // }
            default :
                break;
        }
    }
    public void SetTalkCanvasDirection(string direction = ""){
        if(talkCanvas!=null){
            //var v = 1;
//            Debug.Log("A");
            if(direction == ""){

                var v = playerBody.localScale.x > 0 ? 1 : -1;
                
                var tempRect = talkCanvas.GetComponent<RectTransform>();
                tempRect.localPosition = new Vector2(defaultTalkCanvasHolderPosX * v , tempRect.localPosition.y);
                tempRect.localScale = new Vector2(v, tempRect.localScale.y);
                
                var tempRect1 = talkCanvas.GetChild(0).GetComponent<RectTransform>();
                tempRect1.localScale = new Vector2(v, tempRect.localScale.y);
            }
            else if(direction == "left"){
                var tempRect = talkCanvas.GetComponent<RectTransform>();
                tempRect.localPosition = new Vector2(defaultTalkCanvasHolderPosX * 1 , tempRect.localPosition.y);
                tempRect.localScale = new Vector2(1, tempRect.localScale.y);
                
                var tempRect1 = talkCanvas.GetChild(0).GetComponent<RectTransform>();
                tempRect1.localScale = new Vector2(1, tempRect.localScale.y);
            }
            else if(direction == "right"){
                var tempRect = talkCanvas.GetComponent<RectTransform>();
                tempRect.localPosition = new Vector2(defaultTalkCanvasHolderPosX * -1 , tempRect.localPosition.y);
                tempRect.localScale = new Vector2(-1, tempRect.localScale.y);
                
                var tempRect1 = talkCanvas.GetChild(0).GetComponent<RectTransform>();
                tempRect1.localScale = new Vector2(-1, tempRect.localScale.y);

            }
        }
    }

    IEnumerator DepletedDirt(){
        darkerVignette.SetActive(true);
        UIManager.instance.SetHUD(false);
        SceneController.instance.SetLensOrthoSize(3.5f,0.04f);            
        SceneController.instance.SetSomeConfiner(SceneController.instance.mapZoomBounds[DBManager.instance.curData.curMapNum]);
        animator.SetBool("shake", true);
        //SceneController.instance.confiner2D.m_BoundingShape2D = null;//SceneController.instance.temp;
        yield return wait1000ms;
        yield return wait125ms;
        yield return wait125ms;
        for(int i=0; i<4;i++){
            redEyes[i].SetActive(true);
            yield return wait500ms;
        }
        for(int i=4; i<redEyes.Length;i++){
            redEyes[i].SetActive(true);
            yield return wait125ms;
        }
        yield return wait1000ms;
        UIManager.instance.SetGameOverUI(2);
    }

    public void ResetRigidbody(){
        
        rb.AddForce(Vector2.zero);
    }

}
