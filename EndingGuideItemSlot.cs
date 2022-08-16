
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingGuideItemSlot : MonoBehaviour
{
    public enum EndingGuideSlotType{
        Item,
        Trigger,
    }
    public EndingGuideSlotType slotType;
    public int ID;
    public int getMapID;
    [Header("[If this is item slot]")]
    public Image itemImage;
    public bool onX;
    GameObject xImage;

    [Header("[If this is trigger slot]")]
    public Image triggerImage;
    //public Text itemNameText;
    //TranslateText translateText;




    [Space]
    public bool isTrigger;

    void Awake(){
        if(isTrigger){
            slotType = EndingGuideSlotType.Trigger;
        }

        if(slotType==EndingGuideSlotType.Item){

            itemImage = transform.GetChild(0).GetComponent<Image>();
            itemImage.sprite = DBManager.instance.cache_ItemDataList[ID].icon;
        }
            xImage = transform.GetChild(1).gameObject;
        if(onX){
            xImage.SetActive(true);
        }
        else{
            xImage.SetActive(false);

        }
        //translateText = transform.GetChild(1).GetComponent<TranslateText>();
        //translateText.key = itemID;
    }
    void OnEnable(){
        if(!isTrigger){

            if(InventoryManager.instance.CheckHaveItem(ID)){
                itemImage.color = new Color(1,1,1);
            }
            else{
                itemImage.color = new Color(0,0,0);
            }
        }
        else if(isTrigger){
            
            if(DBManager.instance.CheckTrigOver(ID)){
                triggerImage.color = new Color(1,1,1);
            }
            else{
                triggerImage.color = new Color(0,0,0);
            }
        }
    }
}
