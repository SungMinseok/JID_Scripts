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
[System.Serializable]
public class ShopSales
{
    public int itemID;
    public int itemAmount;
    public ShopSales(int _itemID, int _itemAmount = 1){
        itemID = _itemID;
        itemAmount = _itemAmount;
    }
}
public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [Space]
    [Header("[Game Objects]─────────────────")]
    public GameObject ui_shop;
    public GameObject ui_shop_title;
    public Image shopIconImage;
    public TextMeshProUGUI shopNameText;
    public Transform shopSlotGrid;
    public ShopSlot[] shopSlots;
    public Sprite[] shopIconSprites;
    [Space]
    public GameObject ui_shop_check;
    public Image check_iconImage;
    public TextMeshProUGUI check_nameText;
    public Image check_goldTypeImage;
    public Text check_priceText;
    public TextMeshProUGUI check_desText;



    [Space]
    [Header("[Debug]─────────────────")]
    public int curShopIconIndex;
    public string curShopName;
    public int curItemPrice;
    //public int[] curShopSalesItemIndexes;
    public ShopSales[] curShopSales;
    public int lastBuyItemIndex;    //마지막 구매 아이템 체크용

    [Space]
    public int selectedSlotNum;
    // public int selectedItemID;
    // public string selectedItemName;
    // public string selectedItemPrice;
    // public Sprite selectedItemIconSprite;
    // public Sprite selectedItemGoldResourceSprite;





    // public int curSelectedNum;
    // public int maxSelectedNum;
    // public int lastSelectedNum;


    // bool revealTextFlag;
    // bool revealTextFlag_NPC;
    // bool dialogueFlag;
    // public bool canSkip;
    // public bool canSkip2;
    // public bool goSkip;
    
    WaitForSeconds wait250ms = new WaitForSeconds(0.25f);
    WaitForSeconds wait100ms = new WaitForSeconds(0.1f);
    
    void Awake()
    {
        instance = this;
    }
    void Start(){
        
        //shopSlotGrid.GetChild(0).GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(()=>CloseShopUI());
        
        for(int i=0;i<shopSlotGrid.childCount-1;i++){
            int temp = i;
            shopSlotGrid.GetChild(temp+1).GetChild(0).GetComponent<Button>().onClick.AddListener(()=>OpenBuyCheckPopUp(temp));
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
    public void OpenShopUI(int shopIconIndex, string shopName, ShopSales[] shopSales, bool shopTitleActive = true){

        PlayerManager.instance.isShopping = true;

        ui_shop_title.SetActive(shopTitleActive);

        ResetShopUI();
        UIManager.instance.SetOnlyHudBlock(true);
        UIManager.instance.SetHUD(true);
        

        DBManager theDB = DBManager.instance;

        //curShopSalesItemIndexes = shopSalesItemIndexes;
        curShopSales = shopSales;

        //curShopIconIndex = shopIconIndex;
        shopIconImage.sprite = shopIconSprites[shopIconIndex];

        curShopName = shopName;
        shopNameText.text = curShopName;

        for(int i=0;i<curShopSales.Length;i++){
            int curShopSalesID = curShopSales[i].itemID;
            int curShopSalesAmount = curShopSales[i].itemAmount;

            shopSlots[i].itemImage.sprite = theDB.cache_ItemDataList[curShopSalesID].icon;
            shopSlots[i].itemNameText.text = theDB.cache_ItemDataList[curShopSalesID].name + " x " +curShopSalesAmount.ToString();
            shopSlots[i].itemPriceText.text = (theDB.cache_ItemDataList[curShopSalesID].price * curShopSalesAmount).ToString();
            shopSlots[i].goldTypeImage.sprite = theDB.cache_ItemDataList[theDB.cache_ItemDataList[curShopSalesID].goldID].icon;

            shopSlotGrid.GetChild(i+1).gameObject.SetActive(true);
        }

        ui_shop.SetActive(true);

        StartCoroutine(SlotAnimationCoroutine(curShopSales.Length));

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
        UIManager.instance.SetOnlyHudBlock(false);
    }
    public void BuyItem(){
        int slotNum = selectedSlotNum;
        
        ui_shop_check.SetActive(false);

        int curHoneyAmount = DBManager.instance.curData.curHoneyAmount;

        curItemPrice = int.Parse(shopSlots[slotNum].itemPriceText.text);

        //아이템 구매에 필요한 재화 구분 (goldResourceID)
        //꿀방울
        if(DBManager.instance.cache_ItemDataList[curShopSales[slotNum].itemID].goldID == 7){
            //구매 성공
            if(curHoneyAmount >= curItemPrice){
                DBManager.instance.curData.curHoneyAmount -= curItemPrice;
                InventoryManager.instance.AddItem(curShopSales[slotNum].itemID,curShopSales[slotNum].itemAmount);
                lastBuyItemIndex = curShopSales[slotNum].itemID;

                DBManager.instance.ItemPurchaseOver(curShopSales[slotNum].itemID);
                //DBManager.instance.ItemPurchaseOver1(curShopSales[slotNum].itemID);
                
                string[] tempArg = new string[1]{
                    shopSlots[slotNum].itemNameText.text
                    //DBManager.instance.cache_ItemDataList[lastBuyItemIndex].name.ToString(),
                };
                MenuManager.instance.OpenPopUpPanel_OneAnswer("89",mainArguments: tempArg);

            }   
            //구매 실패
            else{
                DM("구매 실패 : 골드 부족 - "+ curItemPrice);
                string[] tempArg = new string[1]{
                    InventoryManager.instance.GetCompleteWorld(DBManager.instance.cache_ItemDataList[7].name.ToString(),1),
                };
                MenuManager.instance.OpenPopUpPanel_OneAnswer("90",mainArguments: tempArg);
            }
        }
        //골드 제외 아이템으로 구매가능한 아이템
        else{
            var curGoldID = DBManager.instance.cache_ItemDataList[curShopSales[slotNum].itemID].goldID;

            var tempItemID = DBManager.instance.cache_ItemDataList.FindIndex(x => x.ID == curGoldID);

            if(InventoryManager.instance.CheckHaveItem(tempItemID)){
                InventoryManager.instance.RemoveItem(tempItemID);
                InventoryManager.instance.AddItem(curShopSales[slotNum].itemID,curShopSales[slotNum].itemAmount);
                lastBuyItemIndex = curShopSales[slotNum].itemID;
                
                DBManager.instance.ItemPurchaseOver(curShopSales[slotNum].itemID);

                string[] tempArg = new string[1]{
                    //DBManager.instance.cache_ItemDataList[lastBuyItemIndex].name.ToString(),
                    shopSlots[slotNum].itemNameText.text
                };
                MenuManager.instance.OpenPopUpPanel_OneAnswer("89",mainArguments: tempArg);

            }
            else{
                DM("구매 실패 : "+tempItemID+"번 아이템 부족");
                var neededItemName = DBManager.instance.cache_ItemDataList[tempItemID].name.ToString();
                neededItemName = InventoryManager.instance.GetCompleteWorld(neededItemName,1);
                string[] tempArg = new string[1]{
                    neededItemName,
                };
                MenuManager.instance.OpenPopUpPanel_OneAnswer("90",mainArguments: tempArg);
            }
        }



    }
    public void OpenBuyCheckPopUp(int slotNum){
        selectedSlotNum = slotNum;

        check_iconImage.sprite = shopSlots[slotNum].itemImage.sprite;
        check_nameText.text = shopSlots[slotNum].itemNameText.text;
        check_goldTypeImage.sprite = shopSlots[slotNum].goldTypeImage.sprite;
        check_priceText.text = shopSlots[slotNum].itemPriceText.text;


        check_desText.text = DBManager.instance.cache_ItemDataList[curShopSales[slotNum].itemID].description;

        ui_shop_check.SetActive(true);
    }

    
    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);

}   
