using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public enum ItemType{
    Honey,
    Dirt,
    Item,
}
public class ItemScript : MonoBehaviour
{
    public ItemType type;
    //[Header("Honey")]
    public int objectID;
    [Header("Honey ────────────────────")]
    public int amount_honey;
    public bool noGetHoneyAlert;
    [Header("Dirt ────────────────────")]
    public float amount_dirt;
    [Header("Item ────────────────────")]
    public float amount_item;
    public int itemID;
    //public string getItemDefaultDialogue = "4";
    [Header("Dialogue ────────────────────")]
    //을(를) 얻었다! 사용.
    public bool useDialogue = true;
    public bool isDefaultDialogue = true;
    public Dialogue getItemDialogue;
    
    [Header("Tutorial ────────────────────")]
    public int[] tutorialIds;
    [Header("Setting ────────────────────")]
    public bool isAvailable = true;
    public bool disableAnimation;
    public string getItemDefaultDialogue = "4";
    [Tooltip("true일 경우, 인벤토리 내에 있을 경우 미노출")]
    public bool isDisabledWhenHold;  //true일 경우, 인벤토리 내에 있을 경우 미노출
    public bool isPopMode; // 체크 시, 활성화 직후 튀어오르기
    [Header("Debug ────────────────────")]
    //[Header("Dirt")]
    //public float dirtAmount;
    public Animator animator;
    //public BoxCollider2D itemCol;
    //Vector2 itemVector;
    WaitUntil waitTalking = new WaitUntil(()=>!PlayerManager.instance.isTalking);
    bool getFlag;
    
