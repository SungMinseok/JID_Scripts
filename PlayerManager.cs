using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[System.Serializable]
public class EquipmentSprite{
    public int id;
    public Sprite sprite;
    public Sprite sprite_back;
}
[System.Serializable]
public class Armor{
    public string name;
    public int itemID;
    public GameObject[] objects;
}
[System.Serializable]
public class Helmet{
    public string name;
    public int itemID;
    public GameObject[] objects;
}
public class PlayerManager : CharacterScript
{

    public static PlayerManager instance;
    [Header("Objects━━━━━━━━━━━━━━━━━━━")]
    public SpriteRenderer sr_helmet;
    public SpriteRenderer sr_back_helmet;
    public SpriteRenderer sr_armor;
    public SpriteRenderer sr_back_armor;
    public Armor[] armors;
    public Helmet[] helmets;
    
    [Header("Wearable Objects")]
    //public SpriteRenderer[] helmets;
    public SpriteRenderer[] weapons;//장착 무기 별로 애니메이션 달라서 개별설정 필요
    // public SpriteRenderer sr_shovel;
    // public SpriteRenderer sr_pick;
    [Header("Current Status")]
    public float curSpeed;
    //public float curDirtAmount;
    //public float curHoneyAmount;
    [Header("Current Equipment : 0 Helmet/1 Armor/2 Weapon")]
    public int[] equipments_id;
    // public int equip_helmet_id;
    // public int equip_armor_id;
    // public int equip_weapon_id;
    //public byte armorNum;
    //public uint weaponNum;
    //[Header("Equipment Sprites ( Need Set ) ━━━━━━━━━━━━━━")]
    //public EquipmentSprite[] equipmentSprites;
    [Header("Set Default Status")]
    [SerializeField] [Range(2f, 10f)] public float speed;
    [SerializeField] [Range(10f, 50f)] public float jumpPower;
    [SerializeField] [Range(1f, 20f)] public float gravityScale;
    public float maxDirtAmount;
    public float maxHoneyAmount;

    Coroutine animationCoroutine;
    WaitForSeconds animDelay0 = new WaitForSeconds(0.833f);
    WaitForSeconds animDelay1 = new WaitForSeconds(0.82f);
    [Header("Input Check")]
    public float wInput;
    public float hInput;
    public bool jumpInput, downInput, interactInput, interactKeepInput;
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
    public bool onRope; // 밧줄 소리용
    public int bodyMode;    //0 : 후면, 1 : 정면
    public bool onKeyTutorial;
    public bool onMonologue;//독백중 애니메이션 X
    public byte flagID;//특수 설정을 위한 플래그값(권총 엔딩 등)
    public bool watchingGameEnding;//트루 엔딩 UI 발동 시 트루, 트리거 발동 제한
    [Header("────────────────────────────")]
    public float delayTime_WaitingInteract;
    public float delayTime_Jump;
    public float delayTime_JumpDown;
    public float delayTime_GetLadder;
    [Header("────────────────────────────")]
    public Transform talkCanvas;
    public LocationRader locationRader;
    public Transform tutorialBox;
    public Transform tutorialBox_Right;
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
    [Header("Animatior─────────────────────")]
    public Animator animator;
    public Animator animator_back;

    //public Transform dialogueHolder;
    [Header("Debug─────────────────────")]
    //public Collider2D lastPlatform;
    public Transform onTriggerCol;//onTrigger 콜라이더
    public Transform orderDestinationCol;
    public Collider2D footCol;
    //public GameObject curEquipment;
    public GameObject[] tempHelmet;
    [Header("Vignette━━━━━━━━━━━━━━━━━━")]
    public GameObject vignette;
    public GameObject darkerVignette;
    public GameObject redVignette;
    public GameObject[] redEyes;
    WaitForSeconds wait1000ms = new WaitForSeconds(1);
    WaitForSeconds wait500ms = new WaitForSeconds(0.5f);
    WaitForSeconds wait125ms = new WaitForSeconds(0.125f);
    Coroutine jumpDelayCoroutine;
    Coroutine getLadderDelayCoroutine;
    
