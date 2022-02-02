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
    [Header("UI_Save&Load")]
    public GameObject savePanel;
    public GameObject loadPanel;
    public Transform saveSlotGrid;
    public SaveLoadSlot[] saveSlots;
    public Transform loadSlotGrid;
    public SaveLoadSlot[] loadSlots;
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
    public TextMeshProUGUI collectionSubText0;
    public TextMeshProUGUI collectionNameText;
    public Sprite collectionNullImage;
    public Image[] collectionCardImages;
    public Sprite[] collectionCardSprites;
    public Button[] collectionScrollArrows;
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
    void Update(){
            if(Input.GetButtonUp("Cancel") && PlayerManager.instance.canMove){
                menuPanel.SetActive(!menuPanel.activeSelf);
            }

            //Debug.Log(curPopUpType);
    }
    void Start(){

#region Reset Collection
        totalPage = DBManager.instance.endingCollectionSprites.Length;
        ResetCardOrder();

        collectionScrollArrows[0].GetComponent<Button>().onClick.AddListener(()=>CollectionScrollRightBtn());
        collectionScrollArrows[1].GetComponent<Button>().onClick.AddListener(()=>CollectionScrollLeftBtn());
        
#endregion

#region Reset Save&Load
        
        for(int i=0;i<3;i++){
            int temp = i;
            btns[temp].GetComponent<Button>().onClick.AddListener(()=>OpenPanel(temp));
        }

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
    
    }

    public void OpenLoadPanel(){
    
    }
    
    public void OpenCollectionPanel(){

    }


    // void Update(){
    //     if(animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Collection_Scroll_Right") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime>=1f){
    //         RearrangeCardOrder();
    //         Debug.Log("33");
    //     }
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
        for(int i=0;i<5;i++){

            if(DBManager.instance.CheckEndingCollectionOver(tempCardNum[i])){
                collectionCardImages[i].sprite = DBManager.instance.endingCollectionSprites[tempCardNum[i]];//collectionCardSprites[tempCardNum[i]];
                //collectionNameText.text = DBManager.instance.cache_EndingCollectionDataList[i].name;
            }
            else{

                collectionCardImages[i].sprite = collectionNullImage;//collectionCardSprites[tempCardNum[i]];
                //collectionNameText.text = "???";
            }



        }

    
        if(DBManager.instance.CheckEndingCollectionOver(tempCardNum[2])){
            collectionNameText.text = DBManager.instance.cache_EndingCollectionDataList[tempCardNum[2]].name;
            collectionSubText0.text = "획득 : "+DBManager.instance.cache_EndingCollectionDataList[tempCardNum[2]].clearedCount +"번 째 플레이";
        }
        else{                
            collectionSubText0.text = "미획득";
            collectionNameText.text = "???";

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
                saveSlots[i].saveNameText.text = DBManager.instance.GetData(i).curMapName;
                saveSlots[i].saveDateText.text = DBManager.instance.GetData(i).curPlayDate;
                saveSlots[i].itemInfoText0.text = DBManager.instance.GetData(i).curHoneyAmount.ToString();
                saveSlots[i].itemInfoText1.text = string.Format("{0:N0}", DBManager.instance.GetData(i).curDirtAmount/DBManager.instance.maxDirtAmount);
            }
            else{
                saveSlots[i].saveNameText.text = "빈 슬롯";
                saveSlots[i].saveDateText.text = "-";
                saveSlots[i].itemInfoText0.text ="-";
                saveSlots[i].itemInfoText1.text = "-";
            }
        }
    }
    public void ResetLoadSlots(){
        for(int i=0; i<loadSlotGrid.childCount; i++){
            if(DBManager.instance.CheckSaveFile(i)){
                loadSlots[i].saveNameText.text = DBManager.instance.GetData(i).curMapName;
                loadSlots[i].saveDateText.text = DBManager.instance.GetData(i).curPlayDate;
                loadSlots[i].itemInfoText0.text = DBManager.instance.GetData(i).curHoneyAmount.ToString();
                loadSlots[i].itemInfoText1.text = string.Format("{0:N0}", 100*DBManager.instance.GetData(i).curDirtAmount/DBManager.instance.maxDirtAmount) + "%";
                //loadSlots[i].itemInfoText1.text = DBManager.instance.GetData(i).curDirtAmount.ToString();
            }
            else{
                loadSlots[i].saveNameText.text = "빈 슬롯";
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

            default:
                Debug.Log(curPopUpType);
                popUpOkayCheck = true;
                Invoke("ResetPopUpOkayCheck",0.5f);
                break;

        }

    }
    public void ResetPopUpOkayCheck(){
        popUpOkayCheck = false;

    }
    public void Save(int curSaveNum){
        DBManager.instance.CallSave(curSaveNum);
        saveSlots[curSaveNum].saveNameText.text = DBManager.instance.curData.curMapName;
        saveSlots[curSaveNum].saveDateText.text = DBManager.instance.curData.curPlayDate;
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
    public void OpenPanel(int panelNum){
        CloseAllPanels();

        switch(panelNum){
            case 0 :
                ResetSaveSlots();
                break;
            case 1 :
                ResetLoadSlots();
                break;
        }

        panels[panelNum].SetActive(true);
        
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


}
