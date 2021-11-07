﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
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
    [Header("UI_Save&Load")]
    public Transform saveSlotGrid;
    public SaveLoadSlot[] saveSlots;
    public Transform loadSlotGrid;
    public SaveLoadSlot[] loadSlots;
    [System.Serializable]
    public class SaveLoadSlot{
        public TextMeshProUGUI saveNameText;
        public TextMeshProUGUI saveDateText;
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
            if(Input.GetButtonUp("Cancel")){
                menuPanel.SetActive(!menuPanel.activeSelf);
                
            }
    }
    void Start(){

#region Collection
        totalPage = DBManager.instance.endingCollectionSprites.Length;
        ResetCardOrder();
#endregion

#region Save&Load

        //saveSlots = new SaveLoadSlot[saveSlotGrid.childCount];
        //loadSlots = new SaveLoadSlot[loadSlotGrid.childCount];

        
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
        }
        for(int i=0;i<loadSlotGrid.childCount;i++){
            loadSlots[i].saveNameText = loadSlotGrid.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
            loadSlots[i].saveDateText = loadSlotGrid.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>();
        }

        ResetSaveSlots();
        ResetLoadSlots();
#endregion
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
        Debug.Log(tempCardNum[0]+","+tempCardNum[1]+","+tempCardNum[2]+","+tempCardNum[3]+","+tempCardNum[4]);
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
            collectionSubText0.text = "획득 : "+DBManager.instance.cache_EndingCollectionDataList[tempCardNum[2]].count +"번 째 플레이";
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
            }
            else{
                saveSlots[i].saveNameText.text = "빈 슬롯";
                saveSlots[i].saveDateText.text = "-";
            }
        }
    }
    public void ResetLoadSlots(){
        for(int i=0; i<loadSlotGrid.childCount; i++){
            if(DBManager.instance.CheckSaveFile(i)){
                loadSlots[i].saveNameText.text = DBManager.instance.GetData(i).curMapName;
                loadSlots[i].saveDateText.text = DBManager.instance.GetData(i).curPlayDate;
            }
            else{
                loadSlots[i].saveNameText.text = "빈 슬롯";
                loadSlots[i].saveDateText.text = "-";
            }
        }
    }
    //저장 슬롯 터치 시
    public void TrySave(int num){
        curSaveNum = num;

        if(DBManager.instance.CheckSaveFile(num)){
            OpenPopUpPanel("save");
        }
        else{
            Save(curSaveNum);
        }
        
    }
    public void TryLoad(int num){
        curLoadNum = num;

        if(DBManager.instance.CheckSaveFile(num)){
            OpenPopUpPanel("load");
        }
        else{
            //Load(curLoadNum);
        }
        
    }
    public void OpenPopUpPanel(string type){
        curPopUpType = type;
        //확인
        popUpText[2].text ="2";
        //취소
        popUpText[3].text ="3";
        switch(type){
            case "save" :
                popUpText[0].text = "0";
                popUpText[1].text = "1";
                break;
            case "load" :
                popUpText[0].text = "0";
                popUpText[1].text = "1";
                break;
            default :
                break;
        }

        for(int i=0;i<4;i++){
            popUpText[i].text = CSVReader.instance.GetIndexToString(int.Parse(popUpText[i].text),"sysmsg");
        }

        popUpPanel.SetActive(true);
    }
    public void PopUpOkayBtn(){
        switch(curPopUpType){
            case "save" :
                Save(curSaveNum);
                //Debug.Log(curSaveNum + "번 저장 시도");
                break;

            case "load" :
                Load(curLoadNum);
                //Debug.Log(curSaveNum + "번 저장 시도");
                break;
        }

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
