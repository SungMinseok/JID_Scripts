using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemSlot2 : MonoBehaviour{
    public Image itemImage;
    public Text itemAmountText;
    
    void Start(){
        // itemImage = this.transform.GetChild(0).GetComponent<Image>();
        // itemAmountText = this.transform.GetChild(1).GetComponent<Text>();
        // Debug.Log(MenuManager.instance);
        
        // itemImage.sprite = MenuManager.instance.nullSprite;
        // itemAmountText.text = string.Empty;
    }
}
