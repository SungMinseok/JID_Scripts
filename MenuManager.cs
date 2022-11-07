using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
#if !DISABLESTEAMWORKS
using Steamworks;
#endif
using System;

public class MenuManager : MonoBehaviour
{
    public Text versionText;
    public static MenuManager instance;
    [Header("[Game Objects]━━━━━━━━━━━━━━━━━━━━━━━")]
    //public GameObject btnGridObject;
    public GameObject[] panels;
    //public GameObject[] btns;
    [Header("UI_Menu_Main")]
    public GameObject menuPanel;
    //public Color defaultBtnColor;
    //public Color activatedBtnColor;
    public GameObject collectionPanel;
    //public GameObject antCollectionPanel;
    //public GameObject endingCollectionPanel;
    [Header("UI_PopUp")]
    public GameObject popUpPanel;//확인, 취소 버튼 총 2개
    public TextMeshProUGUI[] popUpText; //main, sub, ok, cancel
    public GameObject popUpPanel1;//확인 버튼 한개
    public TextMeshProUGUI[] popUpText1; //main, sub, ok, cancel
    public bool popUpOkayCheck;
    public GameObject popUpOnWork;
    public bool waitPopUpClosed;
    [Header("UI_Save&Load")]
    public GameObject savePanel;
    public GameObject loadPanel;
    public Transform saveSlotGrid;
    //public SaveLoadSlot[] saveSlots;
    public Transform loadSlotGrid;
    //public SaveLoadSlot[] loadSlots;
    [Header("UI_Settings")]
    public GameObject settingPanel;
    public GameObject[] settingPages;
    public Button[] settingMenuBtns;
    public Transform setting_languagePage;
    public Slider slider_sound;
    public Slider slider_bgm;
    public GameObject toggleBtnOff_talkSound;
    public GameObject toggleBtnOn_talkSound;
    public GameObject checkedBtn_fullScreen;
    public GameObject checkedBtn_window;
    public TMP_Dropdown dropdown_resolution;
    public TMP_Dropdown dropdown_frameRate;
    public TMP_Dropdown dropdown_dirtOnlyHUD;
    public bool waitKeyChange;
    public string curChangingKeyName;
    public Text keyText_jump;
    public Text keyText_interact;
    public Text keyText_pet;
    public Text keyText_adddirt;

    // [System.Serializable]
    // public class SaveLoadSlot{
    //     public TextMeshProUGUI saveNameText;
    //     public TextMeshProUGUI saveDateText;
    //     public TextMeshProUGUI slotNumText;
    //     public TextMeshProUGUI itemInfoText0;
    //     public TextMeshProUGUI itemInfoText1;
    // }
    [Header("UI_Collection")]
    public Button[] collectionTabBtns;
    public GameObject[] collectionPages;
    [Header("UI_Collection_Ending")]
    public GameObject endingCollectionPanel;
    public Animator animator;
    public GameObject collectionEndingNumberVessel;
    public Text collectionEndingNumberText;
    public TextMeshProUGUI collectionNameText;
    public Text collectionPlayCountText;
    public Text collectionRateText;
    public Text collectionTimeText;
    public Sprite collectionNullImage;
    public Image[] collectionCardImages;
    public Sprite[] collectionCardSprites;
    public Button[] collectionScrollArrows;
    public GameObject[] collectionCardRedDots;
    public Text collectionCardOrderText;
    public Transform collectionEndingGrid;
    public Image[] collectionEndingGridImages;
    public GameObject[] collectionEndingGridRedDots;
    [Header("UI_Collection_Ant")]
    public GameObject antCollectionPanel;
    public Text antMainNameText;
    public Image antMainImage;
    public GameObject antMainNameTextHolderObj;
    public Transform antStickersMother;
    StickerScript[] antStickers;
    public Sprite[] antSprites;
    [Header("UI_ETC")]
    public Sprite nullSprite;
    public Font[] fonts ;
    [Header("UI_Language")]
    public GameObject languagePanel;
    public Transform languagePanelBtnGrid;
    public TranslateText languagePanelMainText;
    [Header("UI_Coupon")]
    public GameObject ui_coupon;
    public InputField couponInputField;
    [Header("Debug────────────────────")]
    public string curPopUpType;
    public int curSaveNum;
    public int curLoadNum;
    public int totalPage;
    public int curPage;
    public int[] tempCardNum = new int[5]; //sortOrder와 일치

