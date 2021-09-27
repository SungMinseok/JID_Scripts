using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Item{
    public int itemID;
    public string itemName;
    public string itemDesc;
    public byte itemType;
    public int resourceID;
    public bool isStack;
    public Sprite itemIcon;
    //public bool isStack;

    public enum ItemType{
        equip,
    }
    public Item(int _itemId, string _itemName, string _itemDesc, byte _itemType, int _resourceID, bool _isStack){
        itemID = _itemId;
        itemName = _itemName;
        itemDesc = _itemDesc;
        itemType = _itemType;
        resourceID = _resourceID;
        itemIcon = InventoryManager.instance.itemSprites[resourceID];



        isStack = _isStack;
    }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public List<int> itemList;  //현재 보유한 아이템 ID 저장
    public int curPage;
    public UIManager theUI;
    
    [Header("UI_Inventory")]
    public Transform itemSlotGrid;
    public Transform[] itemSlot;
    public GameObject upArrow, downArrow;
    public Sprite[] itemSprites;
    public Sprite nullSprite;
    void Awake(){
        instance = this;

        itemSlot = new Transform[itemSlotGrid.childCount];
        for(int i=0;i<itemSlotGrid.childCount;i++){
            itemSlot[i]=itemSlotGrid.GetChild(i);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
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
        itemList.Add(ID);
        RefreshInventory(curPage);
    }
    public void RemoveItem(int ID){
        itemList.Remove(ID);
        RefreshInventory(curPage);

    }
    public void RefreshInventory(int pageNum){
        //var theUI = UIManager.instance;
        var theDB = DBManager.instance;

        for(int i= pageNum * 7 ; i< (pageNum + 1) * 7 ;i++){
            if(i<itemList.Count){
                int itemID = itemList[i];
                //itemSlotGrid.GetChild(i%7).GetComponent<Image>().sprite = theDB.itemDataList[itemList[i]].itemIcon;
                itemSlot[i%7].GetComponent<Image>().sprite = theDB.itemDataList[itemID].itemIcon;
                for(var j=0;j<PlayerManager.instance.equipments.Length;j++){
                    if(PlayerManager.instance.equipments[j]==itemID){
                        itemSlot[i%7].GetChild(0).gameObject.SetActive(true);
                        break;
                    }
                    itemSlot[i%7].GetChild(0).gameObject.SetActive(false);
                }
            }
            else{
                //itemSlotGrid.GetChild(i%7).GetComponent<Image>().sprite = nullSprite;
                itemSlot[i%7].GetComponent<Image>().sprite = nullSprite;
                itemSlot[i%7].GetChild(0).gameObject.SetActive(false);
            }
        }
        BtnActivateCheck();
    }
    public void BtnNextPage(){
        RefreshInventory(++curPage );
        BtnActivateCheck();
    }
    public void BtnPreviousPage(){
        RefreshInventory(--curPage);
        BtnActivateCheck();
    }
    public void BtnActivateCheck(){

        if(curPage == 0){
            upArrow.SetActive(false);
        }
        else{
            upArrow.SetActive(true);
        }

        if(itemList.Count > (curPage+1)*7) {

            downArrow.SetActive(true);
        }
        else{
            
            downArrow.SetActive(false);
        }
        
    }
    public void PushItemBtn(int slotNum){
        int curSlotNum = curPage * 7 + slotNum;
        
        if(curSlotNum >= itemList.Count){
            return;
        }

        Item curItem = DBManager.instance.itemDataList[itemList[curSlotNum]];

        Debug.Log(curSlotNum + "번 슬롯 클릭, itemID : " + curItem.itemID + ", name : " + DBManager.instance.itemDataList[curItem.itemID].itemName);

        // if(curItem.itemType != 0 || curItem.itemType != 4){
        //     itemSlot[slotNum].gameObject.SetActive(!itemSlot[slotNum].gameObject.activeSelf);
        // }
        UseItem(curItem);
    }
    public void UseItem(Item curItem){
        
        if(curItem.itemType==0||curItem.itemType ==4){
            return;
        }
        
        switch(curItem.itemType){   // 0:기타, 1:헬멧, 2:옷, 3:무기, 4.소모품
            case 1 : 
                PlayerManager.instance.SetEquipment(curItem.itemType, curItem.itemID);
                break;
            case 3 : 
                PlayerManager.instance.SetEquipment(curItem.itemType, curItem.itemID);
                break;
            default :
                break;
        }

        
        RefreshInventory(curPage);

    }
}
