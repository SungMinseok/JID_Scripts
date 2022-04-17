using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    [Header("[Game Objects]━━━━━━━━━━━━━━━━━━━━━━━")]
    public GameObject btnGridObject;
    public GameObject[] panels;
    public GameObject[] btns;
    [Header("UI_Menu_Main")]
    public GameObject menuPanel;
    public Color defaultBtnColor;
    public Color activatedBtnColor;
    public GameObject collectionPanel;
    public GameObject antCollectionPanel;
    public GameObject endingCollectionPanel;
    [Header("UI_PopUp")]
    public GameObject popUpPanel;
    public TextMeshProUGUI[] popUpText; //main, sub, ok, cancel
    public GameObject popUpPanel1;
    public TextMeshProUGUI[] popUpText1; //main, sub, ok, cancel
    public bool popUpOkayCheck;
    public GameObject popUpOnWork;
    public bool waitPopUpClosed;
    [Header("UI_Save&Load")]
    public GameObject savePanel;
    public GameObject loadPanel;
    public Transform saveSlotGrid;
    public SaveLoadSlot[] saveSlots;
    public Transform loadSlotGrid;
    public SaveLoadSlot[] loadSlots;
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
    [System.Serializable]
    public class SaveLoadSlot{
        public TextMeshProUGUI saveNameText;
        public TextMeshProUGUI saveDateText;
        public TextMeshProUGUI slotNumText;
        public TextMeshProUGUI itemInfoText0;
        public TextMeshProUGUI itemInfoText1;
    }
    [Header("UI_Collection_Ending")]
    public Animator animator;
    public GameObject collectionEndingNumberVessel;
    public Text collectionEndingNumberText;
    public Text collectionNameText;
    public Text collectionPlayCountText;
    public Text collectionRateText;
    public Text collectionTimeText;
    public Sprite collectionNullImage;
    public Image[] collectionCardImages;
    public Sprite[] collectionCardSprites;
    public Button[] collectionScrollArrows;
    
    [Header("UI_ETC")]
    public Font[] fonts ;
    [Header("Debug────────────────────")]
    public string curPopUpType;
    public int curSaveNum;
    public int curLoadNum;
    public int totalPage;
    public int curPage;
    public int[] tempCardNum = new int[5];

    void Awake(){
        instance = this;
    }    
    // void Update(){
    //     if(menuPanel.activeSelf){

    //     }

    // }
    void Start(){

        fonts = new Font[2];
        fonts[0] = Resources.Load<Font>("Cafe24Ssurround");
        fonts[1] = Resources.Load<Font>("uzura");

#region Reset Collection
        //totalPage = DBManager.instance.endingCollectionSprites.Length;
        totalPage = DBManager.instance.cache_EndingCollectionDataList.Count;

        collectionScrollArrows[0].GetComponent<Button>().onClick.AddListener(()=>CollectionScrollRightBtn());
        collectionScrollArrows[1].GetComponent<Button>().onClick.AddListener(()=>CollectionScrollLeftBtn());
        
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

        for(int i=0;i<saveSlotGrid.childCount;i++){
            saveSlots[i].saveNameText = saveSlotGrid.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
            saveSlots[i].saveDateText = saveSlotGrid.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>();
            saveSlots[i].slotNumText = saveSlotGrid.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>();
            saveSlots[i].slotNumText.text = (i+1).ToString();
            saveSlots[i].itemInfoText0 = saveSlotGrid.GetChild(i).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
            saveSlots[i].itemInfoText1 = saveSlotGrid.GetChild(i).GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        for(int i=0;i<loadSlotGrid.childCount;i++){
            loadSlots[i].saveNameText = loadSlotGrid.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
            loadSlots[i].saveDateText = loadSlotGrid.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>();
            loadSlots[i].slotNumText = loadSlotGrid.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>();
            loadSlots[i].slotNumText.text = (i+1).ToString();
            loadSlots[i].itemInfoText0 = loadSlotGrid.GetChild(i).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
            loadSlots[i].itemInfoText1 = loadSlotGrid.GetChild(i).GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        ResetSaveSlots();
        ResetLoadSlots();
#endregion
#region Settings
        slider_bgm.onValueChanged.AddListener(delegate{SoundManager.instance.SetVolumeBGM(MenuManager.instance.slider_bgm.value);});
        slider_sound.onValueChanged.AddListener(delegate{SoundManager.instance.SetVolumeSFX(MenuManager.instance.slider_sound.value);});
        dropdown_resolution.onValueChanged.AddListener(delegate{SetResolutionByValue(dropdown_resolution.value);});
        dropdown_frameRate.onValueChanged.AddListener(delegate{SetFrameRateByValue(dropdown_frameRate.value);});

        for(int i=0;i<setting_languagePage.childCount;i++){
            int temp = i;
            setting_languagePage.GetChild(i).GetComponent<Button>().onClick.AddListener(()=>ChangeLanguage(temp));

        }


        SoundManager.instance.SetVolumeBGM(DBManager.instance.localData.bgmVolume);
        slider_bgm.value = DBManager.instance.localData.bgmVolume;
        SoundManager.instance.SetVolumeSFX(DBManager.instance.localData.sfxVolume);
        slider_sound.value = DBManager.instance.localData.sfxVolume;
        ToggleTalkSound(DBManager.instance.localData.onTalkingSound);

        ChangeLanguage(DBManager.instance.localData.languageValue, resetUI : false);

        SetWindowedMode(DBManager.instance.localData.isWindowedMode);
        dropdown_resolution.value = DBManager.instance.localData.resolutionValue;
        dropdown_frameRate.value = DBManager.instance.localData.frameRateValue;
#endregion
    
    }

    public void OpenLoadPanel(){
    
    }
    
    public void OpenCollectionPanel(){

    }


    // void Update(){

    // }
#region Collection
    public void CollectionScrollRightBtn(){
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

            int tempCollectionID = theDB.GetClearedEndingCollectionID(theDB.cache_EndingCollectionDataList[tempCardNum[i]].ID);

            if(tempCollectionID != -1){
                //collectionCardImages[i].sprite = DBManager.instance.endingCollectionSprites[tempCardNum[i]];
                collectionCardImages[i].sprite = theDB.cache_EndingCollectionDataList[tempCardNum[i]].sprite;
            }
            else{

                collectionCardImages[i].sprite = collectionNullImage;//collectionCardSprites[tempCardNum[i]];
                //collectionNameText.text = "???";
            }
        }

        collectionRateText.text = string.Format("{0:N0}%",100*theDB.localData.endingCollectionOverList.Count/theDB.cache_EndingCollectionDataList.Count);

        int tempCenterCollectionID = theDB.GetClearedEndingCollectionID(theDB.cache_EndingCollectionDataList[tempCardNum[2]].ID);
    
        if(tempCenterCollectionID!=-1){
            collectionNameText.text = DBManager.instance.cache_EndingCollectionDataList[tempCardNum[2]].name;

            int tempTrueEndingNum = 0;  //트루엔딩 번호
            //Debug.Log(tempCardNum[2]);

            if(theDB.cache_GameEndDataList.FindIndex(x => x.endingCollectionNum == tempCenterCollectionID)!=-1){
                collectionEndingNumberVessel.SetActive(true);
                tempTrueEndingNum = theDB.cache_GameEndDataList[theDB.cache_GameEndDataList.FindIndex(x => x.endingCollectionNum == tempCenterCollectionID)].endingNum;
            //Debug.Log(theDB.cache_GameEndDataList[theDB.cache_GameEndDataList.FindIndex(x => x.endingCollectionNum == tempCardNum[2])].comment);
            }
            else{
                
                collectionEndingNumberVessel.SetActive(false);
                tempTrueEndingNum = 0;
            }

            collectionEndingNumberText.text = string.Format("Ending no.{0}",tempTrueEndingNum);//[tempCardNum[2]].endingNum);//"획득 : "+DBManager.instance.cache_EndingCollectionDataList[tempCardNum[2]].clearedCount +"번 째 플레이";
            collectionPlayCountText.text = string.Format("{0}",DBManager.instance.localData.endingCollectionOverList[tempCenterCollectionID].clearedPlayCount);//"획득 : "+DBManager.instance.cache_EndingCollectionDataList[tempCenterCollectionID].clearedCount +"번 째 플레이";
            //collectionRateText.text = string.Format("{0}%");//"획득 : "+DBManager.instance.cache_EndingCollectionDataList[tempCenterCollectionID].clearedCount +"번 째 플레이";
            collectionTimeText.text = string.Format("{0}",DBManager.instance.localData.endingCollectionOverList[tempCenterCollectionID].clearedDate);//.Substring(0,10);//"획득 : "+DBManager.instance.cache_EndingCollectionDataList[tempCenterCollectionID].clearedCount +"번 째 플레이";
        }
        else{                
            
            collectionNameText.text = "???";
            collectionEndingNumberVessel.SetActive(false);
            //collectionEndingNumberText.text = string.Format("Ending no.{0}",DBManager.instance.cache_GameEndDataList[tempCardNum[2]].endingNum);//"획득 : "+DBManager.instance.cache_EndingCollectionDataList[tempCardNum[2]].clearedCount +"번 째 플레이";
            collectionPlayCountText.text = "-";
            collectionTimeText.text = "-";

        }
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
    
#endregion
    
    
#region Save&Load 
    public void ResetSaveSlots(){
        for(int i=0; i<saveSlotGrid.childCount; i++){
            if(DBManager.instance.CheckSaveFile(i)){
                saveSlots[i].saveNameText.text = CSVReader.instance.GetIndexToString(DBManager.instance.GetData(i).curMapNum,"map");
                saveSlots[i].saveDateText.text = DBManager.instance.GetData(i).curPlayDate;
                saveSlots[i].itemInfoText0.text = DBManager.instance.GetData(i).curHoneyAmount.ToString();
                saveSlots[i].itemInfoText1.text = string.Format("{0:N0}", 100*DBManager.instance.GetData(i).curDirtAmount/DBManager.instance.maxDirtAmount) + "%";
            }
            else{
                saveSlots[i].saveNameText.text = CSVReader.instance.GetIndexToString(69,"sysmsg");
                saveSlots[i].saveDateText.text = "-";
                saveSlots[i].itemInfoText0.text ="-";
                saveSlots[i].itemInfoText1.text = "-";
            }
        }
    }
    public void ResetLoadSlots(){
        for(int i=0; i<loadSlotGrid.childCount; i++){
            if(DBManager.instance.CheckSaveFile(i)){
                loadSlots[i].saveNameText.text = CSVReader.instance.GetIndexToString(DBManager.instance.GetData(i).curMapNum,"map");
                loadSlots[i].saveDateText.text = DBManager.instance.GetData(i).curPlayDate;
                loadSlots[i].itemInfoText0.text = DBManager.instance.GetData(i).curHoneyAmount.ToString();
                loadSlots[i].itemInfoText1.text = string.Format("{0:N0}", 100*DBManager.instance.GetData(i).curDirtAmount/DBManager.instance.maxDirtAmount) + "%";
                //loadSlots[i].itemInfoText1.text = DBManager.instance.GetData(i).curDirtAmount.ToString();
            }
            else{
                loadSlots[i].saveNameText.text = CSVReader.instance.GetIndexToString(69,"sysmsg");
                loadSlots[i].saveDateText.text = "-";
                loadSlots[i].itemInfoText0.text ="-";
                loadSlots[i].itemInfoText1.text = "-";
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
    public void TryQuitGame(){

        OpenPopUpPanel("quitGame");
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
                popUpText[1].text = "3";
                break;
            case "loadLast" :
                popUpText[0].text = "5";
                popUpText[1].text = "";
                break;
            case "goMain" :
                popUpText[0].text = "6";
                popUpText[1].text = "";
                break;
            case "quitGame" :
                popUpText[0].text = "2";
                popUpText[1].text = "";
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
    
    public void OpenPopUpPanel_SetStringByIndex(string mainIndex, string okayIndex = "0"){

        popUpText1[0].text = mainIndex;
        //확인
        popUpText1[1].text = okayIndex;

        
        for(int i=0;i<2;i++){
            if(popUpText1[i].text != "")
                popUpText1[i].text = CSVReader.instance.GetIndexToString(int.Parse(popUpText1[i].text),"sysmsg");
        }

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

            default:
//                Debug.Log(curPopUpType);
                popUpOkayCheck = true;
                Invoke("ResetPopUpOkayCheck",0.05f);
                break;

        }

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
        saveSlots[curSaveNum].saveNameText.text = CSVReader.instance.GetIndexToString(DBManager.instance.curData.curMapNum,"map");
        saveSlots[curSaveNum].saveDateText.text = DBManager.instance.curData.curPlayDate;
        saveSlots[curSaveNum].itemInfoText0.text = DBManager.instance.curData.curHoneyAmount.ToString();
        saveSlots[curSaveNum].itemInfoText1.text = string.Format("{0:N0}", 100*DBManager.instance.curData.curDirtAmount/DBManager.instance.maxDirtAmount) + "%";
        
    }
    public void Load(int curLoadNum){
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
                break;
            case "setting" :
                settingPanel.SetActive(true);
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
        Debug.Log(LoadManager.instance.lastLoadFileNum + "번 파일 로드 시도 (마지막 저장)");

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
}