    void Awake(){
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }    
    void Update(){
        if(Input.GetButtonUp("Cancel")){
            if(PlayerManager.instance == null){
                Debug.Log("3333");
                if(popUpOnWork.activeSelf){
                    popUpOnWork.SetActive(false);
                }
                else if(popUpPanel.activeSelf){
                    popUpPanel.SetActive(false);
                }
                else if(popUpPanel1.activeSelf){
                    popUpPanel1.SetActive(false);
                }
                else if(loadPanel.activeSelf){
                    loadPanel.SetActive(false);
                }
                else if(collectionPanel.activeSelf){
                    collectionPanel.SetActive(false);
                }
                else if(settingPanel.activeSelf){
                    settingPanel.SetActive(false);
                }
                // else{
                //     //MenuManager.instance.menuPanel.SetActive(!MenuManager.instance.menuPanel.activeSelf);
                //     MenuManager.instance.ToggleMenuPanel(!MenuManager.instance.menuPanel.activeSelf);
                // }

            }
        }

    }
    void Start(){

        fonts = new Font[2];
        fonts[0] = Resources.Load<Font>("Cafe24Ssurround");
        fonts[1] = Resources.Load<Font>("uzura");

        string[] tempVer = Application.version.Split('-');
        string subText = "";
#if alpha
        subText = "alpha";
#endif
        versionText.text = string.Format("v{0}-{1}{2}",tempVer[0],subText,tempVer[1]);

#region Reset Collection
        //totalPage = DBManager.instance.endingCollectionSprites.Length;
        totalPage = DBManager.instance.cache_EndingCollectionDataList.Count;

        collectionScrollArrows[0].GetComponent<Button>().onClick.AddListener(()=>CollectionScrollRightBtn());
        collectionScrollArrows[1].GetComponent<Button>().onClick.AddListener(()=>CollectionScrollLeftBtn());
        
        RearrangeCardOrder();
        ResetCardOrder();

        collectionEndingGridImages = new Image[totalPage];
        collectionEndingGridRedDots = new GameObject[totalPage];

        for(int i=0;i<totalPage;i++){
            collectionEndingGridImages[i] = collectionEndingGrid.GetChild(i).GetChild(0).GetComponent<Image>();
            collectionEndingGridRedDots[i] = collectionEndingGrid.GetChild(i).GetChild(1).gameObject;
        }
        for(int i=totalPage;i<collectionEndingGrid.childCount;i++){
            collectionEndingGrid.GetChild(i).gameObject.SetActive(false);
        }
        //ClickEndingCollectionGridSlot
        
        for(int i=0;i<collectionEndingGrid.childCount;i++){
            int temp = i;
            collectionEndingGrid.GetChild(temp).GetComponent<Button>().onClick.AddListener(()=>ClickEndingCollectionGridSlot(temp));
        }
#endregion

#region Reset Save&Load
        
        // for(int i=0;i<3;i++){
        //     int temp = i;
        //     btns[temp].GetComponent<Button>().onClick.AddListener(()=>OpenPanel(temp));
        // }

        for(int i=0;i<saveSlotGrid.childCount;i++){
            int temp = i;
            saveSlotGrid.GetChild(temp).GetComponent<Button>().onClick.AddListener(()=>TrySave(temp));
        }
        for(int i=0;i<loadSlotGrid.childCount;i++){
            int temp = i;
            loadSlotGrid.GetChild(temp).GetComponent<Button>().onClick.AddListener(()=>TryLoad(temp));
        }

        // for(int i=0;i<saveSlotGrid.childCount;i++){
        //     saveSlots[i].saveNameText = saveSlotGrid.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
        //     saveSlots[i].saveDateText = saveSlotGrid.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>();
        //     saveSlots[i].slotNumText = saveSlotGrid.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>();
        //     saveSlots[i].slotNumText.text = (i+1).ToString();
        //     saveSlots[i].itemInfoText0 = saveSlotGrid.GetChild(i).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
        //     saveSlots[i].itemInfoText1 = saveSlotGrid.GetChild(i).GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>();
        // }
        // for(int i=0;i<loadSlotGrid.childCount;i++){
        //     loadSlots[i].saveNameText = loadSlotGrid.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
        //     loadSlots[i].saveDateText = loadSlotGrid.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>();
        //     loadSlots[i].slotNumText = loadSlotGrid.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>();
        //     loadSlots[i].slotNumText.text = (i+1).ToString();
        //     loadSlots[i].itemInfoText0 = loadSlotGrid.GetChild(i).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
        //     loadSlots[i].itemInfoText1 = loadSlotGrid.GetChild(i).GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>();
        // }

        ResetSaveSlots();
        ResetLoadSlots();
#endregion

#region Settings
        slider_bgm.onValueChanged.AddListener(delegate{SoundManager.instance.SetVolumeBGM(MenuManager.instance.slider_bgm.value);});
        slider_sound.onValueChanged.AddListener(delegate{SoundManager.instance.SetVolumeSFX(MenuManager.instance.slider_sound.value);});
        dropdown_resolution.onValueChanged.AddListener(delegate{SetResolutionByValue(dropdown_resolution.value);});
        dropdown_frameRate.onValueChanged.AddListener(delegate{SetFrameRateByValue(dropdown_frameRate.value);});
        dropdown_dirtOnlyHUD.onValueChanged.AddListener(delegate{SetDirtOnlyHUD(dropdown_dirtOnlyHUD.value);});

        for(int i=0;i<setting_languagePage.childCount;i++){
            int temp = i;
            setting_languagePage.GetChild(i).GetComponent<Button>().onClick.AddListener(()=>ChangeLanguage(temp));
            setting_languagePage.GetChild(i).GetComponent<Button>().onClick.AddListener(()=>languagePanelMainText.ApplyTranslation());

        }


        SoundManager.instance.SetVolumeBGM(DBManager.instance.localData.bgmVolume / DBManager.instance.bgmAdjustVal);
        slider_bgm.value = DBManager.instance.localData.bgmVolume  / DBManager.instance.bgmAdjustVal;
        SoundManager.instance.SetVolumeSFX(DBManager.instance.localData.sfxVolume / DBManager.instance.sfxAdjustVal);
        slider_sound.value = DBManager.instance.localData.sfxVolume  / DBManager.instance.sfxAdjustVal;
        ToggleTalkSound(DBManager.instance.localData.onTalkingSound);

        ChangeLanguage(DBManager.instance.localData.languageValue, resetUI : false);

        SetWindowedMode(DBManager.instance.localData.isWindowedMode);
        dropdown_resolution.value = DBManager.instance.localData.resolutionValue;
        dropdown_frameRate.value = DBManager.instance.localData.frameRateValue;

        
#endregion
    
#region Set Ant Collection Object
        antStickers = new StickerScript[antStickersMother.childCount];

        for(int i=0;i<antStickersMother.childCount;i++){
            antStickers[i] = antStickersMother.GetChild(i).GetComponent<StickerScript>();
        }

        ResetAntCollectionUI();
#endregion


#region 

        for(int i=0;i<languagePanelBtnGrid.childCount;i++){
            int temp = i;
            languagePanelBtnGrid.GetChild(i).GetComponent<Button>().onClick.AddListener(()=>ChangeLanguage(temp,false));
            languagePanelBtnGrid.GetChild(i).GetComponent<Button>().onClick.AddListener(()=>languagePanelMainText.ApplyTranslation());

        }

#endregion
    }