    [Header("────────────────────────────")]
    public GameObject petHolder;
    void Awake()
    {
        //Application.targetFrameRate = 120;
        animator_back.speed = 0;
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        equipments_id = new int[3]{-1,-1,-1};
    }
    //public AnimationClip animation0;
    Color hideColor = new Color(1,1,1,0);
    Color unhideColor = new Color(1,1,1,1);
    void Start()
    {
        d = DebugManager.instance;
        defaultScale = playerBody.transform.localScale;

        //canMove = true;

        talkCanvas = transform.GetChild(0).GetChild(0);
        talkCanvas.gameObject.SetActive(false);
        defaultTalkCanvasHolderPosX = talkCanvas.GetComponent<RectTransform>().localPosition.x;

        spriteRenderers = animator.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer s in spriteRenderers){
            s.sortingLayerName = "Player";
        }
        spriteRenderers_back = animator_back.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer s in spriteRenderers_back){
            s.sortingLayerName = "Player";
        }
        for(int i=0;i<weapons.Length;i++){
            weapons[i].gameObject.SetActive(false);
        }
        ApplyEquipments();
    }


    void Update()
    {
        //if(!UIManager.instance.ui_endingGuide.activeSelf){// && MenuManager.instance != null && !MenuManager.instance.menuPanel.activeSelf){

            interactInput = GameInputManager.GetKeyDown("Interact") ? true : false;
            interactKeepInput = GameInputManager.GetKey("Interact") ? true : false;
        //}

        if(isGameOver /* || UIManager.instance.ui_gameEnd.activeSelf */){
            canMove = false;
        }
        // if(MenuManager.instance != null){
        //     canMove = MenuManager.instance.menuPanel.activeSelf ? false : true;
        // }
        if (canMove)
        {
            wSet = 0;
            wInput = Input.GetAxisRaw("Horizontal");
            hInput = Input.GetAxisRaw("Vertical");
            //jumpInput = Input.GetButton("Jump") ? true : false;
            jumpInput = GameInputManager.GetKey("Jump") ? true : false;
            //interactInput = Input.GetButton("Interact_OnlyKey") ? true : false;

            if(DBManager.instance.curData.curDirtAmount<=0 && canMove){
                canMove = false;
                StartCoroutine(DepletedDirt());
            }

            //가만히 있어도 OnTriggerStay2D 발동하게 함
            rb.AddForce(Vector2.zero);
            
            //움직이는 중 체크
            if(rb.velocity.x > 0.1f ||rb.velocity.x < -0.1f ||rb.velocity.y > 0.1f ||rb.velocity.y< -0.1f ){
                isMoving = true;
            }
            else{
                isMoving = false;
            }
            //isMoving = rb.velocity != Vector2.zero ? true : false;
        }
        else{

            wInput = 0;
            hInput = 0;
            jumpInput = false;
        }


#region case 1 : 가만히 있거나 걸을 때(공중에 떠 있지 않을 때)
        if (isGrounded)
        {
            isJumping = false;
            if(isFalling){
                isFalling = false;
                //씬 로드 중 착지 소리 방지
                if(!LoadManager.instance.reloadScene)
                    SoundManager.instance.PlaySound("lucky_land");
            }

            animator.SetBool("jump", false);

            if(hInput<0 && !getLadder){
                
                animator.SetBool("down", true);
                bodyCollider2D.enabled = false;

                if(jumpInput && !jumpDownFlag){
                    jumpDownFlag = true;
                    if(footCol!=null) StartCoroutine(JumpDown());
                }
            }
            else{
                animator.SetBool("down", false);
                bodyCollider2D.enabled = true;

            }

        }
#endregion

#region case 2 : 공중에 떠 있을 때
        else
        {
            //점프안하고 추락 시 점프 가능한 것 방지
            if(!jumpDelay && !onLadder) Jump(0);
            animator.SetBool("jump", true);
            animator.SetBool("down", false);

            isFalling = rb.velocity.y < 0 ? true : false;

        }
#endregion

#region case 3 : 좌우 이동 중
        if ((wInput != 0 || wSet != 0 || isForcedRun) )   
        {
            if(!isForcedRun) animator.SetBool("run", true);
            if (wInput > 0|| wSet > 0)
            {
                playerBody.localScale = new Vector2(defaultScale.x, defaultScale.y);
            }
            else if (wInput < 0|| wSet < 0)
            {
                if(!isForcedRun)
                    playerBody.localScale = new Vector2(-defaultScale.x, defaultScale.y);
            }
        }
#endregion

#region case 4 : 좌우로 이동 중이지 않을 때
        else
        {
            
            animator.SetBool("run", false);
            
            if(interactKeepInput && !isActing){
                if(getDirt){
                    //if(equipments_id[2]==21){
                    if(InventoryManager.instance.CheckHaveItem(21)){

                        animator.SetBool("shoveling1", true);
                        if(!digFlag){
                            digFlag = true;
                            if(animationCoroutine!=null) StopCoroutine(animationCoroutine);
                            animationCoroutine = StartCoroutine(CheckAnimationState(0));
                            dirtTarget.GetDug();
                        }
                    }
                }
                else if(getIcicle){
                    //if(equipments_id[2]==26){
                    if(InventoryManager.instance.CheckHaveItem(26)){
                        animator.SetBool("icebreak", true);
                        if(!digFlag){
                            digFlag = true;
                            if(animationCoroutine!=null) StopCoroutine(animationCoroutine);
                            animationCoroutine = StartCoroutine(CheckAnimationState(1));
                            dirtTarget.GetDug();
                        }
                    }
                }
            }
            else{
                animator.SetBool("shoveling1", false);
                animator.SetBool("icebreak", false);

            }
        }

        if(DBManager.instance.curData.curDirtAmount / DBManager.instance.maxDirtAmount <= 0.1f
        ||(isGameOver&&!UIManager.instance.ui_gameOver.activeSelf)
        ){
            redVignette.SetActive(true);
        }
        else{
            redVignette.SetActive(false);
        }
#endregion


        if(talkCanvas.gameObject.activeSelf && !DialogueManager.instance.canSkip2){
            if(!onMonologue){
                animator.SetBool("talk", true);

            }
        }
        else{
            animator.SetBool("talk", false);

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

        if (wInput != 0)
        {
            if(!animator.GetBool("down")){

                rb.velocity = new Vector2(curSpeed * wInput, rb.velocity.y);
            }

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
                Jump();
            }

        }

        if(getLadder){
            if(hInput<0 || (hInput>0&&!isGrounded)){
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
                
                if(getLadderDelayCoroutine!=null) StopCoroutine(getLadderDelayCoroutine);
                getLadderDelayCoroutine = StartCoroutine(GetLadderDelay());
                Jump();
            }
            else{   
                isJumping = false;
                rb.gravityScale = 0f;
                rb.velocity = new Vector2(0, (curSpeed*0.7f) * hInput  );
                ToggleBodyMode(0);
            }

                
            if(hInput!=0){
                animator_back.speed = 1;
            }
            else{
                animator_back.speed = 0;
            }

            if(isGrounded&&wInput!=0&&hInput==0){
                onLadder = false;
                rb.gravityScale = gravityScale;
                ToggleBodyMode(1);
            }

        }
        else{
            rb.gravityScale = gravityScale;
            ToggleBodyMode(1);
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
        bodyMode = modeNum;
        if(modeNum == 0 ){
            for(int i=0; i<spriteRenderers.Length;i++){
                spriteRenderers[i].color = hideColor;
            }
            for(int i=0; i<spriteRenderers_back.Length;i++){
                spriteRenderers_back[i].color = unhideColor;
            }
        }
        else{

            animator_back.speed = 0;
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
        Debug.Log("Jump");
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
        //SpriteRenderer _spriteRenderer ;

        // switch(animNum){
        //     case 0 : 
        //         _spriteRenderer = weapons[0];
        //         break;
        //     case 1 : 
        //         _spriteRenderer = weapons[1];
        //         break;
        //     default :
        //         _spriteRenderer = null;
        //         break;
        // }
        // if(_spriteRenderer != null) _spriteRenderer.gameObject.SetActive(true);

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
        
        //if(_spriteRenderer != null) _spriteRenderer.gameObject.SetActive(false);

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
    public void KillPlayer(int endingCollectionNum, string animationName = "dead0"){
        PlayerManager.instance.isGameOver = true;
        PlayerManager.instance.canMove = false;
        PlayerManager.instance.animator.SetBool(animationName,true);
        switch(animationName){
            case "dead0" :
                SoundManager.instance.PlaySound("lucky_die_0"+UnityEngine.Random.Range(1,3));
                //SoundManager.instance.PlaySound("lucky_die_02");
                break;
            case "drowning" :
                SoundManager.instance.PlaySound("waterdrop_0"+UnityEngine.Random.Range(1,3));
                //SoundManager.instance.PlaySound("lucky_die_02");
                break;
            default :
                break;

        }
        UIManager.instance.SetGameOverUI(endingCollectionNum);
    }

    public void LockPlayer(){
        PlayerManager.instance.canMove = false;
    }
    public void UnlockPlayer(){
        PlayerManager.instance.canMove = true;
        PlayerManager.instance.isActing = false;
    }
    public void ToggleInteract(bool active){//false : 트리거 상호작용 잠그기
        if(active){
            PlayerManager.instance.isActing = false;
        }
        else{
            PlayerManager.instance.isActing = true;
            
        }
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
    public void SetEquipment(byte type, int itemID){//itemType 0:기타, 1:헬멧, 2:옷, 3:무기, 4.소모품

        switch(type){
            case 1:
                if(equipments_id[0]!=itemID){
                    equipments_id[0] = itemID;
                    // for(int i=0;i<equipmentSprites.Length;i++){
                    //     if(equipmentSprites[i].id==itemID){
                    //         Debug.Log("착용, itemID : " + itemID + ", itemName : "+ DBManager.instance.cache_ItemDataList[itemID].name);
                    //         break;
                    //     }
                    // }
                    // Debug.Log("fail equip : no item");
                    
                }
                else{
                    equipments_id[0] = -1;
                    //Debug.Log("착용 해제, itemID : " + itemID + ", itemName : "+ DBManager.instance.cache_ItemDataList[itemID].name);
                }
                break;   
            case 2: //옷
                if(equipments_id[1]!=itemID){
                    equipments_id[1] = itemID;
                    // for(int i=0;i<equipmentSprites.Length;i++){
                    //     if(equipmentSprites[i].id==itemID){
                    //         Debug.Log("착용, itemID : " + itemID + ", itemName : "+ DBManager.instance.cache_ItemDataList[itemID].name);
                    //         break;
                    //     }
                    // }
                    // Debug.Log("fail equip : no item");
                    
                }
                else{
                    equipments_id[1] = -1;
                    //Debug.Log("착용 해제, itemID : " + itemID + ", itemName : "+ DBManager.instance.cache_ItemDataList[itemID].name);
                }
                break;
         
            case 3:
                // if(equipments_id[2]!=itemID){
                //     equipments_id[2] = itemID;
                //     Debug.Log("무기 착용, itemID : " + itemID + ", itemName : "+ DBManager.instance.cache_ItemDataList[itemID].name);
                // }
                // else{
                //     equipments_id[2] = -1;
                //     Debug.Log("무기 착용 해제, itemID : " + itemID + ", itemName : "+ DBManager.instance.cache_ItemDataList[itemID].name);
                // }
                
                if(equipments_id[2]!=itemID){
                    equipments_id[2] = itemID;
                    // for(int i=0;i<equipmentSprites.Length;i++){
                    //     if(equipmentSprites[i].id==itemID){
                    //         Debug.Log("착용, itemID : " + itemID + ", itemName : "+ DBManager.instance.cache_ItemDataList[itemID].name);
                    //         break;
                    //     }
                    // }
                    // Debug.Log("fail equip : no item");
                    
                }
                else{
                    equipments_id[2] = -1;
                    //Debug.Log("착용 해제, itemID : " + itemID + ", itemName : "+ DBManager.instance.cache_ItemDataList[itemID].name);
                }
                break;

            default :
                break;
        }
        ApplyEquipments();
    }
    public void ApplyEquipments(){
        // for(int i=0;i<equipmentSprites.Length;i++){
        //     if(equipmentSprites[i].id == equipments_id[0]){
        //         sr_helmet.sprite = equipmentSprites[i].sprite;
        //     sr_back_helmet.enabled = true;
        //         sr_back_helmet.sprite = equipmentSprites[i].sprite_back;
        //         break;
        //     }
        //     sr_helmet.sprite = null;
        //     sr_back_helmet.enabled = false;
            
        // }
        // var helmetIndex = Array.FindIndex(equipmentSprites, x => x.id == equipments_id[0]);

        // if(helmetIndex != -1){
            
        // }



        // for(int i=0;i<equipmentSprites.Length;i++){
        //     if(equipmentSprites[i].id == equipments_id[1]){
        //         sr_armor.sprite = equipmentSprites[i].sprite;
        //         sr_back_armor.sprite = equipmentSprites[i].sprite_back;
        //         break;
        //     }
        //     sr_armor.sprite = null;
        //     sr_back_armor.sprite = null;
            
        // }


        for(int i=0;i<helmets.Length;i++){
            foreach(GameObject curHelmet in helmets[i].objects){
                curHelmet.SetActive(false);
            }
        }
        var helmetIndex = Array.FindIndex(helmets, x => x.itemID == equipments_id[0]);

        if(helmetIndex != -1){
            foreach(GameObject curHelmet in helmets[helmetIndex].objects){
                curHelmet.SetActive(true);
            }
        }




        for(int i=0;i<armors.Length;i++){
            foreach(GameObject curArmor in armors[i].objects){
                curArmor.SetActive(false);
            }
        }
        var armorIndex = Array.FindIndex(armors, x => x.itemID == equipments_id[1]);

        if(armorIndex != -1){
            foreach(GameObject curArmor in armors[armorIndex].objects){
                curArmor.SetActive(true);
            }
        }

        // for(int i=0;i<equipmentSprites.Length;i++){
        //     if(equipmentSprites[i].id == equipments_id[2]){
        //         sr_shovel.sprite = equipmentSprites[i].sprite;
        //         break;
        //     }
        //     sr_weapon.sprite = null;
        // }

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
        SoundManager.instance.PlayLoopSound("lucky_afraid");
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
        SoundManager.instance.StopLoopSound();
        UIManager.instance.SetGameOverUI(2);
    }

    public void ResetRigidbody(){
        
        rb.AddForce(Vector2.zero);
    }

    public void PlayerLookObject(Transform target){
        if(target == null){
            Debug.LogWarning("No target");
            return;
        }

        if(this.transform.position.x > target.position.x){
            Look("left");
        }
        else{
            Look("right");
        }
        
        PlayerManager.instance.SetTalkCanvasDirection();
    }
    
    public void SetMainBodySize(float scale, float speed = 0.01f){
        circleCollider2D.transform.SetParent(playerBody.transform);
        bodyCollider2D.transform.SetParent(playerBody.transform);

        StartCoroutine(SetMainBodySizeCoroutine(scale,speed));
    }
    IEnumerator SetMainBodySizeCoroutine(float scale, float speed){
        //Debug.Log(speed);
        WaitForSeconds waitSpeed = new WaitForSeconds(speed);
        while(playerBody.localScale.x < scale){
            playerBody.localScale += new Vector3(0.1f,0.1f)  ;//new Vector2(playerBody.localScale + 0.1f)
            yield return waitSpeed;

        }
    }
}
