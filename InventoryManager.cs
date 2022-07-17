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
    
    //public ItemSlot(GameObject a)
}
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    //public UIManager theUI;
    
    [Header("UI_Inventory───────────────")]
    public Transform itemSlotGrid;
    //[SerializeField]
    public ItemSlot[] itemSlot;
    //public ItemSlot[] temp;
    public GameObject upArrow, downArrow;
   // public Sprite[] itemSprites;
    public Sprite nullSprite;
    public Animator inventoryAnimator;
    public Button toggleBtn;
    [Header("Debug───────────────")]
    public int curPage;
    public int slotCountPerPage;
    public bool isHide;
    public bool selectFlag;
    public bool waitGetItemDialogue;//트루 시, 트리거 종료 후에도 이동 불가
    Dialogue tempDialogue;
    void Awake(){
        instance = this;


        
    }
    // Start is called before the first frame update
    void Start()
    {
        //itemSlot = new ItemSlot[7];
        for(int i=0;i<slotCountPerPage;i++){
            //itemSlot[i].equippedMark = PlayerManager.instance.gameObject;
            itemSlot[i].itemSlotBtn = itemSlotGrid.GetChild(i).GetComponent<Button>();
            itemSlot[i].equippedMark = itemSlotGrid.GetChild(i).GetChild(0).gameObject;
            itemSlot[i].itemAmount = itemSlotGrid.GetChild(i).GetChild(2).GetComponent<Text>();
            itemSlot[i].itemImage = itemSlotGrid.GetChild(i).GetChild(1).GetComponent<Image>();
            itemSlot[i].itemDescriptionWindow = itemSlotGrid.GetChild(i).GetChild(3).gameObject;
            itemSlot[i].itemName = itemSlotGrid.GetChild(i).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
            itemSlot[i].itemDescription = itemSlotGrid.GetChild(i).GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>();
        }

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
    
    public void AddItem(int ID, int amount = 1, bool activateDialogue = false, float delayTime = 0, int tutorialID = -1){
        if(amount<1) return;

        if(activateDialogue){
            waitGetItemDialogue = true;
            StartCoroutine(GetItemDialogue(ID, delayTime:delayTime, tutorialID:tutorialID));
        }

        if(!DBManager.instance.localData.itemCollectionOverList.Contains(ID)){
            DBManager.instance.localData.itemCollectionOverList.Add(ID);
        }

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
        //DBManager.instance.curData.itemList.Add(ID);
        //스택형 아이템이면
        if(DBManager.instance.cache_ItemDataList[ID].isStack){
            //if(DBManager.instance.curData.itemList.Contains(0))
            for(var i=0;i< DBManager.instance.curData.itemList.Count;i++){
                //인벤토리에 이미 있으면 개수만 추가해줌
                if(DBManager.instance.curData.itemList[i].itemID == ID){
                    DBManager.instance.curData.itemList[i].itemAmount += amount;
                    RefreshInventory(curPage);
//                    Debug.Log("a");
                    return;
                }
            }
            //인벤토리에 없으면 새로 추가해줌
            DBManager.instance.curData.itemList.Add(new ItemList(ID,amount));
            //Debug.Log("b");
            
        }
        //스택형 아이템이 아니면
        else{
            for(var i=0;i<amount;i++){
                DBManager.instance.curData.itemList.Add(new ItemList(ID,1));
                    //Debug.Log("c");
            }
        }
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
            //아이템 있을 경우
            if(i<theDB.curData.itemList.Count){
                int itemID = DBManager.instance.curData.itemList[i].itemID;

                //아이템 이미지 불러오기
                itemSlot[i%slotCountPerPage].itemImage.sprite = theDB.cache_ItemDataList[itemID].icon;

                //아이템 정보 불러오기
                itemSlot[i%slotCountPerPage].itemName.text = theDB.cache_ItemDataList[itemID].name;
                itemSlot[i%slotCountPerPage].itemDescription.text = theDB.cache_ItemDataList[itemID].description;

                if(theDB.cache_ItemDataList[itemID].isStack){
                    itemSlot[i%slotCountPerPage].itemAmount.text = DBManager.instance.curData.itemList[i].itemAmount.ToString();
                    itemSlot[i%slotCountPerPage].itemAmount.gameObject.SetActive(true);
                }
                else{
                    itemSlot[i%slotCountPerPage].itemAmount.gameObject.SetActive(false);

                }




                //장착 가능 아이템 타입일 경우(1,2,3), 장착한 아이템일 경우 장착 중 표시 여부
                if(theDB.cache_ItemDataList[itemID].type == 1
                ||theDB.cache_ItemDataList[itemID].type == 2
                ||theDB.cache_ItemDataList[itemID].type == 3              
                ){
                    for(var j=0;j<PlayerManager.instance.equipments_id.Length;j++){
                        if(PlayerManager.instance.equipments_id[j]==itemID){
                            itemSlot[i%slotCountPerPage].equippedMark.SetActive(true);
                            break;
                        }
                        itemSlot[i%slotCountPerPage].equippedMark.SetActive(false);
                    }
                }
                //삽, 곡괭이 자동 장착 ON
                else if(theDB.cache_ItemDataList[itemID].type == 6){

                    itemSlot[i%slotCountPerPage].equippedMark.SetActive(true);
                }
                else{
                    itemSlot[i%slotCountPerPage].equippedMark.SetActive(false);

                }

                //아이템 버튼 활성화
                itemSlot[i%slotCountPerPage].itemSlotBtn.interactable = true;
            }
            //아이템 없을 경우
            else{
                //itemSlotGrid.GetChild(i%slotCountPerPage).GetComponent<Image>().sprite = nullSprite;
                //Debug.Log(itemSlot[i%slotCountPerPage].itemImage.sprite);
                itemSlot[i%slotCountPerPage].itemImage.sprite = nullSprite;
                itemSlot[i%slotCountPerPage].equippedMark.gameObject.SetActive(false);
                
                itemSlot[i%slotCountPerPage].itemAmount.gameObject.SetActive(false);

                //아이템 버튼 비활성화
                itemSlot[i%slotCountPerPage].itemSlotBtn.interactable = false;
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
                    for(var i=0;i< theDB.curData.itemList.Count;i++){
                        //인벤토리에 이미 있으면 개수만 추가해줌
                        if(theDB.curData.itemList[i].itemID == curItem.ID){

                            if(theDB.curData.itemList[i].itemAmount > 1){
                                theDB.curData.itemList[i].itemAmount -= 1;
                            }
                            else{
                                RemoveItem(curItem.ID);
                            }
                            RefreshInventory(curPage);
#region 아이템 별 사용 내용
                            switch(curItem.ID){
                                case 5:
        
                                    AddDirt(5);
                                    SoundManager.instance.PlaySound("dirt_charge");

                                
                                    break;
                                
                                default :
                                    SoundManager.instance.PlaySound("item_use");
                                    break;
                            }
#endregion








                            
                            return;
                        }
                    }
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
    }
    public void AddHoney(int honeyAmount, bool activateDialogue = false, float delayTime = 0){
        DBManager.instance.curData.curHoneyAmount += honeyAmount;
        
        if(activateDialogue){
            waitGetItemDialogue = true;
            StartCoroutine(GetItemDialogue(999, honeyAmount:honeyAmount ,delayTime:delayTime));
        }
    }
    IEnumerator SetSelectByUsingItem(int itemID){

        PlayerManager.instance.LockPlayer();
        PlayerManager.instance.isActing = true;
            //PlayerManager.instance.rb.AddForce(Vector2.zero);


        Select tempSelect = new Select();
        Dialogue tempDialogue = new Dialogue();

        switch(itemID){
            case 33 ://권총
                tempSelect.answers = new string[2]{"161","162"};
                SelectManager.instance.SetSelect(tempSelect,null);
                Array.Clear(tempSelect.answers,0,tempSelect.answers.Length);
                yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                if(SelectManager.instance.GetSelect()==0){
                    if(PlayerManager.instance.flagID == 1){
                        UIManager.instance.SetGameEndUI(11);
                    }
                    else if(DBManager.instance.curData.curMapNum==23){
                        UIManager.instance.SetGameEndUI(12);
                    }
                    else{
                        tempDialogue.sentences = new string[1]{"863"};
                        tempDialogue.isMonologue = true;
                        DialogueManager.instance.SetDialogue(tempDialogue);
                        Array.Clear(tempDialogue.sentences,0,tempDialogue.sentences.Length);
                        yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                    }
                }
                break;
            case 34 ://소총(기관총)
                tempSelect.answers = new string[2]{"161","162"};
                SelectManager.instance.SetSelect(tempSelect,null);
                Array.Clear(tempSelect.answers,0,tempSelect.answers.Length);
                yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                if(SelectManager.instance.GetSelect()==0){
                    if(DBManager.instance.curData.curMapNum==8){
                        UIManager.instance.SetGameEndUI(10);
                    }
                    else{
                        tempDialogue.sentences = new string[1]{"863"};
                        tempDialogue.isMonologue = true;
                        DialogueManager.instance.SetDialogue(tempDialogue);
                        Array.Clear(tempDialogue.sentences,0,tempDialogue.sentences.Length);
                        yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                    }
                }
                break;
            case 39 ://거대물약
                tempSelect.answers = new string[2]{"163","164"};
                SelectManager.instance.SetSelect(tempSelect,null);
                Array.Clear(tempSelect.answers,0,tempSelect.answers.Length);
                yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                if(SelectManager.instance.GetSelect()==0){
                    SoundManager.instance.PlaySound("drinking_sth");
                    yield return new WaitForSeconds(2f);

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
                break;
        }

        selectFlag = false;
        PlayerManager.instance.UnlockPlayer();
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

    public string GetCompleteWorld(string name) {
        char lastName = name.ElementAt(name.Length - 1);
        int index = (lastName - 0xAC00) % 28;
        Console.WriteLine(index);
        //한글의 제일 처음과 끝의 범위 밖일경우 에러 
        if (lastName < 0xAC00 || lastName > 0xD7A3) {
            return name;
        }

        string selectVal = (lastName - 0xAC00) % 28 > 0 ? "을" : "를";

        return name + selectVal;
    }
             
}