    public void OpenLoadPanel(){
        loadPanel.SetActive(true);
    }
    
    public void CloseCurrentUI(){
        this.transform.parent.gameObject.SetActive(false);
    }


    // void Update(){

    // }
#region Collection_Ending

    #region Collection_Ending_Slider
    public void CollectionScrollRightBtn(){
        //var tempCollectionRealID = DBManager.instance.cache_EndingCollectionDataList.Find(x=>x.sort)


        EndingCollectionRedDotOff(DBManager.instance.cache_EndingCollectionDataList[tempCardNum[2]].ID);

        DeactivateBtns(collectionScrollArrows);
        
        if(curPage+1 == totalPage){
            curPage = 0;
        }
        else{
            curPage += 1;
        }
        RearrangeCardOrder();
        animator.SetTrigger("right");
    }
    
    public void CollectionScrollLeftBtn(){
        EndingCollectionRedDotOff(DBManager.instance.cache_EndingCollectionDataList[tempCardNum[2]].ID);

        DeactivateBtns(collectionScrollArrows);
        if(curPage == 0){
            curPage = totalPage-1;
        }
        else{
            curPage -= 1;
        }
        RearrangeCardOrder();
        animator.SetTrigger("left");
    }
    public void ResetCardOrder(){
        curPage = 0;
        RearrangeCardOrder();
        RearrangeCards();

    }
    public void RearrangeCardOrder(){
        tempCardNum = new int[]{curPage-2,curPage-1,curPage,curPage+1,curPage+2};
        if(tempCardNum[0]==-2) tempCardNum[0]=totalPage-2;
        else if(tempCardNum[0]==-1) tempCardNum[0]=totalPage-1;
        if(tempCardNum[1]==-1) tempCardNum[1]=totalPage-1;
        if(tempCardNum[3]==totalPage) tempCardNum[3]=0;
        if(tempCardNum[4]==totalPage+1) tempCardNum[4]=1;
        else if(tempCardNum[4]==totalPage) tempCardNum[4]=0;
//        Debug.Log(tempCardNum[0]+","+tempCardNum[1]+","+tempCardNum[2]+","+tempCardNum[3]+","+tempCardNum[4]);
    }
    public void RearrangeCards(){

        if(DBManager.instance == null) return;

        DBManager theDB = DBManager.instance;

        for(int i=0;i<5;i++){
            //cache_EndingCollectionDataList : data_collection 내 sortOrder에 의해 정렬된 캐시
            int tempCollectionID = theDB.GetClearedEndingCollectionID(theDB.cache_EndingCollectionDataList[tempCardNum[i]].ID);

            if(tempCollectionID != -1){
                //collectionCardImages[i].sprite = DBManager.instance.endingCollectionSprites[tempCardNum[i]];
                collectionCardImages[i].sprite = theDB.cache_EndingCollectionDataList[tempCardNum[i]].sprite;

                //220827 레드닷 노출
                
                if(!DBManager.instance.localData.endingCollectionOverList[tempCollectionID].isRecognized){
                    collectionCardRedDots[i].SetActive(true);

                    if(UIManager.instance!=null)
                        UIManager.instance.hud_sub_collection_redDot.SetActive(true);
                }
                else{
                    collectionCardRedDots[i].SetActive(false);

                }


            }
            else{

                collectionCardImages[i].sprite = collectionNullImage;//collectionCardSprites[tempCardNum[i]];
                collectionCardRedDots[i].SetActive(false);
                //collectionNameText.text = "???";
            }
        }

        collectionRateText.text = string.Format("{0:N0}%",100*theDB.localData.endingCollectionOverList.Count/theDB.cache_EndingCollectionDataList.Count);

        //int tempCenterCollectionID = theDB.GetClearedEndingCollectionID(theDB.cache_EndingCollectionDataList[tempCardNum[2]].ID);
        int tempCenterCollectionID = theDB.cache_EndingCollectionDataList[tempCardNum[2]].ID;
        int tempEndedIndex = theDB.localData.endingCollectionOverList.FindIndex(x=>x.ID==tempCenterCollectionID);
    
        if(tempEndedIndex!=-1){
            collectionNameText.text = DBManager.instance.cache_EndingCollectionDataList[tempCardNum[2]].name;

            int tempTrueEndingNum = 0;  //트루엔딩 번호
//            Debug.Log(tempCardNum[2]);

            if(theDB.cache_GameEndDataList.FindIndex(x => x.endingCollectionNum == tempCenterCollectionID)!=-1){
                collectionEndingNumberVessel.SetActive(true);
                tempTrueEndingNum = theDB.cache_GameEndDataList[theDB.cache_GameEndDataList.FindIndex(x => x.endingCollectionNum == tempCenterCollectionID)].endingNum;
                
                Debug.Log(tempCenterCollectionID);
            //Debug.Log(theDB.cache_GameEndDataList[theDB.cache_GameEndDataList.FindIndex(x => x.endingCollectionNum == tempCardNum[2])].comment);
            }
            else{
                
                collectionEndingNumberVessel.SetActive(false);
                tempTrueEndingNum = 0;
            }

//            Debug.Log(tempEndedIndex);

            collectionEndingNumberText.text = string.Format("Ending no.{0}",tempTrueEndingNum);//[tempCardNum[2]].endingNum);//"획득 : "+DBManager.instance.cache_EndingCollectionDataList[tempCardNum[2]].clearedCount +"번 째 플레이";
            collectionPlayCountText.text = string.Format("{0}",DBManager.instance.localData.endingCollectionOverList[tempEndedIndex].clearedPlayCount);//"획득 : "+DBManager.instance.cache_EndingCollectionDataList[tempCenterCollectionID].clearedCount +"번 째 플레이";
            //collectionRateText.text = string.Format("{0}%");//"획득 : "+DBManager.instance.cache_EndingCollectionDataList[tempCenterCollectionID].clearedCount +"번 째 플레이";
            collectionTimeText.text = string.Format("{0}",DBManager.instance.localData.endingCollectionOverList[tempEndedIndex].clearedDate);//.Substring(0,10);//"획득 : "+DBManager.instance.cache_EndingCollectionDataList[tempCenterCollectionID].clearedCount +"번 째 플레이";
        }
        else{                
            
            collectionNameText.text = "???";
            collectionEndingNumberVessel.SetActive(false);
            //collectionEndingNumberText.text = string.Format("Ending no.{0}",DBManager.instance.cache_GameEndDataList[tempCardNum[2]].endingNum);//"획득 : "+DBManager.instance.cache_EndingCollectionDataList[tempCardNum[2]].clearedCount +"번 째 플레이";
            collectionPlayCountText.text = "-";
            collectionTimeText.text = "-";

        }

        collectionCardOrderText.text = string.Format("{0}/{1}",tempCardNum[2]+1,DBManager.instance.cache_EndingCollectionDataList.Count);


        ActivateBtns(collectionScrollArrows);
    }
    public void ActivateBtns(Button[] btns){
        for(int i=0; i<btns.Length;i++){
            btns[i].interactable = true;
        }
    }
    public void DeactivateBtns(Button[] btns){
        for(int i=0; i<btns.Length;i++){
            btns[i].interactable = false;
        }
    }
    public void EndingCollectionRedDotOff(int collectionID){//collectionID는 collection의 sortOrder가 아닌 실제 ID
        int curEndingCollectionID = DBManager.instance.GetClearedEndingCollectionID(collectionID);
        var collectionSortOrderID = DBManager.instance.cache_EndingCollectionDataList.FindIndex(x=>x.ID == collectionID);
        var tempEndingCollection = DBManager.instance.localData.endingCollectionOverList.Find(x => x.ID == collectionID);

        Debug.Log(collectionID);

        if(collectionID!=-1){

            collectionCardRedDots[2].SetActive(false);
            collectionEndingGridRedDots[collectionSortOrderID].SetActive(false);
            //DBManager.instance.localData.endingCollectionOverList[curEndingCollectionID].isRecognized = true;
            if(tempEndingCollection != null) tempEndingCollection.isRecognized = true;

            //전부 인식 완료됐으면 메인 레드닷 제거
            if(UIManager.instance!=null && UIManager.instance.CheckCollectionOverListAllRecognized()){
                UIManager.instance.hud_sub_collection_redDot.SetActive(false);
            }
        }
        //var centerEndingCardID = DBManager.instance.cache_EndingCollectionDataList[tempCardNum[2]].ID;
        
    }
    #endregion

