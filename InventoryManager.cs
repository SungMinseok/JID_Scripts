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
            itemSlot[i].itemAmount = itemSlotGrid.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>();
            itemSlot[i].itemImage = itemSlotGrid.GetChild(i).GetChild(2).GetComponent<Image>();
            itemSlot[i].itemDescriptionWindow = itemSlotGrid.GetChild(i).GetChild(3).gameObject;
            itemSlot[i].itemName = itemSlotGrid.GetChild(i).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
            itemSlot[i].itemDescription = itemSlotGrid.GetChild(i).GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>();
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

    public void AddItem(int ID){
        DBManager.instance.curData.itemList.Add(ID);
        RefreshInventory(curPage);
    }
    public void RemoveItem(int ID){
        DBManager.instance.curData.itemList.Remove(ID);
        RefreshInventory(curPage);

    }
    public void RefreshInventory(int pageNum){
        //var theUI = UIManager.instance;
        var theDB = DBManager.instance;

        for(int i= pageNum * slotCountPerPage ; i< (pageNum + 1) * slotCountPerPage ;i++){
            //아이템 있을 경우
            if(i<DBManager.instance.curData.itemList.Count){
                int itemID = DBManager.instance.curData.itemList[i];

                //아이템 이미지 불러오기
                itemSlot[i%slotCountPerPage].itemImage.sprite = theDB.cache_ItemDataList[itemID].icon;

                //아이템 정보 불러오기
                itemSlot[i%slotCountPerPage].itemName.text = theDB.cache_ItemDataList[itemID].name;
                itemSlot[i%slotCountPerPage].itemDescription.text = theDB.cache_ItemDataList[itemID].description;

                //장착한 아이템일 경우 장착 중 표시 여부
                for(var j=0;j<PlayerManager.instance.equipments.Length;j++){
                    if(PlayerManager.instance.equipments[j]==itemID){
                        itemSlot[i%slotCountPerPage].equippedMark.SetActive(true);
                        break;
                    }
                    itemSlot[i%slotCountPerPage].equippedMark.SetActive(false);
                }

                //아이템 버튼 활성화
                itemSlot[i%slotCountPerPage].itemSlotBtn.interactable = true;
            }
            //아이템 없을 경우
            else{
                //itemSlotGrid.GetChild(i%slotCountPerPage).GetComponent<Image>().sprite = nullSprite;
                itemSlot[i%slotCountPerPage].itemImage.sprite = nullSprite;
                itemSlot[i%slotCountPerPage].equippedMark.gameObject.SetActive(false);

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

        Item curItem = DBManager.instance.cache_ItemDataList[DBManager.instance.curData.itemList[curSlotNum]];

        Debug.Log(curSlotNum + "번 슬롯 클릭, itemID : " + curItem.ID + ", name : " + DBManager.instance.cache_ItemDataList[curItem.ID].name);

        // if(curItem.itemType != 0 || curItem.itemType != 4){
        //     itemSlot[slotNum].gameObject.SetActive(!itemSlot[slotNum].gameObject.activeSelf);
        // }
        UseItem(curItem);
    }
    public void UseItem(Item curItem){
        
        if(curItem.type==0||curItem.type ==4){
            return;
        }
        
        switch(curItem.type){   // 0:기타, 1:헬멧, 2:옷, 3:무기, 4.소모품
            case 1 : 
                PlayerManager.instance.SetEquipment(curItem.type, curItem.ID);
                break;
            case 3 : 
                PlayerManager.instance.SetEquipment(curItem.type, curItem.ID);
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
