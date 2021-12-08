using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



[System.Serializable]
public class ItemSlot{
    public Button itemSlotBtn;
    public GameObject equippedMark;
    public TextMeshProUGUI itemAmount;
    public Image itemImage;
    public GameObject itemDescriptionWindow;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    
    //public ItemSlot(GameObject a)
}
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    [Header("Debug───────────────")]
    public int curPage;
    public int slotCountPerPage;
    public bool isHide;
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
            itemSlot[i].itemAmount = itemSlotGrid.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>();
            itemSlot[i].itemImage = itemSlotGrid.GetChild(i).GetChild(1).GetComponent<Image>();
            itemSlot[i].itemDescriptionWindow = itemSlotGrid.GetChild(i).GetChild(3).gameObject;
            itemSlot[i].itemName = itemSlotGrid.GetChild(i).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
            itemSlot[i].itemDescription = itemSlotGrid.GetChild(i).GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>();
        }

        for(int i=0;i<itemSlotGrid.transform.childCount;i++){
            int temp = i;
            itemSlotGrid.GetChild(temp).GetComponent<Button>().onClick.AddListener(()=>PushItemBtn(temp));
        }        

        ResetInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ResetInventory(){

        curPage = 0;
        RefreshInventory(0);
    }

    public void AddItem(int ID, int amount = 1){
        if(amount<1) return;
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
    public void RemoveItem(int ID){
        //DBManager.instance.curData.itemList.Remove(ID);
        var curItemList = DBManager.instance.curData.itemList;

        for (var i = curItemList.Count - 1; i >= 0; i--) { // reverse iteration
            if (curItemList[i].itemID == ID) {
                curItemList.RemoveAt(i);
                break;
            }
        }
        RefreshInventory(curPage);

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




                //장착 가능 아이템 타입일 경우(1), 장착한 아이템일 경우 장착 중 표시 여부
                if(theDB.cache_ItemDataList[itemID].type == 1){
                    for(var j=0;j<PlayerManager.instance.equipments.Length;j++){
                        if(PlayerManager.instance.equipments[j]==itemID){
                            itemSlot[i%slotCountPerPage].equippedMark.SetActive(true);
                            break;
                        }
                        itemSlot[i%slotCountPerPage].equippedMark.SetActive(false);
                    }
                }

                //아이템 버튼 활성화
                itemSlot[i%slotCountPerPage].itemSlotBtn.interactable = true;
            }
            //아이템 없을 경우
            else{
                //itemSlotGrid.GetChild(i%slotCountPerPage).GetComponent<Image>().sprite = nullSprite;
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
        
        switch(curItem.type){   // 0:기타, 1:헬멧, 2:옷, 3:무기, 4.소모품
            case 1 : 
                PlayerManager.instance.SetEquipment(curItem.type, curItem.ID);
                break;
            case 3 : 
                PlayerManager.instance.SetEquipment(curItem.type, curItem.ID);
                break;
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
                            return;
                        }
                    }
                }
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
}