    #region Collection_Ending_Grid
    public void ClickEndingCollectionGridSlot(int slotNum){
        var theDB = DBManager.instance;
        int tempCollectionID = theDB.GetClearedEndingCollectionID(theDB.cache_EndingCollectionDataList[slotNum].ID);
        if(tempCollectionID!=-1)
            EndingCollectionRedDotOff(DBManager.instance.cache_EndingCollectionDataList[slotNum].ID);
        //Debug.Log(slotNum);
    }
    public void OpenEndingCollectionGridPage(){
        var theDB = DBManager.instance;

        for(int i=0;i<theDB.cache_EndingCollectionDataList.Count;i++){
            int tempCollectionID = theDB.GetClearedEndingCollectionID(theDB.cache_EndingCollectionDataList[i].ID);
                
            if(tempCollectionID != -1){
                collectionEndingGridImages[i].sprite = theDB.cache_EndingCollectionDataList[i].sprite;
                
                
                if(!DBManager.instance.localData.endingCollectionOverList[tempCollectionID].isRecognized){
                    collectionEndingGridRedDots[i].SetActive(true);

                    if(UIManager.instance!=null)
                        UIManager.instance.hud_sub_collection_redDot.SetActive(true);
                }
                else{
                    collectionEndingGridRedDots[i].SetActive(false);
                }

            }
            else{
                collectionEndingGridImages[i].sprite = collectionNullImage;
                collectionEndingGridRedDots[i].SetActive(false);
            }
        }

    }
    #endregion

#endregion
    
    
#region Save&Load 
    public Transform saveSlotMother;
    public void ResetSaveSlots(){
        for(int i=0; i<saveSlotMother.childCount; i++){
            ResetSaveSlot(i);
        }
    }

    public void ResetSaveSlot(int slotNum){


        var curSlot = saveSlotMother.GetChild(slotNum).GetComponent<SaveLoadSlot>();

        ResetSlot(slotNum, curSlot);
        
    }
    public Transform loadSlotMother;

