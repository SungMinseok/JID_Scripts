using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Item{
    public int itemId;
    public string itemName;
    public string itemDesc;
    public int itemType;
    public int resourceID;
    public bool isStack;
    public Sprite itemIcon;
    //public bool isStack;

    public enum ItemType{
        equip,
    }
    public Item(int _itemId, string _itemName, string _itemDesc, int _itemType, int _resourceID, bool _isStack){
        itemId = _itemId;
        itemName = _itemName;
        itemDesc = _itemDesc;
        itemType = _itemType;
        resourceID = _resourceID;
        itemIcon = UIManager.instance.itemSprites[resourceID];



        isStack = _isStack;
    }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public List<int> itemList;
    public int curPage;
    void Awake(){
        instance = this;
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
    public void RefreshInventory(int pageNum){
        var theUI = UIManager.instance;
        var theDB = DBManager.instance;
        // for(int i=0;i<theUI.ItemSlotGrid.childCount;i++){

        // }

        Debug.Log(pageNum + "번 페이지 출력");

        for(int i= pageNum * 7 ; i< (pageNum + 1) * 7 ;i++){
            if(i<itemList.Count){

                theUI.itemSlotGrid.GetChild(i%7).GetComponent<Image>().sprite = theDB.itemDataList[itemList[i]].itemIcon;
                Debug.Log(i+"번 아이템 출력");
                //UIManager.instance.ItemSlotGrid.GetChild(i).GetComponent<Image>().sprite = UIManager.instance.itemSprites[TextLoader.instance.dictionaryItemText[itemList[i]].resourceID];
            }
            else{
                theUI.itemSlotGrid.GetChild(i%7).GetComponent<Image>().sprite = theUI.nullSprite;
                Debug.Log(i+"번 스롯 아이템 없음");

                //Debug.Log(i);
            }

            //if(i>=itemList.Count) break;

        }
        BtnActivateCheck();
    }
    public void BtnNextPage(){
        //curPage ++;
        RefreshInventory(++curPage );
        BtnActivateCheck();
        // if(curPage > 0){
        //     UIManager.instance.upArrow.SetActive(true);
        // }
       // if(itemList.Count> c)
    }
    public void BtnPreviousPage(){
        RefreshInventory(--curPage);
        BtnActivateCheck();
    }
    public void BtnActivateCheck(){

        if(curPage == 0){
            UIManager.instance.upArrow.SetActive(false);
        }
        else{
            UIManager.instance.upArrow.SetActive(true);
        }

        if(itemList.Count > (curPage+1)*7) {

            UIManager.instance.downArrow.SetActive(true);
        }
        else{
            
            UIManager.instance.downArrow.SetActive(false);
        }
        
    }
}
