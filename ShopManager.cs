using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class ShopSlot
{
    public Image itemImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public Image goldTypeImage;
    
}
public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [Space]
    [Header("[Game Objects]─────────────────")]
    public GameObject ui_shop;
    public Image shopIconImage;
    public TextMeshProUGUI shopNameText;
    public Transform shopSlotGrid;
    public ShopSlot[] shopSlots;
    public Sprite[] shopIconSprites;



    [Space]
    [Header("[Debug]─────────────────")]
    public int curShopIconIndex;
    public string curShopName;
    public int curItemPrice;
    public int[] curShopSalesItemIndexes;
    public int lastBuyItemIndex;    //마지막 구매 아이템 체크용







    public int curSelectedNum;
    public int maxSelectedNum;
    public int lastSelectedNum;


    bool revealTextFlag;
    bool revealTextFlag_NPC;
    bool dialogueFlag;
    public bool canSkip;
    public bool canSkip2;
    public bool goSkip;
    
    WaitForSeconds wait250ms = new WaitForSeconds(0.25f);
    WaitForSeconds wait100ms = new WaitForSeconds(0.1f);
    
    void Awake()
    {
        instance = this;
    }
    void Start(){
        
        shopSlotGrid.GetChild(0).GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(()=>CloseShopUI());
        
        for(int i=0;i<shopSlotGrid.childCount-1;i++){
            int temp = i;
            shopSlotGrid.GetChild(temp+1).GetChild(0).GetChild(5).GetComponent<Button>().onClick.AddListener(()=>BuyItem(temp));
        }
        
        for(int i=0;i<shopSlotGrid.childCount-1;i++){
            shopSlots[i].itemImage = shopSlotGrid.GetChild(i+1).GetChild(0).GetChild(4).GetChild(0).GetComponent<Image>();
            shopSlots[i].itemNameText = shopSlotGrid.GetChild(i+1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            shopSlots[i].itemPriceText = shopSlotGrid.GetChild(i+1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            shopSlots[i].goldTypeImage = shopSlotGrid.GetChild(i+1).GetChild(0).GetChild(3).GetComponent<Image>();
        }
    }
    // public void ResetSelectUI(){
    //     for(int i=0;i<UIManager.instance.ui_select_grid.childCount;i++){
    //         UIManager.instance.ui_select_grid.GetChild(i).gameObject.SetActive(false);
    //         UIManager.instance.ui_select_grid.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    //     }
    //     lastSelectedNum = 0;
    //     curSelectedNum = 0;
    // }

    // public void SetSelect(Select select){

    //     //PlayerManager.instance.LockPlayer();

    //     ResetSelectUI();

    //     for(int i=0;i<select.answers.Length;i++){
    //         UIManager.instance.ui_select_grid.GetChild(i).gameObject.SetActive(true);
    //         UIManager.instance.ui_select_grid.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = select.answers[i];
    //     }

    //     maxSelectedNum = select.answers.Length;

    //     HighlightSelectedAnswer(0);

    //     UIManager.instance.ui_select.SetActive(true);

    //     PlayerManager.instance.ActivateWaitInteract();  //대화 스킵하다가 바로 선택해버리기 방지
        
    //     StartCoroutine(SelectSlotAnimationCoroutine(select));

    //     PlayerManager.instance.isSelecting = true;

    // }

    // public void HighlightSelectedAnswer(int selectedNum){

    //     for(int i=0;i<UIManager.instance.ui_select_grid.childCount;i++){
    //         UIManager.instance.ui_select_grid.GetChild(i).GetChild(1).GetComponent<Image>().sprite = UIManager.instance.non_selected_sprite;
    //         UIManager.instance.ui_select_grid.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().color = UIManager.instance.non_selected_color;
    //     }

    //     UIManager.instance.ui_select_grid.GetChild(selectedNum).GetChild(1).GetComponent<Image>().sprite = UIManager.instance.selected_sprite;
    //     UIManager.instance.ui_select_grid.GetChild(selectedNum).GetChild(0).GetComponent<TextMeshProUGUI>().color = UIManager.instance.selected_color;
    // }
    // public void ExitSelect(){
    //     PlayerManager.instance.isSelecting = false;

    //     lastSelectedNum = curSelectedNum;
        
    //     UIManager.instance.ui_select.SetActive(false);
        
    //     //PlayerManager.instance.UnlockPlayer();
    // }

    // void Update(){
    //     if(PlayerManager.instance.isSelecting){

    //         if(Input.GetButtonDown("Vertical")){
    //             if(Input.GetAxisRaw("Vertical")<0){
    //                 if(curSelectedNum < maxSelectedNum - 1) curSelectedNum ++;
    //                 else curSelectedNum = 0;
                    
    //                 HighlightSelectedAnswer(curSelectedNum);
    //             }
    //             else if(Input.GetAxisRaw("Vertical")>0){
    //                 if(curSelectedNum != 0) curSelectedNum --;
    //                 else curSelectedNum = maxSelectedNum - 1;

    //                 HighlightSelectedAnswer(curSelectedNum);
    //             }
    //         }


    //         if(Input.GetButtonDown("Interact_OnlyKey") && !PlayerManager.instance.isWaitingInteract){
    //             ExitSelect();
    //         }




    //     }

    // }
    // public void PushSelect(int selectNum){
    //     curSelectedNum = selectNum;
    //     ExitSelect();
    // }
    // public int GetSelect(){
    //     return lastSelectedNum;
    // }
    public void ResetShopUI(){
        lastBuyItemIndex = -1;//아무것도 구매하지 않음

        for(int i=1;i<shopSlotGrid.childCount;i++){
            shopSlotGrid.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void OpenShopUI(int shopIconIndex, string shopName, int[] shopSalesItemIndexes){

        PlayerManager.instance.isShopping = true;
        
        ResetShopUI();
        

        DBManager theDB = DBManager.instance;

        curShopSalesItemIndexes = shopSalesItemIndexes;

        //curShopIconIndex = shopIconIndex;
        shopIconImage.sprite = shopIconSprites[shopIconIndex];

        curShopName = shopName;
        shopNameText.text = curShopName;

        for(int i=0;i<shopSalesItemIndexes.Length;i++){

            shopSlots[i].itemImage.sprite = theDB.cache_ItemDataList[shopSalesItemIndexes[i]].icon;
            shopSlots[i].itemNameText.text = theDB.cache_ItemDataList[shopSalesItemIndexes[i]].name;
            shopSlots[i].itemPriceText.text = theDB.cache_ItemDataList[shopSalesItemIndexes[i]].price.ToString();
            shopSlots[i].goldTypeImage.sprite = theDB.cache_ItemDataList[shopSalesItemIndexes[i]].goldIcon;

            shopSlotGrid.GetChild(i+1).gameObject.SetActive(true);
        }

        ui_shop.SetActive(true);

        StartCoroutine(SlotAnimationCoroutine(shopSalesItemIndexes.Length));

    }
    IEnumerator SlotAnimationCoroutine(int slotCount){
        for(int i=0;i<slotCount+1;i++){
            shopSlotGrid.GetChild(i).GetComponent<Animator>().SetTrigger("on");
            yield return wait100ms;
        }
    }
    public void CloseShopUI(){
        PlayerManager.instance.isShopping = false;
        ui_shop.SetActive(false);
    }
    public void BuyItem(int slotNum){
        int curHoneyAmount = DBManager.instance.curData.curHoneyAmount;

        curItemPrice = int.Parse(shopSlots[slotNum].itemPriceText.text);

        //골드로 구매가능한 아이템
        if(DBManager.instance.cache_ItemDataList[curShopSalesItemIndexes[slotNum]].goldResourceID == "Honey Drop"){
            if(curHoneyAmount >= curItemPrice){
                DBManager.instance.curData.curHoneyAmount -= curItemPrice;
                InventoryManager.instance.AddItem(curShopSalesItemIndexes[slotNum]);
                lastBuyItemIndex = curShopSalesItemIndexes[slotNum];
            }   
            else{
                DM("구매 실패 : 골드 부족");
            }
        }
        //골드 제외 아이템으로 구매가능한 아이템
        else{
            var goldTypeIndex = DBManager.instance.cache_ItemDataList[curShopSalesItemIndexes[slotNum]].goldResourceID;

            var tempItemID = DBManager.instance.cache_ItemDataList.FindIndex(x => x.resourceID == goldTypeIndex);

            if(InventoryManager.instance.CheckHaveItem(tempItemID)){
                InventoryManager.instance.RemoveItem(tempItemID);
                InventoryManager.instance.AddItem(curShopSalesItemIndexes[slotNum]);
                lastBuyItemIndex = curShopSalesItemIndexes[slotNum];
            }
            else{
                DM("구매 실패 : "+tempItemID+"번 아이템 부족");
            }
        }



    }

    
    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);

}   