    public void ResetLoadSlots(){
        for(int i=0; i<loadSlotMother.childCount; i++){
            ResetLoadSlot(i);
        }
    }
    public void ResetLoadSlot(int slotNum){
        var curSlot = loadSlotMother.GetChild(slotNum).GetComponent<SaveLoadSlot>();
        
        ResetSlot(slotNum, curSlot);
    }
    public void ResetSlot(int slotNum, SaveLoadSlot curSlot){
        
        if(DBManager.instance.CheckSaveFile(slotNum)){

            var getData = DBManager.instance.GetData(slotNum);
            var getItemList = getData.itemList;

            //if(UIManager.instance!=null)
                MenuManager.instance.ApplyFont(curSlot.saveNameText);

            curSlot.saveNameText.text = CSVReader.instance.GetIndexToString(getData.curMapNum,"map");
            curSlot.playTimeText.text = ConvertSeconds2TimeString(getData.curPlayTime);
            curSlot.saveDateText.text = getData.curPlayDate;
            curSlot.itemInfoText0.text = string.Format("{0:#,###0}", getData.curHoneyAmount);//getData.curHoneyAmount.ToString();
            curSlot.itemInfoText1.text = string.Format("{0:N0}%", 100*getData.curDirtAmount/DBManager.instance.maxDirtAmount);
            
            for(int j=0;j<getItemList.Count;j++){
                if(j==10) break;
                var curItem = getItemList[j];

                Item curItemInfo = null;
                //if(DBManager.instance.cache_ItemDataList.Count != 0)
                try {
                    curItemInfo = DBManager.instance.cache_ItemDataList.Find(x => x.ID == curItem.itemID);
                }
                catch (NullReferenceException){
                    Debug.Log(slotNum + "번 슬롯 아이템 리스트 불러오기 오류");
                    return;
                }

                curSlot.itemSlotGrid.GetChild(j).GetComponent<ItemSlot2>().itemImage.sprite = curItemInfo.icon;

                if(curItemInfo.isStack){
                    curSlot.itemSlotGrid.GetChild(j).GetComponent<ItemSlot2>().itemAmountText.text = curItem.itemAmount.ToString();
                }
                else{
                    
                    curSlot.itemSlotGrid.GetChild(j).GetComponent<ItemSlot2>().itemAmountText.text = string.Empty;
                }
            }
            
            for(int j=getItemList.Count;j<10;j++){
                curSlot.itemSlotGrid.GetChild(j).GetComponent<ItemSlot2>().itemImage.sprite = nullSprite;
                curSlot.itemSlotGrid.GetChild(j).GetComponent<ItemSlot2>().itemAmountText.text = string.Empty;
                
            }
            
        }
        else{

            //if(UIManager.instance!=null)
                MenuManager.instance.ApplyFont(curSlot.saveNameText);

            curSlot.saveNameText.text = CSVReader.instance.GetIndexToString(69,"sysmsg");
            curSlot.playTimeText.text = "-";
            curSlot.saveDateText.text = "-";
            curSlot.itemInfoText0.text ="-";
            curSlot.itemInfoText1.text = "-";
            
            for(int j=0;j<10;j++){
                if(MenuManager.instance != null)
                    curSlot.itemSlotGrid.GetChild(j).GetComponent<ItemSlot2>().itemImage.sprite = MenuManager.instance.nullSprite;
                curSlot.itemSlotGrid.GetChild(j).GetComponent<ItemSlot2>().itemAmountText.text = string.Empty;
            }
        }
    }
    
    //저장 슬롯 터치 시
    public void TrySave(int num){
        curSaveNum = num;

        if(DBManager.instance.CheckSaveFile(num)){
            OpenPopUpPanel("save_overwrite");
        }
        else{
            Save(curSaveNum);
        }
        
    }
    //로드 슬롯 터치시
    public void TryLoad(int num){
        curLoadNum = num;

        if(DBManager.instance.CheckSaveFile(num)){
            OpenPopUpPanel("load");
        }
        else{
            //Load(curLoadNum);
        }
    }
    public void TryLoadLast(){
        OpenPopUpPanel("loadLast");

    }
    public void TryLoadMain(){

        OpenPopUpPanel("goMain");
    }
    public void TryRestartGame(){
        OpenPopUpPanel("restart");

    }
    public void TryQuitGame(){

        OpenPopUpPanel("quitGame");
    }
        
    public void TryDeleteSaveFile(int saveFileNum){
        curSaveNum = saveFileNum;
        
        if(DBManager.instance.CheckSaveFile(saveFileNum)){
            OpenPopUpPanel("deleteSaveFile");
        }
        else{
            Debug.Log("No Save File : "+saveFileNum);
        }
    }
    public void OpenPopUpPanel(string type){
        curPopUpType = type;
        //확인
        popUpText[2].text ="0";
        //취소
        popUpText[3].text ="1";
        switch(type){
            case "save_overwrite" :
                popUpText[0].text = "7";
                popUpText[1].text = "";
                break;
            case "load" :
                popUpText[0].text = "8";
                popUpText[1].text = PlayerManager.instance!=null ? "3" : "";
                break;
            case "loadLast" :
                popUpText[0].text = "5";
                popUpText[1].text = "";
                break;
            case "goMain" :
                popUpText[0].text = "6";
                popUpText[1].text = "";
                break;
            case "restart" :
                popUpText[0].text = "11";
                popUpText[1].text = "3";
                break;
            case "quitGame" :
                popUpText[0].text = "2";
                popUpText[1].text = "";
                break;
            case "deleteSaveFile" :
                popUpText[0].text = "12";
                popUpText[1].text = "";
                break;
            case "resetData" :
                popUpText[0].text = "13";
                popUpText[1].text = "14";
                break;

            default :
                break;
        }

        for(int i=0;i<4;i++){
            if(popUpText[i].text != "")
                popUpText[i].text = CSVReader.instance.GetIndexToString(int.Parse(popUpText[i].text),"sysmsg");
        }

        popUpPanel.SetActive(true);
    }
    
