using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StickerScript : MonoBehaviour
{
    public byte stickerID;//sysmsg index
    public string resourceName;
    public Button button;
    public Image image;
    public GameObject lockedObject;

    void Awake(){
        lockedObject = transform.GetChild(0).gameObject;
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        var sprites = MenuManager.instance.antSprites;
        image.sprite = sprites[Array.FindIndex(sprites, x =>x.name == "antglass_"+ resourceName)];

    }
    void Start(){
        
        if(DBManager.instance.GetClearedAntCollectionIndex(stickerID)!=-1){
            ActivateSticker();
        }
        else{
            DeactivateSticker();
        }
    }
    // void OnEnable(){
    //     if(){
    //         ActivateSticker();
    //     }
    //     else{
    //         DeactivateSticker();
    //     }
    // }
    public void ActivateSticker(){
        button.interactable = true;
        image.color = new Color (1,1,1,1);
        lockedObject.SetActive(false);
//        Debug.Log("있음 " + gameObject.name);
    }
    public void DeactivateSticker(){

        button.interactable = false;
        image.color = new Color (0,0,0,1);
        lockedObject.SetActive(true);
    }
    public void ClickSticker(){
        var sprites = MenuManager.instance.antSprites;

        MenuManager.instance.antMainNameTextHolderObj.SetActive(true);
        MenuManager.instance.antMainImage.sprite = sprites[Array.FindIndex(sprites, x =>x.name == "antglass_"+ resourceName)];
        MenuManager.instance.antMainNameText.text = CSVReader.instance.GetIndexToString(stickerID + 300,"sysmsg");
    }
}