    WaitForSeconds wait100ms = new WaitForSeconds(0.1f);
    WaitForSeconds wait500ms = new WaitForSeconds(0.5f);
    WaitForSeconds wait1000ms = new WaitForSeconds(1);
    WaitForSeconds wait2000ms = new WaitForSeconds(2);
    [Space]
    public Transform itemObject;
    void Awake(){
        if(isPopMode) isAvailable = false;
    }
    void Start(){
        if(type == ItemType.Dirt){
            itemObject = this.transform;
        }
        else{
            if(transform.childCount != 0)
                itemObject = transform.GetChild(0);
        }

        if(GetComponent<Animator>()!=null)
            animator= itemObject.GetComponent<Animator>();


        switch(type){
            case ItemType.Honey :
                isDefaultDialogue = false;
                itemObject.GetComponent<SpriteRenderer>().sprite = DBManager.instance.honeySprite;

                break;


            case ItemType.Item :
                
                itemObject.GetComponent<SpriteRenderer>().sprite = DBManager.instance.cache_ItemDataList[itemID].icon;

                if(isDefaultDialogue){


                    getItemDefaultDialogue = CSVReader.instance.GetIndexToString(int.Parse(getItemDefaultDialogue),"sysmsg");

                    if(DBManager.instance.language == "kr"){

                        getItemDialogue.sentences[0] = DBManager.instance.cache_ItemDataList[itemID].name + getItemDefaultDialogue;
                    }
                }

                break;
        }

        if(disableAnimation){
            animator.speed = 0;
        }

        if(isDisabledWhenHold){
            if(InventoryManager.instance.CheckHaveItem(itemID)){
                gameObject.SetActive(false);
            }
        }
    }
    void OnEnable(){
        getFlag = false;

        if(isPopMode){
            if(TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)){
                
                rb.AddForce(new Vector2(0,1) * (7), ForceMode2D.Impulse);
                Invoke("SetItemAvailable",1f);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other) {
        
        if(other.CompareTag("Player")){

            if(isAvailable){

                if(!getFlag) {
                    getFlag = true;

                    StartCoroutine(GetItemCoroutine());

                }
            }
        }

    }
    IEnumerator GetItemCoroutine(){
//아이템 습득 시 대화


        if(type == ItemType.Honey){
            StartCoroutine(GetItemAndRemoveCoroutine());
            //DM("꿀 충전 : "+amount_honey);
            if(noGetHoneyAlert){
            InventoryManager.instance.AddHoney(amount_honey);

            }
            else{
            InventoryManager.instance.AddHoney(amount_honey,activateDialogue:true);

            }
            //DBManager.instance.curData.curHoneyAmount+=amount_honey;
            SoundManager.instance.PlaySound("get_honeydrop");
        }
        else if(type == ItemType.Dirt){
            StartCoroutine(GetItemAndRemoveCoroutine());
            
            InventoryManager.instance.AddItem(5,3);
            SoundManager.instance.PlaySound(SoundManager.instance.defaultGetItemSoundName);

        }
        else if(type == ItemType.Item){
            StartCoroutine(GetItemAndRemoveCoroutine());
            DM(itemID+"번 아이템 "+amount_item+"개 획득");
            
            if(useDialogue){
                InventoryManager.instance.AddItem(itemID);

            }
            else{
                InventoryManager.instance.AddItem(itemID,activateDialogue:true);

            }

            
        }

        if(!DBManager.instance.curData.getItemOverList.Contains(objectID)){
            DBManager.instance.curData.getItemOverList.Add(objectID);
        }

        if(useDialogue){
            
            PlayerManager.instance.isActing =true;  
            
            PlayerManager.instance.SetLockPlayer(true);
            PlayerManager.instance.SetTalkCanvasDirection();
            if(GameManager.instance.mode_zoomWhenInteract){

                SceneController.instance.SetCameraDefaultZoomIn();
                SceneController.instance.SetSomeConfiner(SceneController.instance.mapZoomBounds[DBManager.instance.curData.curMapNum]);
            }
            UIManager.instance.SetHUD(false);
            SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0;
            SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0;
            //트리거 시작시 배경음 감소
            SoundManager.instance.bgmPlayer.volume *= DBManager.instance.bgmFadeValueInTrigger;
//            Debug.Log(getItemDialogue.sentences[1]);
            DialogueManager.instance.SetDialogue(getItemDialogue);
            yield return waitTalking;

            UIManager.instance.SetHUD(true);

            if(GameManager.instance.mode_zoomWhenInteract){
                SceneController.instance.SetCameraDefaultZoomOut();
                SceneController.instance.SetSomeConfiner(SceneController.instance.mapBounds[DBManager.instance.curData.curMapNum]);
            }

            SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 2;
            SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 2;
            //트리거 완료 혹은 재시작 가능 시 느낌표 재출력을 위해 로케이션 레이더 재활성화
            PlayerManager.instance.locationRader.ResetLocationRader();


            PlayerManager.instance.isActing =false;  
            if(tutorialIds.Length == 0) PlayerManager.instance.SetLockPlayer(false);
            if(itemID==21) UIManager.instance.AcceptQuest(11);
            
        }

        if(tutorialIds.Length != 0){
            yield return wait100ms;
            yield return wait100ms;
            yield return wait100ms;
            for(int i=0;i<tutorialIds.Length;i++){
                UIManager.instance.OpenTutorialUI(tutorialIds[i]);
                yield return new WaitUntil(()=>!UIManager.instance.waitTutorial);
                //yield return wait100ms;
                //yield return wait100ms;
                if(tutorialIds[i]==0){
                    UIManager.instance.AcceptQuest(12);
                }
            }
        }
        
        SoundManager.instance.bgmPlayer.volume = DBManager.instance.localData.bgmVolume;//MenuManager.instance.slider_bgm.value;
        
        if(!isPopMode)
            itemObject.gameObject.SetActive(false);
        else{
            gameObject.SetActive(false);
        }
    }

    IEnumerator GetItemAndRemoveCoroutine(){
        yield return null;
        itemObject.GetComponent<SpriteRenderer>().color= new Color(1,1,1,0);
        if(itemObject.TryGetComponent<PolygonCollider2D>(out PolygonCollider2D col)){
            col.enabled = false;
        }
    }

    public void SetItemAvailable(){
        isAvailable = true;
    }
    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);
}
