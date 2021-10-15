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
    public SpriteRenderer wearables0;
    Coroutine animationCoroutine;
    WaitForSeconds animDelay0 = new WaitForSeconds(0.833f);
    [Header("Input Check")]
    public float wInput;
    public float hInput;
    public bool jumpInput, downInput, interactInput;
    [Space]
    public float wSet;
    [Header("States────────────────────")]
    public bool canMove;
    public bool isTalking;
    public bool isSelecting;
    public bool isPlayingMinigame;
    public bool isActing;   //트리거 로케이션 onTriggerStay 체크용 bool 값
    public bool isGrounded;
    public bool isJumping;
    public bool jumpDelay;
    public bool getLadder;
    public bool ladderDelay;
    public bool onLadder;
    public bool isFalling;
    public bool isHiding;   //구조물에 숨은 상태 (경비개미에게 발각되지 않음)
    public bool isCaught;
    public bool isDead;
    public bool getDirt;    //플레이어가 흙더미 로케이션에 닿아있는 것 체크 (흙 팔 수 있는 상태)
    public bool digFlag;
    public bool isWaitingInteract;
    public bool isForcedRun;    //강제로 달리기 애니메이션 실행 (미니게임 2)
    public float isSlowDown;
    [Header("────────────────────────────")]
    public float delayTime_WaitingInteract;
    public float delayTime_Jump;
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
    public BoxCollider2D boxCollider2D;
    public CircleCollider2D circleCollider2D;
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
    public GameObject curEquipment;
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
        if (canMove)
        {
            wSet = 0;
            wInput = Input.GetAxisRaw("Horizontal");
            hInput = Input.GetAxisRaw("Vertical");
            jumpInput = Input.GetButton("Jump") ? true : false;
            interactInput = Input.GetButton("Interact_OnlyKey") ? true : false;





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
            //점프안하고 추락 시 점프 가능한 것 방지
            if(!jumpDelay) Jump(0);
            animator.SetBool("jump", true);
            animator.SetBool("down", false);
        }

//좌우 이동 중
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
                playerBody.localScale = new Vector2(-defaultScale.x, defaultScale.y);
            }
        }
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
                //wearables0.gameObject.SetActive(true);
            }
            else{
                animator.SetBool("shoveling1", false);
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

        if (jumpInput && !isJumping)
        {
            if (!jumpDelay && !onLadder)
            {
                //jumpDelay = true;
                Jump();
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
            
            if(jumpInput && wInput!=0){
                onLadder = false;
                StartCoroutine(GetLadderDelay());
                if(!jumpDelay) Jump();
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
        jumpDelay = true;
//        Debug.Log(multiple);
        StartCoroutine(JumpDelay());
        rb.velocity = Vector2.zero;
        Debug.Log("Jump");
        rb.AddForce(Vector2.up * (jumpPower * multiple), ForceMode2D.Impulse);
    }
    IEnumerator JumpDelay()
    {
        //if (!jumpDelay)
        //{

        //    jumpDelay = true;
            //yield return new WaitForSeconds(0.5f);

            yield return new WaitForSeconds(delayTime_Jump);
            //if(!onLadder)
                yield return new WaitUntil(() => isGrounded);
            //else
            //    jumpDelay = false;
            jumpDelay = false;
        //}
    }
    IEnumerator GetLadderDelay()
    {
        if (!ladderDelay)
        {
//            Debug.Log("33");
            getLadder = false;
            ladderDelay = true;
            yield return new WaitForSeconds(0.5f);
            ladderDelay = false;
        }
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
                _spriteRenderer = wearables0;
                break;
            default :
                _spriteRenderer = null;
                break;
        }
        if(_spriteRenderer != null) _spriteRenderer.gameObject.SetActive(true);

        // while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.8f){
        //     yield return null;
        //     Debug.Log("대기");
        // }

        yield return animDelay0;
        //진행중
        // while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.833f){

        //     if(_spriteRenderer != null) _spriteRenderer.gameObject.SetActive(true);
        //     yield return null;
        // }
        if(_spriteRenderer != null) _spriteRenderer.gameObject.SetActive(false);

        switch(animNum){
            case 0 : 
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
        PlayerManager.instance.isDead = false;
        
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
        //byte itemID = (byte)_itemID;
        switch(itemID){
            case 2 :
                curEquipment = helmets[0].gameObject;
                break;
        }

        switch(type){
            case 1:
                if(equipments[0]!=itemID){
                    equipments[0] = itemID;
                    curEquipment.gameObject.SetActive(true);
                    Debug.Log("헬멧 착용, itemID : " + itemID + ", itemName : "+ DBManager.instance.cache_ItemDataList[itemID].name);
                }
                else{
                    equipments[0] = -1;
                    curEquipment.gameObject.SetActive(false);
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
    public void SetTalkCanvasDirection(){
        if(talkCanvas!=null){
            //var v = 1;
            Debug.Log("A");
            
            var v = playerBody.localScale.x > 0 ? 1 : -1;
            
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
}