    public void OpenPopUpPanel_OneAnswer(string mainIndex, string okayIndex = "0", string[] mainArguments = null, string type = ""){

        if(string.IsNullOrEmpty(type)){
            curPopUpType = string.Empty;
        }


        popUpText1[0].text = mainIndex;
        //확인
        popUpText1[1].text = okayIndex;


            if(popUpText1[0].text != "")
                popUpText1[0].text = mainArguments == null ?
                CSVReader.instance.GetIndexToString(int.Parse(popUpText1[0].text),"sysmsg")
                : string.Format(CSVReader.instance.GetIndexToString(int.Parse(popUpText1[0].text),"sysmsg"),mainArguments);

            if(popUpText1[1].text != "")
                popUpText1[1].text = CSVReader.instance.GetIndexToString(int.Parse(popUpText1[1].text),"sysmsg");
        

        popUpPanel1.SetActive(true);
    }
    public void PopUpOkayBtn(){
        //Debug.Log("A");
        switch(curPopUpType){
            case "save_overwrite" :
                Save(curSaveNum);
                ResetSaveSlots();
                ResetLoadSlots();
                //Debug.Log(curSaveNum + "번 저장 시도");
                break;

            case "load" :
                Load(curLoadNum);
                //Debug.Log(curSaveNum + "번 로드 시도");
                break;
            case "restart" :
                Load(-1);
                //Debug.Log(curSaveNum + "번 로드 시도");
                break;

            case "loadLast" :
                LoadLast();
                break;
            case "goMain" :
                LoadManager.instance.LoadMain();
                break;
            case "quitGame" :
                Application.Quit();
                //LoadManager.instance.LoadMain();
                break;
            // case "changeKey" :
            //     waitKeyChange = false;
            //     //LoadManager.instance.LoadMain();
            //     break;

            case "deleteSaveFile" :
                DBManager.instance.DeleteSaveFile(curSaveNum);
                ResetSaveSlot(curSaveNum);
                ResetLoadSlot(curSaveNum);
                break;

            case "resetData" :
                
                DBManager.instance.ResetAllData();
                settingPanel.SetActive(false);
                menuPanel.SetActive(false);
                SoundManager.instance.BgmOff();
                LoadManager.instance.LoadMain();
                break;
            default:
//                Debug.Log(curPopUpType);
                popUpOkayCheck = true;
                Invoke("ResetPopUpOkayCheck",0.05f);
                break;

        }
        curPopUpType = "";

    }
    public void ResetPopUpOkayCheck(){
        popUpOkayCheck = false;

    }
    public void ClosePopUp(){
        waitPopUpClosed = false;
    }
    public void Save(int curSaveNum){
        DBManager.instance.CallSave(curSaveNum);
        LoadManager.instance.lastLoadFileNum = curSaveNum;

        ResetSaveSlot(curSaveNum);
        // saveSlots[curSaveNum].saveNameText.text = CSVReader.instance.GetIndexToString(DBManager.instance.curData.curMapNum,"map");
        // saveSlots[curSaveNum].saveDateText.text = DBManager.instance.curData.curPlayDate;
        // saveSlots[curSaveNum].itemInfoText0.text = DBManager.instance.curData.curHoneyAmount.ToString();
        // saveSlots[curSaveNum].itemInfoText1.text = string.Format("{0:N0}", 100*DBManager.instance.curData.curDirtAmount/DBManager.instance.maxDirtAmount) + "%";
        
    }
    public void Load(int curLoadNum){
        if(!DBManager.instance.CheckFileExist(curLoadNum)) return;

        CloseAllPanels();
        CloseMenuPanel();
        LoadManager.instance.isLoadingInGame = true;
        LoadManager.instance.lastLoadFileNum = curLoadNum;
        LoadManager.instance.LoadGame();
        Debug.Log(curLoadNum + "번 파일 로드 시도");
        //DBManager.instance.CallLoad(curSaveNum);
        //saveSlots[curSaveNum].saveNameText.text = DBManager.instance.curData.curMapName;
        //saveSlots[curSaveNum].saveDateText.text = DBManager.instance.curData.curPlayDate;
    }
    public void CloseAllPanels(){
        for(int i=0; i<panels.Length;i++){
            panels[i].SetActive(false);
        }
    }
    public void CloseMenuPanel(){
        menuPanel.SetActive(false);
    }
    public void ToggleSubPanel(int panelNum){
        CloseAllPanels();
        panels[panelNum].SetActive(true);
    }
    public void OpenPanel(string panelName){
        CloseAllPanels();

        switch(panelName){
            case "save" :
                ResetSaveSlots();
                savePanel.SetActive(true);
                break;
            case "load" :
                ResetLoadSlots();
                loadPanel.SetActive(true);
                break;
            case "collection" :
                ResetCardOrder();
                collectionPanel.SetActive(true);

                        
                if(DBManager.instance.localData.endingCollectionOverList.Count == DBManager.instance.cache_EndingCollectionDataList.Count){
                    SteamAchievement.instance.ApplyAchievements(0);
                }

                break;
            case "setting" :
                settingPanel.SetActive(true);
                break;
            case "language" :
                languagePanel.SetActive(true);
                break;
        }

        //panels[panelNum].SetActive(true);
        
    }
    public void LoadLast(){
        CloseAllPanels();
        CloseMenuPanel();
        LoadManager.instance.isLoadingInGameToLastPoint = true;
        //LoadManager.instance.lastLoadFileNum = curLoadNum;
        LoadManager.instance.LoadGame();
        Debug.Log("Try to load last saved file..., FileNum : " + LoadManager.instance.lastLoadFileNum);

    }
#endregion

#region Settings / Options
    public void OpenSettingPage(int pageNum){
        foreach(GameObject page in settingPages){
            page.SetActive(false);
        }
        foreach(Button btn in settingMenuBtns){
            btn.interactable = true;
        }

        settingPages[pageNum].SetActive(true);
        settingMenuBtns[pageNum].interactable = false;
    }

