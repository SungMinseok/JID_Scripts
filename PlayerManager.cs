using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
[System.Serializable]
public class PlayerStat{
    public float speedBonus;
    public float jumpPowerBonus;//dirtResistance,5|speed,5|
    public float dirtResistanceBonus;
    public float luckBonus;

}
[System.Serializable]
public class ItemStat{
    public string statName;
    public float statAmount;
    public ItemStat(string _statName, float _statAmount){
        statName = _statName;
        statAmount = _statAmount;
    }
}
[System.Serializable]
public class Armor{
    public string name;
    public int itemID;
    public GameObject[] objects;
    public ItemStat[] stats;
}
// [System.Serializable]
// public class Helmet{
//     public string name;
//     public int itemID;
//     public GameObject[] objects;
// }
public class PlayerManager : CharacterScript
{

    public static PlayerManager instance;
    [Header("Objects━━━━━━━━━━━━━━━━━━━")]
    public bool useWearableScript;
    List<Armor> armorList;
    List<Armor> helmetList;
    List<GameObject> armorObjList;
    List<GameObject> helmetObjList;
    //public Armor[] armors;
    //public Helmet[] helmets;
    public WearableScript wearable;
    
    [Header("Wearable Objects")]
    //public SpriteRenderer[] helmets;
    public SpriteRenderer[] weapons;//장착 무기 별로 애니메이션 달라서 개별설정 필요
    // public SpriteRenderer sr_shovel;
    // public SpriteRenderer sr_pick;
    [Header("Current Status")]
    public float curSpeed;
    public float curLuck;
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
    public PlayerStat stat;

    [Header("Status")]
    //[SerializeField] [Range(2f, 10f)] public float speed;
    //[SerializeField] [Range(10f, 50f)] public float jumpPower;
    //[SerializeField] [Range(1f, 20f)] public float gravityScale;
    [Header("Set Real Default Status")]
    public float defaultSpeed;
    public float defaultJumpPower;
    public float defaultGravityScale;
    public float maxDirtAmount;
    public float maxHoneyAmount;

    Coroutine animationCoroutine;
    WaitForSeconds animDelay0 = new WaitForSeconds(0.833f);
    WaitForSeconds animDelay1 = new WaitForSeconds(0.82f);
    [Header("Input Check")]
    public float wInput;
    public float hInput;
    public bool jumpInput, downInput, interactInput, interactKeepInput,petInput;
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
    public bool isSummoning;
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
    public GameObject iceVignette;
    public GameObject vignette_talkBox;
    public GameObject[] redEyes;
    WaitForSeconds wait1000ms = new WaitForSeconds(1);
    WaitForSeconds wait500ms = new WaitForSeconds(0.5f);
    WaitForSeconds wait125ms = new WaitForSeconds(0.125f);
    Coroutine jumpDelayCoroutine;
    Coroutine getLadderDelayCoroutine;
    
