using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SaveLoadSlot : MonoBehaviour{
    public Text saveNameText;
    public Text playTimeText;
    public Text saveDateText;
    public Text slotNumText;
    public Text itemInfoText0;//꿀
    public Text itemInfoText1;//흙 (%)
    public Transform itemSlotGrid;
    public Button deleteFileBtn;
    void Start(){
        slotNumText.text = (transform.GetSiblingIndex()+1).ToString();
        Debug.Log(DBManager.instance);
        //deleteFileBtn.onClick.AddListener(()=>DBManager.instance.DeleteSaveFile(transform.GetSiblingIndex()));
        deleteFileBtn.onClick.AddListener(()=>MenuManager.instance.TryDeleteSaveFile(transform.GetSiblingIndex()));
        //deleteFileBtn.onClick.AddListener(delegate{DBManager.instance.DeleteSaveFile(1);});

    }
    void DeleteSaveFile(int fileNum){
        DBManager.instance.DeleteSaveFile(fileNum);
    }
}