    public void ChangeLanguage(int languageNum , bool resetUI = true){
        for(int i=0;i<setting_languagePage.childCount;i++){
            setting_languagePage.GetChild(i).GetComponent<Button>().interactable = true;
        }
//        Debug.Log(languageNum);

        setting_languagePage.GetChild(languageNum).GetComponent<Button>().interactable = false;
        string lang = "";
        switch(languageNum){
            case 0 :    
                lang = "kr";
                break;
            case 1 :    
                lang = "en";
                break;
            case 2 :    
                lang = "jp";
                break;
        }
        DBManager.instance.language = lang;
        DBManager.instance.localData.languageValue = languageNum;

        DBManager.instance.ApplyNewLanguage(resetUI);
    }

    public void ToggleTalkSound(bool active){
        //false : off, 1 : on
        //if(active){
            //Debug.Log(gameObject.name);
            toggleBtnOff_talkSound.SetActive(!active);
            toggleBtnOn_talkSound.SetActive(active);
            DBManager.instance.localData.onTalkingSound  = active;

        // }
        // else{
        //     toggleBtnOff_talkSound.SetActive(true);
        //     toggleBtnOn_talkSound.SetActive(false);
        //     DBManager.instance.curData.onTalkingSound = true;
        // }
    }
    public void SetWindowedMode(bool active){
        checkedBtn_fullScreen.SetActive(!active);
        checkedBtn_window.SetActive(active);
        DBManager.instance.localData.isWindowedMode  = active;
        SetResolutionByValue(DBManager.instance.localData.resolutionValue);
        //SetResolutionBySavedValue();
    }

    public void SetResolutionByValue(int value){

        DBManager.instance.localData.resolutionValue = value;

        int width=0, height=0;
        switch(value){
            case 0 :
                width = 1920;
                height = 1080;
                break;
            case 1 :
                width = 1680;
                height = 1050;
                break;
        }
        SetResolution(width, height);

        // DBManager.instance.localData.screenWidth = width;
        // DBManager.instance.localData.screenHeight = height;
        // SetResolutionBySavedValue();
    }
    // public void SetResolutionBySavedValue(){
    //     Screen.SetResolution(DBManager.instance.localData.screenWidth,DBManager.instance.localData.screenHeight,!DBManager.instance.localData.isWindowedMode);
    // }
    void SetResolution(int width, int height){
        Screen.SetResolution(width,height,!DBManager.instance.localData.isWindowedMode);
    }
    
    public void SetFrameRateByValue(int value){
        DBManager.instance.localData.frameRateValue = value;

        switch(value){
            case 0 :
                SetFrameRate(144);
                break;
            case 1 :
                SetFrameRate(60);
                break;
            case 2 :
                SetFrameRate(30);
                break;
        }
        
    }
    // public void SetFrameRateBySavedValue(){
    //     //Debug.Log(DBManager.instance.localData.frameRate);
    //     SetFrameRateByValue(DBManager.instance.localData.frameRateValue);
    //     //Application.targetFrameRate = DBManager.instance.localData.frameRate;
    //     //QualitySettings.vSyncCount = 0;
    // }

    void SetFrameRate(int frameRate){
        Application.targetFrameRate = frameRate;
        QualitySettings.vSyncCount = 0;
    }
    
    void OnGUI()
    {
        if(waitKeyChange){

            Event e = Event.current;
            if (e.isKey && e.keyCode != KeyCode.Escape)
            {
                waitKeyChange = false;
                Debug.Log("설정완료" + e.keyCode.ToString());
                GameInputManager.SetKeyMap(curChangingKeyName,e.keyCode);

                switch (curChangingKeyName)
                {
                    case "Jump" :  
                        DBManager.instance.localData.jumpKey = e.keyCode;
                        break;
                    case "Interact" :  
                        DBManager.instance.localData.interactKey = e.keyCode;
                        break;
                    case "AddDirt" :  
                        DBManager.instance.localData.AddDirtKey = e.keyCode;
                        break;
                    default:
                        break;
                }

                keyText_jump.text = DBManager.instance.localData.jumpKey.ToString();
                keyText_interact.text = DBManager.instance.localData.interactKey.ToString();
                keyText_adddirt.text = DBManager.instance.localData.AddDirtKey.ToString();

                popUpPanel.SetActive(false);
                popUpPanel1.SetActive(false);
                //ClosePopUp();

            }
        }
    }
    public void ChangeKey(string keyName){
        curPopUpType = "changeKey";
        waitKeyChange = true;
        curChangingKeyName = keyName;
        switch (keyName)
        {
            case "Jump" :  
                OpenPopUpPanel_OneAnswer("70","1");
                break;
            case "Interact" :  
                OpenPopUpPanel_OneAnswer("71","1");
                break;
            case "AddDirt" :  
                OpenPopUpPanel_OneAnswer("97","1");
                break;
            default:
                break;
        }
    }
    
    public void TryResetData(){
        
        OpenPopUpPanel("resetData");
    }

