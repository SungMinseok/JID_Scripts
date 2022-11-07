using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;



[System.Serializable]
public class ItemSlot{
    public Button itemSlotBtn;
    public GameObject equippedMark;
    public Text itemAmount;
    public Image itemImage;
    public GameObject itemDescriptionWindow;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public GameObject itemUseableMark;
    public GameObject itemUseableBox;
    
    //public ItemSlot(GameObject a)
}
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    //public UIManager theUI;
    
    [Header("UI_Inventory───────────────")]
    public Transform itemSlotGrid;
    public ItemSlotScript[] itemSlotScripts;
    //[SerializeField]
    //public ItemSlot[] itemSlot;
    //public ItemSlot[] temp;
    public GameObject upArrow, downArrow;
   // public Sprite[] itemSprites;
    public Sprite nullSprite;
    public Animator inventoryAnimator;
    public Button toggleBtn;
    public Image movingItemImage;
    public Queue<int> getItemAlertQueue;
    public Queue<int> getHoneyAmountQueue;
    [Header("Debug───────────────")]
    public int curPage;
    public int slotCountPerPage;
    public bool isHide;
    public bool selectFlag;
    public bool waitGetItemDialogue;//트루 시, 트리거 종료 후에도 이동 불가
    Dialogue tempDialogue;
    public bool itemMoveAccepted;//true 시(드래그 성공 시 트루), 인벤 리셋
    public int itemMoveEndIndex;
    public bool itemIsMoving;
    public bool inventoryTabHovering;
    WaitForSecondsRealtime waitForSecondsRealtime ; 
    WaitForSeconds wait1s;
    public bool getItemAlertQueueFlag;
    void Awake(){
        instance = this;

        getItemAlertQueue = new Queue<int>();
        getHoneyAmountQueue = new Queue<int>();
        
    }
    void Update(){
        if(getItemAlertQueue.Count!=0 /* && !getItemAlertQueueFlag */){
            //getItemAlertQueueFlag = true;
            StartCoroutine(GetItemHudAlert(getItemAlertQueue.Dequeue()));
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //itemSlot = new ItemSlot[7];
        // for(int i=0;i<slotCountPerPage;i++){
        //     itemSlot[i].equippedMark = PlayerManager.instance.gameObject;
        //     itemSlot[i].itemSlotBtn = itemSlotGrid.GetChild(i).GetComponent<Button>();
        //     itemSlot[i].equippedMark = itemSlotGrid.GetChild(i).GetChild(0).gameObject;
        //     itemSlot[i].itemAmount = itemSlotGrid.GetChild(i).GetChild(2).GetComponent<Text>();
        //     itemSlot[i].itemImage = itemSlotGrid.GetChild(i).GetChild(1).GetComponent<Image>();
        //     itemSlot[i].itemDescriptionWindow = itemSlotGrid.GetChild(i).GetChild(3).gameObject;
        //     itemSlot[i].itemName = itemSlotGrid.GetChild(i).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
        //     itemSlot[i].itemDescription = itemSlotGrid.GetChild(i).GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>();
        //     itemSlot[i].itemUseableMark = itemSlotGrid.GetChild(i).GetChild(3).GetChild(2).gameObject;
        //     itemSlot[i].itemUseableBox = itemSlotGrid.GetChild(i).GetChild(3).GetChild(3).gameObject;
        // }

        for(int i=0;i<itemSlotGrid.transform.childCount;i++){
            int temp = i;
            itemSlotGrid.GetChild(temp).GetComponent<Button>().onClick.AddListener(()=>PushItemBtn(temp));
        }        

        if(DBManager.instance.localData.itemCollectionOverList.Count == 0
        && DBManager.instance.curData.itemList.Count != 0
        ){
            for(int i=0; i<DBManager.instance.curData.itemList.Count; i++){
                int tempID = DBManager.instance.curData.itemList[i].itemID;
                DBManager.instance.localData.itemCollectionOverList.Add(tempID);
            }
        }

        ResetInventory();
        ChangeDirtItemPostion(DBManager.instance.dirtOnlyHUD);
        if(DBManager.instance.dirtOnlyHUD){
            UIManager.instance.RefreshDirtHolder();
        }

        wait1s = new WaitForSeconds(1f);
        
    }

    // Update is called once per frame
    // void Update()
    // {
    //     for(int i=1;i<12;i++){
    //         if(Input.GetKeyDown((KeyCode)(48+i))){
    //             PushItemBtn(i-1);
    //         }
    //     }
    //     if(Input.GetKeyDown(KeyCode.Minus)){
    //         PushItemBtn(10);
    //     }
    //     else if(Input.GetKeyDown(KeyCode.Alpha0)){
    //         PushItemBtn(9);
    //     }
    // }
    public void ResetInventory(){

        curPage = 0;
        RefreshInventory(0);
    }
    
    public void AddItem(int ID, int amount = 1, bool activateDialogue = false, float delayTime = 0, int tutorialID = -1, bool mute = false){
        if(amount<1) return;

        if(activateDialogue){
            //waitGetItemDialogue = true;
            //StartCoroutine(GetItemDialogue(ID, delayTime:delayTime, tutorialID:tutorialID));
            EnqueueGetItemID(ID);

        }

        if(!DBManager.instance.localData.itemCollectionOverList.Contains(ID)){
            DBManager.instance.localData.itemCollectionOverList.Add(ID);
        }

        if(!mute){

            switch(ID){
                case 15 : 
                    SoundManager.instance.PlaySound("paperflip");
                    break;
                case 30 : 
                case 31 : 
                    SoundManager.instance.PlaySound("get_item_ice");
                    break;
                default :   
                    SoundManager.instance.PlaySound(SoundManager.instance.defaultGetItemSoundName);
                    break;
            }
        }
        //DBManager.instance.curData.itemList.Add(ID);
        //스택형 아이템이면
        if(DBManager.instance.cache_ItemDataList[ID].isStack){
            //if(DBManager.instance.curData.itemList.Contains(0))
            for(var i=0;i< DBManager.instance.curData.itemList.Count;i++){
                //인벤토리에 이미 있으면 개수만 추가해줌
                if(DBManager.instance.curData.itemList[i].itemID == ID){
                    DBManager.instance.curData.itemList[i].itemAmount += amount;
                        RefreshInventory(curPage);

                    // if(ID==5 && DBManager.instance.dirtOnlyHUD){
                    //     //DBManager.instance.curData.itemList[i].itemAmount += amount;
                    //     ChangeDirtItemPostion(DBManager.instance.dirtOnlyHUD);

                    // }
                    // else{

                    // }
//                    Debug.Log("a");
                    return;
                }
            }
            //Debug.Log("b");
                    DBManager.instance.curData.itemList.Add(new ItemList(ID,amount));
            
            //220816 추가 흙전용 HUD용으로 이전
            //if(DBManager.instance.dirtOnlyHUD){
                if(ID==5){
                    if(DBManager.instance.dirtOnlyHUD){
                        ChangeDirtItemPostion(DBManager.instance.dirtOnlyHUD);
                    }
                    else{
                        RemoveItem(ID,amount);
                        DBManager.instance.curData.itemList.Insert(0, new ItemList(ID,amount));
                        RefreshInventory(curPage);
                    }
                }
                else{

                    //인벤토리에 없으면 새로 추가해줌
                }
            //}
        }
        //스택형 아이템이 아니면
        else{
            for(var i=0;i<amount;i++){
                DBManager.instance.curData.itemList.Add(new ItemList(ID,1));
                    //Debug.Log("c");
            }
        }

        //220903 미니맵 버튼 활성화
        if(ID==12) UIManager.instance.hud_sub_map.GetComponent<Button>().interactable = true;



        RefreshInventory(curPage);
    }
    public void RemoveItem(int ID, int amount = 1){
        //DBManager.instance.curData.itemList.Remove(ID);
        var myItemList = DBManager.instance.curData.itemList;
        int curItemIndex = myItemList.FindIndex(x => x.itemID == ID);

        if(curItemIndex == -1){
            Debug.Log(string.Format("{0}번 아이템 없음",ID));
            return;
        }

        //Debug.Log(curItemIndex);

        // for (var i = curItemList.Count - 1; i >= 0; i--) { // reverse iteration
        //     if (curItemList[i].itemID == ID) {
        //         curItemList.RemoveAt(i);
        //         break;
        //     }
        // }

        //if(DBManager.instance.cache_ItemDataList[ID].isStack){
        if(myItemList[curItemIndex].itemAmount - amount > 0){
            myItemList[curItemIndex].itemAmount -= amount;
        }
        else{
            myItemList.RemoveAt(curItemIndex);

        }
        //}
        //else{
        //    myItemList.RemoveAt(curItemIndex);
        //}



        RefreshInventory(curPage);

    }
    public bool CheckHaveItem(int ID, int amount = 1){
        List<ItemList> myItemList = DBManager.instance.curData.itemList;

        //if(DBManager.instance.curData.itemList.Exists(x => x.itemID == ID && DBManager.instance.curData.itemList[x].itemAmount >= amount) && DBManager.instance.curData.itemList[ID].itemAmount >= amount){//&& DBManager.instance.curData.itemList.Exists(x => x.itemAmount >= amount)){
        if(myItemList.Exists(x => x.itemID == ID)){// && myItemList[myItemList.FindIndex(y => y.)].itemAmount >= amount){
            if(amount>1){
                if(myItemList[myItemList.FindIndex(y => y.itemID == ID)].itemAmount >= amount){
                    return true;
                }
                else{
                    return false;

                }
            }
            else{

                return true;
            }
        }
        else{
            return false;
        }
    }

    public void RefreshInventory(int pageNum){
        //var theUI = UIManager.instance;
        var theDB = DBManager.instance;

        for(int i= pageNum * slotCountPerPage ; i< (pageNum + 1) * slotCountPerPage ;i++){

            int curSlotNum = i%slotCountPerPage ;

            //아이템 있을 경우
            if(i<theDB.curData.itemList.Count){
                int itemID = DBManager.instance.curData.itemList[i].itemID;

                //아이템 이미지 불러오기
                itemSlotScripts[curSlotNum].itemSlot.itemImage.sprite = theDB.cache_ItemDataList[itemID].icon;

                //아이템 정보 불러오기
                itemSlotScripts[curSlotNum].itemSlot.itemName.text = theDB.cache_ItemDataList[itemID].name;
                itemSlotScripts[curSlotNum].itemSlot.itemDescription.text = theDB.cache_ItemDataList[itemID].description;

                if(theDB.cache_ItemDataList[itemID].isStack){
                    itemSlotScripts[curSlotNum].itemSlot.itemAmount.text = DBManager.instance.curData.itemList[i].itemAmount.ToString();
                    itemSlotScripts[curSlotNum].itemSlot.itemAmount.gameObject.SetActive(true);
                }
                else{
                    itemSlotScripts[curSlotNum].itemSlot.itemAmount.gameObject.SetActive(false);

                }




                //장착 가능 아이템 타입일 경우(1,2,3), 장착한 아이템일 경우 장착 중 표시 여부
                if(theDB.cache_ItemDataList[itemID].type == 1
                ||theDB.cache_ItemDataList[itemID].type == 2
                ||theDB.cache_ItemDataList[itemID].type == 3              
                ){
                    for(var j=0;j<PlayerManager.instance.equipments_id.Length;j++){
                        if(PlayerManager.instance.equipments_id[j]==itemID){
                            itemSlotScripts[curSlotNum].itemSlot.equippedMark.SetActive(true);
                            break;
                        }
                        itemSlotScripts[curSlotNum].itemSlot.equippedMark.SetActive(false);
                    }
                }
                //삽, 곡괭이 자동 장착 ON
                else if(theDB.cache_ItemDataList[itemID].type == 6){

                    itemSlotScripts[curSlotNum].itemSlot.equippedMark.SetActive(true);
                }
                else{
                    itemSlotScripts[curSlotNum].itemSlot.equippedMark.SetActive(false);

                }

                
                //사용 가능아이템 처리 220821
                if(theDB.cache_ItemDataList[itemID].type == 1
                ||theDB.cache_ItemDataList[itemID].type == 2
                ||theDB.cache_ItemDataList[itemID].type == 4
                ||theDB.cache_ItemDataList[itemID].type == 5
                ){
                    itemSlotScripts[curSlotNum].itemSlot.itemUseableBox.SetActive(true);
                    itemSlotScripts[curSlotNum].itemSlot.itemUseableMark.SetActive(true);
                    //itemSlot[curSlotNum].equippedMark.SetActive(true);
                }
                else{

                    itemSlotScripts[curSlotNum].itemSlot.itemUseableBox.SetActive(false);
                    itemSlotScripts[curSlotNum].itemSlot.itemUseableMark.SetActive(false);
                }

                //아이템 버튼 활성화
                itemSlotScripts[curSlotNum].itemSlot.itemSlotBtn.interactable = true;
            }
            //아이템 없을 경우
            else{
                //itemSlotGrid.GetChild(curSlotNum).GetComponent<Image>().sprite = nullSprite;
                //Debug.Log(itemSlotScripts[curSlotNum].itemSlot.itemImage.sprite);
                itemSlotScripts[curSlotNum].itemSlot.itemImage.sprite = nullSprite;
                itemSlotScripts[curSlotNum].itemSlot.equippedMark.gameObject.SetActive(false);
                
                itemSlotScripts[curSlotNum].itemSlot.itemAmount.gameObject.SetActive(false);

                //아이템 버튼 비활성화
                itemSlotScripts[curSlotNum].itemSlot.itemSlotBtn.interactable = false;

                //사용 가능아이템 처리 220821
                itemSlotScripts[curSlotNum].itemSlot.itemUseableBox.SetActive(false);
                itemSlotScripts[curSlotNum].itemSlot.itemUseableMark.SetActive(false);

            }
        }
        BtnActivateCheck();
    }
    public void BtnNextPage(){
        if((curPage+1)*slotCountPerPage < DBManager.instance.curData.itemList.Count ){
            RefreshInventory(++curPage );
        }
        else{
            RefreshInventory(0);
            curPage = 0;

        }

        BtnActivateCheck();
    }
    // public void BtnPreviousPage(){
    //     RefreshInventory(--curPage);
    //     BtnActivateCheck();
    // }
    public void BtnActivateCheck(){

        // if(curPage == 0){
        //     upArrow.SetActive(false);
        // }
        // else{
        //     upArrow.SetActive(true);
        // }

        // if(DBManager.instance.curData.itemList.Count > (curPage+1)*slotCountPerPage) {

        //     downArrow.SetActive(true);
        // }
        // else{
            
        //     downArrow.SetActive(false);
        // }
        if(DBManager.instance.curData.itemList.Count < slotCountPerPage) {

            downArrow.SetActive(false);
        }
        else{
            
            downArrow.SetActive(true);
        }
        
    }
    public void PushItemBtn(int slotNum){
        if(PlayerManager.instance.isPlayingMinigame){
            return;
        }

        int curSlotNum = curPage * slotCountPerPage + slotNum;
        
        if(curSlotNum >= DBManager.instance.curData.itemList.Count){
            return;
        }

        Item curItem = DBManager.instance.cache_ItemDataList[DBManager.instance.curData.itemList[curSlotNum].itemID];

        Debug.Log(curSlotNum + "번 슬롯 클릭, itemID : " + curItem.ID + ", name : " + DBManager.instance.cache_ItemDataList[curItem.ID].name);

        // if(curItem.itemType != 0 || curItem.itemType != 4){
        //     itemSlot[slotNum].gameObject.SetActive(!itemSlot[slotNum].gameObject.activeSelf);
        // }
        UseItem(curItem);
    }
    public void UseItem(Item curItem){
        
        if(curItem.type==0){
            return;
        }

        var theDB = DBManager.instance;
        
        switch(curItem.type){   // 0:기타, 1:헬멧, 2:옷, 3:무기, 4.소모품, 5:특수(사용)
            case 1 : 
            case 2 : 
            case 3 : 
                PlayerManager.instance.SetEquipment(curItem.type, curItem.ID);
                SoundManager.instance.PlaySound("item_use");
                break;
            // case 3 : 
            //     PlayerManager.instance.SetEquipment(curItem.type, curItem.ID);
            //     break;
            case 4 :
                if(curItem.isStack){

                    var curItemListIndex = theDB.curData.itemList.FindIndex(x => x.itemID == curItem.ID);

                    if(curItem.ID == 5){

                        if(theDB.curData.itemList[curItemListIndex].itemAmount > 0){
                            theDB.curData.itemList[curItemListIndex].itemAmount -= 1;
                            AddDirt(DBManager.instance.defaultGetDirtAmount);
                        }
                    }

                    else if(curItemListIndex != -1){
                        if(theDB.curData.itemList[curItemListIndex].itemAmount > 1){
                            theDB.curData.itemList[curItemListIndex].itemAmount -= 1;
                        }
                        else{
                            RemoveItem(curItem.ID);
                        }
                    }

                    // for(var i=0;i< theDB.curData.itemList.Count;i++){
                    //     //인벤토리에 이미 있으면 개수만 빼줌
                    //     if(theDB.curData.itemList[i].itemID == curItem.ID){
                            
                    //         if(curItem.ID!=5){

                    //         }
                    //         else{
                    //         //220823 흙은 0으로 표시
                                
                    //         }
                    //         RefreshInventory(curPage);
#region 아이템 별 사용 내용
                        switch(curItem.ID){
                            case 5:
                                //SoundManager.instance.PlaySound(SoundManager.inst);dirt_charge
                                if(theDB.curData.itemList[curItemListIndex].itemAmount!=0)
                                    SoundManager.instance.PlaySound("dirt_charge");
                                break;
                            
                            default :
                                SoundManager.instance.PlaySound("item_use");
                                break;
                        }
#endregion      
                }
                    
                
                break;
            case 5:
                // switch(curItem.ID){
                //     case 34:
                        if(!selectFlag){
                            selectFlag = true;
                            StartCoroutine(SetSelectByUsingItem(curItem.ID));
                        }
                        // break;
                // }
                break;
            default :
                break;
        }

        SetQuestState(curItem.ID);
        RefreshInventory(curPage);

    }
    public void PushToggleBtn(){
        if(isHide){
            isHide = !isHide;
            inventoryAnimator.SetTrigger("open");
            toggleBtn.interactable = false;
            Invoke("ActivateToggleBtn", 1.34f);
        }
        else{
            isHide = !isHide;
            inventoryAnimator.SetTrigger("close");
            toggleBtn.interactable = false;
            Invoke("ActivateToggleBtn", 1.34f);
        }
    }
    public void ActivateToggleBtn(){
        toggleBtn.interactable = true;
    }
    public void AddDirt(float dirtAmount){
        DBManager.instance.curData.curDirtAmount+=dirtAmount;
        if(DBManager.instance.curData.curDirtAmount>DBManager.instance.maxDirtAmount){
            DBManager.instance.curData.curDirtAmount=DBManager.instance.maxDirtAmount;
        }

        DBManager.instance.localData.useDirtCount += 1;
        if(DBManager.instance.localData.useDirtCount >= 100){
            SteamAchievement.instance.ApplyAchievements(16);
        }
        
        SoundManager.instance.PlaySound(SoundManager.instance.defaultAddDirtName);
    }
    public void AddHoney(int honeyAmount, bool activateDialogue = false, float delayTime = 0){
        DBManager.instance.curData.curHoneyAmount += honeyAmount;
        //Debug.Log(DBManager.instance.curData.curHoneyAmount + honeyAmount);

        if(DBManager.instance.curData.curHoneyAmount + honeyAmount > 1000){
            SteamAchievement.instance.ApplyAchievements(1);
        }


        if(activateDialogue){
            EnqueueGetItemID(999, honeyAmount:honeyAmount);

            //waitGetItemDialogue = true;
            //StartCoroutine(GetItemDialogue(999, honeyAmount:honeyAmount ,delayTime:delayTime));
        }
    }
    IEnumerator SetSelectByUsingItem(int itemID){

        PlayerManager.instance.LockPlayer();
        PlayerManager.instance.isActing = true;
            //PlayerManager.instance.rb.AddForce(Vector2.zero);


        Select tempSelect = new Select();
        Dialogue tempDialogue = new Dialogue();

        switch(itemID){
            case 0:
                UIManager.instance.OpenScreen(2);
                yield return new WaitUntil(()=>!UIManager.instance.screenOn);
                SoundManager.instance.PlaySound("hintpaper_open");
                break;
            case 33 ://권총
                tempSelect.answers = new string[2]{"161","162"};
                SelectManager.instance.SetSelect(tempSelect,null);
                Array.Clear(tempSelect.answers,0,tempSelect.answers.Length);
                yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                if(SelectManager.instance.GetSelect()==0){
                    if(PlayerManager.instance.flagID == 1){
                        PlayerManager.instance.weapons[2].gameObject.SetActive(true);
                        PlayerManager.instance.animator.SetBool("smallgun",true);
                        yield return wait1s;
                        yield return wait1s;
                        UIManager.instance.SetGameEndUI(11);
                        yield return wait1s;
                    }
                    else if(DBManager.instance.curData.curMapNum==23){
                        PlayerManager.instance.weapons[2].gameObject.SetActive(true);
                        PlayerManager.instance.animator.SetBool("smallgun",true);
                        yield return wait1s;
                        yield return wait1s;
                        UIManager.instance.SetGameEndUI(12);
                        yield return wait1s;
                    }
                    else{
                        tempDialogue.sentences = new string[1]{"863"};
                        tempDialogue.isMonologue = true;
                        DialogueManager.instance.SetDialogue(tempDialogue);
                        Array.Clear(tempDialogue.sentences,0,tempDialogue.sentences.Length);
                        yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                    }
                }
        //PlayerManager.instance.UnlockPlayer();
                break;
            case 34 ://소총(기관총)
                tempSelect.answers = new string[2]{"161","162"};
                SelectManager.instance.SetSelect(tempSelect,null);
                Array.Clear(tempSelect.answers,0,tempSelect.answers.Length);
                yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                if(SelectManager.instance.GetSelect()==0){
                    if(DBManager.instance.curData.curMapNum==8){
                        PlayerManager.instance.weapons[3].gameObject.SetActive(true);
                        PlayerManager.instance.animator.SetBool("biggun",true);
                        yield return wait1s;
                        yield return wait1s;
                        UIManager.instance.SetGameEndUI(10);
                        yield return wait1s;
                    }
                    else{
                        tempDialogue.sentences = new string[1]{"863"};
                        tempDialogue.isMonologue = true;
                        DialogueManager.instance.SetDialogue(tempDialogue);
                        Array.Clear(tempDialogue.sentences,0,tempDialogue.sentences.Length);
                        yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                    }
                }
        //PlayerManager.instance.UnlockPlayer();
                break;
            case 39 ://거대물약
                tempSelect.answers = new string[2]{"163","164"};
                SelectManager.instance.SetSelect(tempSelect,null);
                Array.Clear(tempSelect.answers,0,tempSelect.answers.Length);
                yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                if(SelectManager.instance.GetSelect()==0){
                    //UIManager.instance.SetMovieEffectUI(true);
                    PlayerManager.instance.animator.SetTrigger("drink");
                    SoundManager.instance.PlaySound("drinking_sth");
                    yield return new WaitForSeconds(2.1f);

                    // tempDialogue.sentences = new string[3];
                    // tempDialogue.sentences[0] = "1062";
                    // tempDialogue.sentences[1] = "1063";
                    // tempDialogue.sentences[2] = "1064";
                    tempDialogue.sentences = new string[3]{"1062","1063","1064"};
                    tempDialogue.isMonologue = true;
                    DialogueManager.instance.SetDialogue(tempDialogue,null);
                    yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                    Array.Clear(tempDialogue.sentences,0,tempDialogue.sentences.Length);

                    PlayerManager.instance.SetMainBodySize(1.25f,0.01f);
                    SceneController.instance.SetCameraNoised(5,5);
                    SoundManager.instance.PlaySound("changing_body_drug");
                    PlayerManager.instance.animator.SetBool("shake", true);
                    yield return new WaitForSeconds(3f);
                    UIManager.instance.SetGameEndUI(9);
                    // if(DBManager.instance.curData.curMapNum==8){
                    // }
                    // else{
                    //     tempDialogue.sentences = new string[1]{"863"};
                    //     tempDialogue.isMonologue = true;
                    //     DialogueManager.instance.SetDialogue(tempDialogue);
                    //     yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                    // }
                }
        //PlayerManager.instance.UnlockPlayer();
                break;

                
            case 46:
                RemoveItem(46);
                SoundManager.instance.PlaySound("item_use");
                AddHoney(10000);
                AddItem(40);
                AddItem(41,mute:true);
                AddItem(42,mute:true);
                AddItem(43,mute:true);
                AddItem(44,mute:true);
                AddItem(45,mute:true);
                AddItem(48,mute:true);
                break;
            case 47:
                RemoveItem(47);
                SoundManager.instance.PlaySound("item_use");
                AddHoney(10000);
                AddItem(40);
                AddItem(41,mute:true);
                AddItem(42,mute:true);
                AddItem(43,mute:true);
                AddItem(44,mute:true);
                AddItem(45,mute:true);
                AddItem(48,mute:true);
                break;
            case 48:
                RemoveItem(48);
                PlayerManager.instance.SummonPet();
                SoundManager.instance.PlaySound("item_use");
                break;
            case 49:
                RemoveItem(49);
                AddHoney(5000);
                SoundManager.instance.PlaySound("item_use");
                break;
            case 50:
                RemoveItem(50);
                AddHoney(5000);
                SoundManager.instance.PlaySound("item_use");
                break;
            case 51:
                RemoveItem(51);
                PlayerManager.instance.SetBodyColor("5AFF00",30f);
                SoundManager.instance.PlaySound("item_use");
                break;
        }
        if(!PlayerManager.instance.watchingGameEnding && itemID != 48){
            PlayerManager.instance.UnlockPlayer();

        }
        selectFlag = false;
    }
    public void CleanUpInventory(){
        DBManager.instance.curData.itemList.Clear();
        RefreshInventory(0);
    }

//itemID = 999 : 꿀
    IEnumerator GetItemDialogue(int itemID, int honeyAmount = 0, float delayTime = 0f, int tutorialID = -1){
        
        UIManager.instance.hud_block.SetActive(true);
        if(delayTime != 0){
            yield return new WaitForSeconds(delayTime);

        }

        PlayerManager.instance.LockPlayer();
        PlayerManager.instance.isActing = true;
        PlayerManager.instance.SetTalkCanvasDirection();

        string curItemName = "";
        string sentenceID = "123";

        if(itemID != 999){
            curItemName = DBManager.instance.cache_ItemDataList[itemID].name;
        }
        else{
            //꿀
            curItemName = honeyAmount.ToString();//CSVReader.instance.GetIndexToString(15,"sysmsg");
            sentenceID = "124";
        }

        if(DBManager.instance.language == "kr") curItemName = GetCompleteWorld(curItemName);

        Debug.Log(curItemName);
        tempDialogue = new Dialogue();
        tempDialogue.sentences = new string[1]{sentenceID};
        tempDialogue.isMonologue = true;

        DialogueManager.instance.SetDialogue(tempDialogue,curItemName);
        Array.Clear(tempDialogue.sentences,0,tempDialogue.sentences.Length);
        yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);

        if(tutorialID != -1){
            UIManager.instance.OpenTutorialUI(tutorialID);
            yield return new WaitUntil(()=>!UIManager.instance.waitTutorial);
        }
        PlayerManager.instance.UnlockPlayer();
        PlayerManager.instance.isActing = false;
        waitGetItemDialogue = false;

        //상호작용 키 연타로 인한 트리거 즉시 재시작 방지
        PlayerManager.instance.ActivateWaitInteract(PlayerManager.instance.delayTime_WaitingInteract);
        UIManager.instance.hud_block.SetActive(false);

    }

    /// type 0 : 을/를
    /// type 1 : 이/가
    /// <summary>
    /// <param name="type"> 0 : 을/를, 1 : 이/가 </param>
    /// </summary>
    public string GetCompleteWorld(string name, int type = 0) {
        char lastName = name.ElementAt(name.Length - 1);
        int index = (lastName - 0xAC00) % 28;
        Console.WriteLine(index);
        //한글의 제일 처음과 끝의 범위 밖일경우 에러 
        if (lastName < 0xAC00 || lastName > 0xD7A3) {
            return name;
        }

        
        string selectVal = "";
        
        if(type==0){
            selectVal = (lastName - 0xAC00) % 28 > 0 ? "을" : "를";

        }
        else if(type==1){
            selectVal = (lastName - 0xAC00) % 28 > 0 ? "이" : "가";

        }

        return name + selectVal;
    }

    public void GiveReward(){
        if(DBManager.instance.localData.usedCouponRewardItemID!=0)//0번아이템 있는데 예외처리 안했음;;
            InventoryManager.instance.AddItem(DBManager.instance.localData.usedCouponRewardItemID, mute: true);
    }
             
    public void ChangeDirtItemPostion(bool active){

        var myItemList = DBManager.instance.curData.itemList;
        int curItemIndex = myItemList.FindIndex(x => x.itemID == 5);
        
        //Debug.Log("curItemAmount " + curItemAmount);
        //Debug.Log("theDB.cache_ItemDataList " + DBManager.instance.cache_ItemDataList.Count);

        //흙 첫번째 고정하기
        if(!active){
            //if(DBManager.instance.curData.curDirtItemCount == 0) return;
            UIManager.instance.dirtHolder.SetActive(false);

            if(!DBManager.instance.CheckTrigOver(1)) return;

            var curDirtIndexInItemList = DBManager.instance.curData.itemList.FindIndex(x=>x.itemID==5);

            if(curDirtIndexInItemList!=-1){
                var curDirtAmount = DBManager.instance.curData.itemList[curDirtIndexInItemList].itemAmount;

                InventoryManager.instance.RemoveItem(5,curDirtAmount);
                DBManager.instance.curData.itemList.Insert(0, new ItemList(5, curDirtAmount));
                DBManager.instance.curData.curDirtItemCount = 0;
            }

            else{

                DBManager.instance.curData.itemList.Insert(0, new ItemList(5, DBManager.instance.curData.curDirtItemCount));
                DBManager.instance.curData.curDirtItemCount = 0;
            }

            RefreshInventory(curPage);
        }
        //흙 전용 HUD 활성화
        else{

            if(curItemIndex != -1){
                    
                int curItemAmount = myItemList[curItemIndex].itemAmount;

                DBManager.instance.curData.curDirtItemCount += curItemAmount;
                RemoveItem(5,curItemAmount);
            }

                UIManager.instance.RefreshDirtHolder();
        }


        //if(curItemIndex!=-1){
        //}
    }

    public void ChangeItemSlotPosition(int startIndex, int endIndex){//출발지 > 선택지

        var startItemID = DBManager.instance.curData.itemList[startIndex].itemID;
        var startItemAmount = DBManager.instance.curData.itemList[startIndex].itemAmount;

        var endItemID = DBManager.instance.curData.itemList[endIndex].itemID;
        var endItemAmount = DBManager.instance.curData.itemList[endIndex].itemAmount;


        RemoveItem(startItemID, startItemAmount);
        DBManager.instance.curData.itemList.Insert(endIndex, new ItemList(startItemID, startItemAmount));
        RemoveItem(endItemID, endItemAmount);
        DBManager.instance.curData.itemList.Insert(startIndex, new ItemList(endItemID, endItemAmount));

        RefreshInventory(curPage);

        SoundManager.instance.PlaySound("button0");

    }
    public IEnumerator InventoryTabHoveringCoroutine(){
        //inventoryTabHoveringTime = 0;
        Debug.Log("34134");

        while(true){
            yield return wait1s ;
            //yield return wait1s ;
            //inventoryTabHoveringTime += 1;

                //if(inventoryTabHoveringTime>=1){
            BtnNextPage();
        }
        //}
    }
    public void EnqueueGetItemID(int itemID , int honeyAmount = 0){
        getItemAlertQueue.Enqueue(itemID);
        if(honeyAmount!=0) getHoneyAmountQueue.Enqueue(honeyAmount);
        Debug.Log("44");
    }
    IEnumerator GetItemHudAlert(int itemID, int honeyAmount = 0, float delayTime = 0f, int tutorialID = -1){
        
        if(delayTime != 0){
            yield return new WaitForSeconds(delayTime);
        }
        
        string curItemName = "";
        string sentenceID = "17";

        if(itemID != 999){
            curItemName = DBManager.instance.cache_ItemDataList[itemID].name;
        }
        else{
            //꿀
            curItemName = getHoneyAmountQueue.Dequeue().ToString();//CSVReader.instance.GetIndexToString(15,"sysmsg");
            sentenceID = "18";
        }//<color=#FEDC2D>{0}</color>#D2C0A7

        if(DBManager.instance.language == "kr") curItemName = GetCompleteWorld(curItemName);


        if(itemID != 999){
            curItemName = "<color=#D2C0A7>" + curItemName + "</color>";
        }
        else{
            //꿀
            curItemName = "<color=#FEDC2D>" + curItemName + "</color>";
        }//<color=#FEDC2D>{0}</color>#D2C0A7

        UIManager.instance.hud_alert_item_text.text = string.Format(CSVReader.instance.GetIndexToString(int.Parse(sentenceID),"sysmsg"),curItemName);

        UIManager.instance.hud_alert_item.SetTrigger("pop");


    }
    /// <summary>
    /// itemID를 포함한 questState가 있다면 questState 업데이트 해줌.
    /// </summary>
    /// <param name="itemID"></param>
    
    public void SetQuestState(int _itemID){

#region majorType 1 (unused)
        // var tempQuestIDList = //이동한 맵으로 이동시 달성 가능한 QuestInfo의 리스트에서 questID만 뽑은 리스트
        // DBManager.instance.cache_questList.FindAll(x=>x.objectives1.Exists(y=>y.itemID==_itemID)).Select(x=>x.ID).ToList();

        // Debug.Log("tempQuestIDList.Count : " + tempQuestIDList.Count);

        // foreach(QuestState questState in DBManager.instance.curData.questStateList){//현재 QuestStateList에서
            
        //     if(tempQuestIDList.Contains(questState.questID)){//위에서 뽑은 아이디중에 해당되는게 있다면

        //         if(questState.isCompleted) break;//예외) 완료상태면 그냥 패스함

        //         var tempQuestInfo = //뭐가필요한지 몰라서 일단 정보 다가져옴
        //         DBManager.instance.cache_questList.Find(x=>x.ID==questState.questID);

        //         var tempItemList = tempQuestInfo.objectives1;

        //         foreach(ItemList itemList in tempItemList){

        //             if(itemList.itemID != _itemID) break;

        //             if(tempQuestInfo.targetVal > 1){

        //                 questState.progress ++;
        //                 questState.progressList.Add(_itemID);

        //                 var slotIndex = UIManager.instance.curQuestIdList.IndexOf(questState.questID);
        //                 UIManager.instance.SetQuestSlotGrid(slotIndex);//그 슬롯만 상태변경해줌.
                        
        //                 if(questState.progress >= tempQuestInfo.targetVal){
        //                     UIManager.instance.CompleteQuest(questState.questID);
        //                 }
        //             }
        //             else{
        //                 UIManager.instance.CompleteQuest(questState.questID);

        //             }

        //         }

        //     }
        // }
        #endregion
   
   
        var tempQuestIDList = //QuestInfo의 리스트에서 조건에 맞는 questID만 뽑은 리스트
        DBManager.instance.cache_questList.FindAll(x=> x.majorType == 4 && x.objectives0.Contains(_itemID) ).Select(x=>x.ID).ToList();

        //Debug.Log("tempQuestIDList.Count : " + tempQuestIDList.Count);

        foreach(QuestState questState in DBManager.instance.curData.questStateList){//현재 QuestStateList에서
            
            if(tempQuestIDList.Contains(questState.questID)){//위에서 뽑은 아이디중에 해당되는게 있다면

                if(questState.isCompleted) break;//예외) 완료상태면 그냥 패스함

                var tempQuestInfo = //뭐가필요할지 몰라서 일단 정보 다가져옴
                DBManager.instance.cache_questList.Find(x=>x.ID==questState.questID);
        
                if(tempQuestInfo.targetVal == 1){ // 아이템 1회사용이면 즉시 완료
                    UIManager.instance.CompleteQuest(questState.questID);
                }
                else if(tempQuestInfo.targetVal > 1){

                    //if(questState.progressList.Contains(mapNum)) break;

                    questState.progress ++;
                    questState.progressList.Add(_itemID);

                    var slotIndex = UIManager.instance.curQuestIdList.IndexOf(questState.questID);
                    UIManager.instance.SetQuestSlotGrid(slotIndex);//그 슬롯만 진행도 업데이트 (n/N)
                    
                    if(questState.progress >= tempQuestInfo.targetVal){
                        UIManager.instance.CompleteQuest(questState.questID);
                    }
                }

            }
        }
   
   
   
    }
}
