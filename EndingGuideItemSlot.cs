using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingGuideItemSlot : MonoBehaviour
{
    public int itemID;
    public int itemGetMapID;
    public Image itemImage;
    public Image triggerImage;
    public bool onX;
    GameObject xImage;
    public bool isTrigger;
    //public Text itemNameText;
    //TranslateText translateText;
    void Awake(){
        itemImage = transform.GetChild(0).GetComponent<Image>();
        xImage = transform.GetChild(1).gameObject;
        itemImage.sprite = DBManager.instance.cache_ItemDataList[itemID].icon;
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

            if(InventoryManager.instance.CheckHaveItem(itemID)){
                itemImage.color = new Color(1,1,1);
            }
            else{
                itemImage.color = new Color(0,0,0);
            }
        }
        else if(isTrigger){
            
            if(DBManager.instance.CheckTrigOver(itemID)){
                triggerImage.color = new Color(1,1,1);
            }
            else{
                triggerImage.color = new Color(0,0,0);
            }
        }
    }
}
