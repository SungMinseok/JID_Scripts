using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StickerScript : MonoBehaviour
{
    public byte stickerID;//sysmsg index
    public string resourceName;
    public bool isItem; 
    [Header("private")]
    public Button button;
    public Image image;
    public GameObject lockedObject;
    public GameObject redDot;
    [Header("DB")]
    public int trueID; //db내 앤트컬렉션오버리스트의 index

    void Awake(){
        lockedObject = transform.GetChild(0).gameObject;
        if(transform.childCount>=2){
            redDot = transform.GetChild(1).gameObject;

            if(!isItem){
                trueID = DBManager.instance.GetClearedAntCollectionIndex(stickerID);
            }
            else{
                trueID = DBManager.instance.GetClearedItemCollectionIndex(stickerID);
            }
        }
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        if(!isItem){

            var sprites = MenuManager.instance.antSprites;
            image.sprite = sprites[Array.FindIndex(sprites, x =>x.name == "antglass_"+ resourceName)];
        }
        else{

            redDot.SetActive(false);
        }

    }
    // void Start(){
        
    //     if(DBManager.instance.GetClearedAntCollectionIndex(stickerID)!=-1){
    //         ActivateSticker();
    //     }
    //     else{
    //         DeactivateSticker();
    //     }
    // }
    void OnEnable(){
        if(!isItem){

            if(DBManager.instance.GetClearedAntCollectionIndex(stickerID)!=-1){
                ActivateSticker();
            }
            else{
                DeactivateSticker();
            }
        }
        //ITEM
        else{
            
            if(DBManager.instance.GetClearedItemCollectionIndex(stickerID)!=-1){
                ActivateSticker();
            }
            else{
                DeactivateSticker();
            }
        }
    }
    public void ActivateSticker(){
        image.color = new Color (1,1,1,1);

        if(!isItem){

            button.interactable = true;
            lockedObject.SetActive(false);
            
            if(!DBManager.instance.localData.antCollectionOverList[trueID].isRecognized){
                redDot.SetActive(true);

                if(UIManager.instance!=null)
                    UIManager.instance.hud_sub_collection_redDot.SetActive(true);
            }
            else{
                redDot.SetActive(false);
            }
        }
        else{
            
            button.interactable = true;
            lockedObject.SetActive(false);
            
            // if(!DBManager.instance.localData.itemCollectionOverList[trueID].isRecognized){
            //     redDot.SetActive(true);

            //     if(UIManager.instance!=null)
            //         UIManager.instance.hud_sub_collection_redDot.SetActive(true);
            // }
            // else{
            //     redDot.SetActive(false);
            // }
        }
        // else{
            
        //     if(!DBManager.instance.localData.itemCollectionOverList[trueID].isRecognized){
        //         redDot.SetActive(true);
        //         UIManager.instance.hud_sub_collection_redDot.SetActive(true);
        //     }
        //     else{
        //         redDot.SetActive(false);
        //     }
        // }
//        Debug.Log("있음 " + gameObject.name);
    }
    public void DeactivateSticker(){

        image.color = new Color (0,0,0,1);
        button.interactable = false;
        //if(!isItem){
            lockedObject.SetActive(true);
                
            redDot.SetActive(false);
        //}
    }
    public void ClickSticker(){
        var sprites = MenuManager.instance.antSprites;

        redDot.SetActive(false);

        //전부 인식 완료됐으면 메인 레드닷 제거
        if(UIManager.instance!=null && UIManager.instance.CheckCollectionOverListAllRecognized()){
            UIManager.instance.hud_sub_collection_redDot.SetActive(false);
        }

        if(!isItem){

            DBManager.instance.localData.antCollectionOverList[trueID].isRecognized = true;
            MenuManager.instance.antMainNameTextHolderObj.SetActive(true);
            MenuManager.instance.antMainImage.sprite = sprites[Array.FindIndex(sprites, x =>x.name == "antglass_"+ resourceName)];
            MenuManager.instance.antMainNameText.text = CSVReader.instance.GetIndexToString(stickerID + 300,"sysmsg");
        }
        // else{
        //     DBManager.instance.localData.itemCollectionOverList[trueID].isRecognized = true;

        // }
    }
}