    [Header("────────────────────────────")]
    public GameObject petHolder;
    public PetScript petScript;
    public bool keepAlertStateFlag;//흙부족 알람 체크용
    public float keepAlertStateTime;//흙부족 유지 시간
    Coroutine keepAlertStateCoroutine;
    WaitForSecondsRealtime waitForSecondsRealtime ;
    [Header("ETC────────────────────────────")]
    public GameObject tempMainGround;
    public ParticleSystem[] dirtEffects;
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
        //defaultSpeed=speed;
        //defaultJumpPower=jumpPower;
        //defaultGravityScale=gravityScale;
    }
    //public AnimationClip animation0;
    
    [SerializeField]
    Color hideColor = new Color(1,1,1,0);
    [SerializeField]
    Color unhideColor = new Color(1,1,1,1);
    
    void Start()
    {
        d = DebugManager.instance;
        defaultScale = playerBody.transform.localScale;
        waitForSecondsRealtime = new WaitForSecondsRealtime(1);

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

        helmetList = wearable.helmets.ToList();
        armorList = wearable.wearables.ToList();

        //for(int i=0;i<curArmor.Length;i++){
        //}
        helmetObjList = new List<GameObject>();
        armorObjList = new List<GameObject>();
        foreach(Armor a in helmetList){
            foreach(GameObject b in a.objects){
                b.SetActive(false);
                helmetObjList.Add(b);
            }
        }
        foreach(Armor a in armorList){
            foreach(GameObject b in a.objects){
                b.SetActive(false);
                armorObjList.Add(b);
            }
        }


        ApplyEquipments(0);
        ApplyEquipments(1);
        ApplyEquipments(2);
        if(DBManager.instance.curData.isSummoning) ApplyPet();
    }


    void Update()
    {
        //if(!UIManager.instance.ui_endingGuide.activeSelf){// && MenuManager.instance != null && !MenuManager.instance.menuPanel.activeSelf){

            interactInput = GameInputManager.GetKeyDown("Interact") ? true : false;
            interactKeepInput = GameInputManager.GetKey("Interact") ? true : false;

            if(isSummoning){
                if(GameInputManager.GetKeyDown("Pet")&&!isTalking&&!isActing&&!isPlayingMinigame){
                    petScript.TrySave();
                }
                //petInput = GameInputManager.GetKeyDown("Pet") ? true : false;
            }
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

            //220817 추가
            if(DBManager.instance.dirtOnlyHUD){
                if(GameInputManager.GetKeyDown("AddDirt")){
                    UIManager.instance.PushDirtHolder();
                }
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


#region case 1-1 : 가만히 있거나 걸을 때(공중에 떠 있지 않을 때)
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

            //playerBody.localPosition = new Vector2(0,-0.03f);
        }
#endregion

#region case 1-2 : 공중에 떠 있을 때
        else
        {
            //점프안하고 추락 시 점프 가능한 것 방지
            if(!jumpDelay && !onLadder) Jump(0);
            animator.SetBool("jump", true);
            animator.SetBool("down", false);

            isFalling = rb.velocity.y < 0 ? true : false;

            //playerBody.localPosition = new Vector2(0,0.15f);
        }
#endregion

#region case 2-1 : 좌우 이동 중
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

#region case 2-2 : 좌우로 이동 중이지 않을 때
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
//심장박동 + 레드비넷
        if(DBManager.instance.curData.curDirtAmount / DBManager.instance.maxDirtAmount <= DBManager.instance.dirtAlertAmount
        ||(isGameOver&&!UIManager.instance.ui_gameOver.activeSelf)
        ){
            redVignette.SetActive(true);
            
            if(!keepAlertStateFlag){
                keepAlertStateFlag = true;
                keepAlertStateCoroutine = StartCoroutine(KeepAlertStateCoroutine());
            }
        }
        else{
            redVignette.SetActive(false);
            
            if(keepAlertStateCoroutine != null){
                keepAlertStateFlag = false;
                StopCoroutine(keepAlertStateCoroutine);
            }
        }
#endregion

        if(isFalling){
                animator.SetBool("fall", true);

        }
        else{
                animator.SetBool("fall", false);

        }
        
        if(talkCanvas.gameObject.activeSelf && !animator.GetBool("bed")){
            if(!onMonologue){
                if(!DialogueManager.instance.canSkip2){
                    animator.SetBool("think", false);
                    animator.SetBool("talk", true);

                }
                else{
                    animator.SetBool("talk", false);
                    
                }

            }
            else{
                animator.SetBool("talk", false);
                animator.SetBool("think", true);

            }
        }
        else{
            animator.SetBool("talk", false);
            animator.SetBool("think", false);

        }
        
    }

    
    void FixedUpdate()
    {

        footPos = new Vector2(transform.position.x, (circleCollider2D.bounds.min.y));
        footRadius = 0.075f;

        isGrounded = Physics2D.OverlapCircle(footPos, footRadius, groundLayer);

//이속 제어
        curSpeed = defaultSpeed * (1 + PlayerManager.instance.stat.speedBonus) * (1 - isSlowDown);
        animator.SetFloat("walkSpeed", (1 + PlayerManager.instance.stat.speedBonus) * (1 - isSlowDown));

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
                rb.gravityScale = defaultGravityScale;
                ToggleBodyMode(1);
            }

        }
        else{
            rb.gravityScale = defaultGravityScale;
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



#region Jump
    void Jump(float multiple = 1)
    {
//        Debug.Log("Jump");
        //if(jumpDelay) return;
        //isJumping = true;
        //Debug.Log("jumpDelay : " + jumpDelay);
        jumpDelay = true;
//        Debug.Log(multiple);
        if(jumpDelayCoroutine!=null) StopCoroutine(jumpDelayCoroutine);
        jumpDelayCoroutine = StartCoroutine(JumpDelay());
        rb.velocity = Vector2.zero;
        //Debug.Log("Jump");
        rb.AddForce(Vector2.up * (defaultJumpPower * multiple), ForceMode2D.Impulse);
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
        rb.AddForce(Vector2.up * (defaultJumpPower * multiple), ForceMode2D.Impulse);
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

#endregion
    

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


    IEnumerator CheckAnimationState(int animNum){
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

        switch(animNum){
            case 0 : 
            case 1 : 
                digFlag = false;
                break;
            default :
                break;
        }
    }


#region Set Player States
    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>left or right
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
    public void SetLockPlayer(bool active){
        if(active){
            LockPlayer();
        }
        else{
            UnlockPlayer();
        }
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
#endregion
    
    
#region Set Equipments
    public void SetEquipment(byte type, int itemID){//itemType 0:기타, 1:헬멧, 2:옷, 3:무기, 4.소모품

        switch(type){
            case 1:
                if(equipments_id[0] != -1){
                    //var curArmor = !useWearableScript ? armors : wearable.wearables;
                    var armorItemIndex = DBManager.instance.cache_ItemDataList.FindIndex(x => x.ID == equipments_id[0]);

                    for(int i=0;i<DBManager.instance.cache_ItemDataList[armorItemIndex].itemStat.Count;i++){
                        ClearItemStat(DBManager.instance.cache_ItemDataList[armorItemIndex].itemStat[i]);
                    }
                }
                if(equipments_id[0]!=itemID){
                    equipments_id[0] = itemID;
                }
                else{
                    equipments_id[0] = -1;
                }
                break;   
            case 2: //옷

                if(equipments_id[1] != -1){
                    var armorItemIndex = DBManager.instance.cache_ItemDataList.FindIndex(x => x.ID == equipments_id[1]);

                    for(int i=0;i<DBManager.instance.cache_ItemDataList[armorItemIndex].itemStat.Count;i++){
                        ClearItemStat(DBManager.instance.cache_ItemDataList[armorItemIndex].itemStat[i]);
                    }
                }


                if(equipments_id[1]!=itemID){
                    equipments_id[1] = itemID;
                }
                else{
                    equipments_id[1] = -1;
                }
                break;
         
            case 3:
                
                if(equipments_id[2]!=itemID){
                    equipments_id[2] = itemID;
                }
                else{
                    equipments_id[2] = -1;
                }
                break;

            default :
                break;
        }
        ApplyEquipments(type-1);
    }
    
    /// <summary>
    /// 0 Helmet/1 Armor/2 Weapon
    /// </summary>
    /// <param name="equipmentType">0 Helmet/1 Armor/2 Weapon</param>
    public void ApplyEquipments(int equipmentType){
        //헬멧 적용 방식 변경 > wearableScript
        //220925
        // Armor curEquipment = null;

        // switch(equipmentType){
        //     case 0 :
        //         curEquipment = helmetList.Find(x => x.itemID == equipments_id[0]);
        //         break;
        //     case 1 :
        //         curEquipment = armorList.Find(x => x.itemID == equipments_id[1]);
        //         break;
        // }
        // var curHelmet = wearable.helmets;
        // var curHelmetSpriteIndex = Array.FindIndex(curHelmet, x => x.itemID == equipments_id[0]);
        // var curHelmetItemIndex = DBManager.instance.cache_ItemDataList.FindIndex(x => x.ID == equipments_id[0]);//List.FindIndex(DBManager.instance.cache_ItemDataList, x => x.itemID == equipments_id[1]);

        // for(int i=0;i<curHelmet.Length;i++){
        //     foreach(GameObject a in curHelmet[i].objects){
        //         a.SetActive(false);
        //     }
        // }

        // if(curHelmetSpriteIndex != -1){
        //     foreach(GameObject a in curHelmet[curHelmetSpriteIndex].objects){
        //         a.SetActive(true);
        //     }
        //     for(int i=0;i<DBManager.instance.cache_ItemDataList[curHelmetItemIndex].itemStat.Count;i++){
        //         ApplyItemStat(DBManager.instance.cache_ItemDataList[curHelmetItemIndex].itemStat[i]);
        //     }
        // }

        // //옷 적용 방식 변경 > wearableScript
        // var curArmor = wearable.wearables;
        // var armorSpriteIndex = Array.FindIndex(curArmor, x => x.itemID == equipments_id[1]);
        // var armorItemIndex = DBManager.instance.cache_ItemDataList.FindIndex(x => x.ID == equipments_id[1]);//List.FindIndex(DBManager.instance.cache_ItemDataList, x => x.itemID == equipments_id[1]);

        // for(int i=0;i<curArmor.Length;i++){
        //     foreach(GameObject a in curArmor[i].objects){
        //         a.SetActive(false);
        //     }
        // }
        // if(armorSpriteIndex != -1){
        //     foreach(GameObject a in curArmor[armorSpriteIndex].objects){
        //         a.SetActive(true);
        //     }

        //     for(int i=0;i<DBManager.instance.cache_ItemDataList[armorItemIndex].itemStat.Count;i++){
        //         ApplyItemStat(DBManager.instance.cache_ItemDataList[armorItemIndex].itemStat[i]);
        //     }
        // }
        // //SetPlayerStat();
        //헬멧 적용 방식 변경 > wearableScript
        //220925



        


        List<Armor> curArmorList = new List<Armor>();                   //해당 아머 참조용
        List<GameObject> curArmorAllObjList = new List<GameObject>();   //스프라이트 해제용(해당 아머타입 전체)
        List<GameObject> curEquipmentObjList = new List<GameObject>();  //스프라이트 적용용(해당 아머)
        List<ItemStat> curEquipmentItemStatList = new List<ItemStat>(); //해당 아머의 아이템 스탯 참조용

        switch(equipmentType){
            case 0 :
                curArmorList = helmetList;
                curArmorAllObjList = helmetObjList;
                break;
            case 1 :
                curArmorList = armorList;
                curArmorAllObjList = armorObjList;
                break;
        }

        foreach(GameObject a in curArmorAllObjList){
            a.SetActive(false);
        }

        //해당 타입에 장착 중인 아이템 ID가 해당 타입에 없을 경우 리턴
        //if(!curArmorList.Select(x=>x.itemID).Contains(equipments_id[equipmentType])) return;
        if(equipments_id[equipmentType]==-1) return;

        curEquipmentObjList = curArmorList.Find(x => x.itemID == equipments_id[equipmentType]).objects.ToList();
        Debug.Log(curEquipmentObjList.Count);
        curEquipmentItemStatList = DBManager.instance.cache_ItemDataList.Find(x => x.ID == equipments_id[equipmentType]).itemStat;


        foreach(GameObject a in curEquipmentObjList){
            a.SetActive(true);
        }

        if(curEquipmentItemStatList!=null){
            foreach(ItemStat a in curEquipmentItemStatList){
                ApplyItemStat(a);
            }
        }
    }


#endregion
/// <summary>
/// 
/// </summary>
/// <param name="direction">right / left</param>
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
        //흙 고갈 사망 시 최초 지급용 220831
        LoadManager.instance.isDeadByDepletingDirt = true;
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
    
    public void SummonPet(){
        PlayerManager.instance.isSummoning = true;
        DBManager.instance.curData.isSummoning = true;
        petHolder.SetActive(true);
        PlayerManager.instance.petScript.SetStartDialogue();
    }
    
    public void ApplyPet(){
        PlayerManager.instance.isSummoning = true;
        petHolder.SetActive(true);

    }
    
    IEnumerator KeepAlertStateCoroutine(){
        keepAlertStateTime = 0f;

        while(true){
            yield return waitForSecondsRealtime ;
            keepAlertStateTime += 1f;
//5AFF00
            if(keepAlertStateTime>=30){
                SteamAchievement.instance.ApplyAchievements(10);
            }
        }
    }
    
    public void SetBodyColor(string hexaCode,float duration){
        StartCoroutine(SetBodyColorCoroutine(hexaCode,duration));
    }
    
    IEnumerator SetBodyColorCoroutine(string hexaCode,float duration){
//unhideColor
        var curCoolTime = 0f;

        ColorUtility.TryParseHtmlString("#"+hexaCode+"FF", out unhideColor);//5AFF00FF
        ColorUtility.TryParseHtmlString("#"+hexaCode+"00", out hideColor);//5AFF0000

        while(true){
            yield return waitForSecondsRealtime ;
            curCoolTime += 1f;
//5AFF00
            if(curCoolTime>=duration){
                        
                ColorUtility.TryParseHtmlString("#FFFFFFFF", out unhideColor);//FFFFFFFF
                ColorUtility.TryParseHtmlString("#FFFFFF00", out hideColor);//FFFFFF00
            }
        }
    }


#region ITEM STATS
    public void ApplyItemStat(ItemStat itemStat){
        string curStatName = itemStat.statName;
        float curStatAmount = itemStat.statAmount;
        Debug.Log(curStatAmount);

        switch(curStatName){
            case "speed" :
                PlayerManager.instance.stat.speedBonus += curStatAmount;
                break;

            case "luck" :
                PlayerManager.instance.stat.luckBonus += curStatAmount;
                break;

            default : 
                Debug.Log("no stat name.");
                break;
        }
    }
    
    public void ClearItemStat(ItemStat itemStat){

        string curStatName = itemStat.statName;
        float curStatAmount = itemStat.statAmount;

        switch(curStatName){
            case "speed" :
                PlayerManager.instance.stat.speedBonus -= curStatAmount;
                break;
            case "luck" :
                PlayerManager.instance.stat.luckBonus -= curStatAmount;
                break;

            default : 
                Debug.Log("no stat name.");
                break;
        }



    }
    public void SetPlayerStat(){

        //PlayerManager.instance.speed = PlayerManager.instance.defaultSpeed *(1 + PlayerManager.instance.stat.speedBonus);
    }
#endregion

    public void CaughtByHunter(){
        StartCoroutine(CaughtByHunterCoroutine());
        canMove = false;
        isActing = true;
    }
    
    
    IEnumerator CaughtByHunterCoroutine(){
        var hunter = HunterManager.instance.hunterBody;
        var tempDialogue = HunterManager.instance.dialogues0;
        WaitUntil waitTalking = new WaitUntil(()=>!PlayerManager.instance.isTalking);

        //darkerVignette.SetActive(true);
        UIManager.instance.SetHUD(false);
        UIManager.instance.SetMovieEffectUI(true);
        SceneController.instance.SetCameraDefaultZoomIn();            
        //SceneController.instance.SetSomeConfiner(SceneController.instance.mapZoomBounds[DBManager.instance.curData.curMapNum]);
        SceneController.instance.SetSomeConfiner(null);
        SceneController.instance.SetCameraNoised(1,1);
        SetTalkCanvasDirection("right");
        animator.SetBool("shake", true);
        yield return wait500ms;
        SoundManager.instance.PlayLoopSound("lucky_afraid");
        Look("left");
        yield return wait500ms;
        Look("right");
        yield return wait500ms;
        Look("left");

        Vector2 startingPos  = new Vector2(-1.61f, 9.92f);
        Vector2 finalPos = new Vector2(-1.61f, 0.3f);

        hunter.transform.localPosition = startingPos;
        hunter.gameObject.SetActive(true);
        //SetTempMainGround(true);
        DialogueManager.instance.SetDialogue(tempDialogue[0]);

        float elapsedTime = 0;
        float fallingDuration = 0.8f;
        while (elapsedTime <= fallingDuration)
        {
            hunter.transform.localPosition = Vector2.Lerp(startingPos, finalPos, (elapsedTime / fallingDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        hunter.animator.SetTrigger("land");
        yield return waitTalking;
        
        DialogueManager.instance.SetDialogue(tempDialogue[1]);
        yield return waitTalking;

        hunter.animator.SetTrigger("paperout");

        yield return wait1000ms;
        yield return wait500ms;
        yield return wait125ms;


        DialogueManager.instance.SetDialogue(tempDialogue[2]);
        yield return waitTalking;
        DialogueManager.instance.SetDialogue(tempDialogue[3]);
        yield return waitTalking;



        //yield return wait1000ms;

        SoundManager.instance.StopLoopSound();
        //흙 고갈 사망 시 최초 지급용 220831
        LoadManager.instance.isDeadByDepletingDirt = true;
        UIManager.instance.SetGameOverUI(21);
    }
    
    
    public void SetTempMainGround(bool active) => tempMainGround.SetActive(active);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="equipType">0 Helmet/1 Armor/2 Weapon</param>
    /// <param name="itemIDList"></param>
    /// <returns></returns>
    public bool CheckEquipments(int equipType, List<int> itemIDList){

        if(itemIDList.Contains(PlayerManager.instance.equipments_id[equipType])){
            return true;
        }
        else{
            return false;
        }

    }

    public bool CheckPlayerDoSomething(){
        if(PlayerManager.instance.isActing
        ||PlayerManager.instance.isPlayingMinigame
        ||PlayerManager.instance.isShopping){
            return true;
        }
        else{
            return false;
        }
    }

    public void PutDirtEffect(){
        for (int i = 0; i < dirtEffects.Length;i++){
            if (dirtEffects[i].isPlaying)
            {
                continue;
            }
            else
            {
                dirtEffects[i].Play();
                break;
            }
        }
    }
}