    public void SetDirtOnlyHUD(int num){
        DBManager.instance.dirtOnlyHUD = num == 0 ? true : false;

        if(UIManager.instance != null){
            UIManager.instance.SetDirtOnlyHUD(DBManager.instance.dirtOnlyHUD);
        }
    }
#endregion

    public void ToggleMenuPanel(bool active){
        MenuManager.instance.menuPanel.SetActive(active);
        if(active){
            PlayerManager.instance.LockPlayer();
        }
        else{
            PlayerManager.instance.UnlockPlayer();

        }
    }
    public void OpenPopUpPanel_onWork(){
        popUpOnWork.SetActive(true);
    }
    public string ConvertSeconds2TimeString(float getSec){
        TimeSpan t = TimeSpan.FromSeconds( getSec );

        return 
        string.Format("{0:D2}:{1:D2}:{2:D2}",
                            t.Hours,
                            t.Minutes,
                            t.Seconds);
    }

#region Collection_Ant
    public void OpenCollectionPage(int pageNum){
        
        foreach(GameObject page in collectionPages){
            page.SetActive(false);
        }
        foreach(Button btn in collectionTabBtns){
            btn.interactable = true;
        }

        if(pageNum==0){
            ResetAntCollectionUI();
            //RefreshAntCollectionUI();
        }
        else{
            RearrangeCardOrder();
        }


        collectionPages[pageNum].SetActive(true);
        collectionTabBtns[pageNum].interactable = false;

        OpenEndingCollectionGridPage();
    }

    public void ResetAntCollectionUI(){
        antMainImage.sprite = nullSprite;
        antMainNameTextHolderObj.SetActive(false);

        //RefreshAntCollectionUI();

        // for(int i=0;i<antStickersMother.childCount;i++){
        //     if(DBManager.instance.localData.antCollectionOverList.Exists(x=>x.ID == i+301)){
        //         antStickers[i].ActivateSticker();
        //     }
        //     else{
        //         antStickers[i].DeactivateSticker();
        //     }
        // }

        
        if(DBManager.instance.localData.antCollectionOverList.Count == MenuManager.instance.antStickersMother.childCount){
            SteamAchievement.instance.ApplyAchievements(14);
        }
    }
    public void RefreshAntCollectionUI(){
        //antMainImage.sprite = nullSprite;

        for(int i=0;i<antStickersMother.childCount;i++){
            if(DBManager.instance.GetClearedAntCollectionIndex(i+1)!=-1){
                antStickers[i].ActivateSticker();
            }
            else{
                antStickers[i].DeactivateSticker();
            }
        }
        // foreach(StickerScript a in antStickersMother.GetComponentsInChildren<StickerScript>()){
        //     if(DBManager.instance.)
        // }
    }










#endregion

    public void CheckCoupon(){
        var couponList = DBManager.instance.cache_couponList;
        string curCode = couponInputField.text;
        int index = couponList.FindIndex(x => x.code == curCode);
        Debug.Log("쿠폰 " + index);

        if(index==-1){
            OpenPopUpPanel_OneAnswer("95");
        }
        else{

#if !DISABLESTEAMWORKS

            SteamUserStats.RequestGlobalStats(60);
            SteamUserStats.RequestCurrentStats();
            SteamUserStats.GetGlobalStat("cp"+index,out long cpGlobal);
            SteamUserStats.GetStat("cp"+index,out int cpLocal);
            if(cpGlobal==1){
                OpenPopUpPanel_OneAnswer("95");
                Debug.Log("서버에 이미 누가 등록함");
            }
            else if(cpLocal==1){
                OpenPopUpPanel_OneAnswer("95");
                Debug.Log("내가 이미 등록함");
            }
            else if(DBManager.instance.localData.usedCouponRewardItemID != 0){

                OpenPopUpPanel_OneAnswer("95");
                Debug.Log("이미 등록한 쿠폰이 있음");
            }

            //if(index!=-1 && DBManager.instance.localData.usedCouponRewardItemID == 0){
            else{
                int rewardItemID = couponList[index].rewardItemID;
                string[] tempArr = {DBManager.instance.cache_ItemDataList[rewardItemID].name};
                DBManager.instance.localData.usedCouponRewardItemID = rewardItemID;
                OpenPopUpPanel_OneAnswer("94",mainArguments:tempArr);
                ui_coupon.SetActive(false);
                SteamUserStats.SetStat("cp"+index,1);
                SteamUserStats.StoreStats();
            }
#endif
        }
    }
    ///<summary>
    ///해당 텍스트가 Text일 경우(TMP가 아닐 경우), 해당 Text.text 설정 전, 폰트 별도 적용 필요
    ///</summary>
    public void ApplyFont(Text curText){
        string curLang = DBManager.instance.language;
        switch(curLang){
            case "kr" :
            case "en" :
                curText.font = MenuManager.instance.fonts[0];
                break;
                
            case "jp" :
                curText.font = MenuManager.instance.fonts[1];
                break;
        }
    }

    
    public void SetCollectionPage(bool active){
        if(MenuManager.instance==null) return;

        ResetAntCollectionUI();
        RearrangeCards();
        OpenEndingCollectionGridPage();

        if(PlayerManager.instance!=null) PlayerManager.instance.SetLockPlayer(active);
        MenuManager.instance.collectionPanel.SetActive(active);

        if(active){
            //if(DBManager.instance.GetClearedEndingCollectionID(0)!=-1)
            //Debug.Log("3434");
            collectionNameText.text = DBManager.instance.GetClearedEndingCollectionID(0)!=-1 ?
            DBManager.instance.cache_EndingCollectionDataList[tempCardNum[2]].name : "???";

        }
        //hud_sub_collection_new.SetActive(false);
    }
}
