using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemControlManager : MonoBehaviour
{
    //public GameObject[] honeyObjects;
    // Start is called before the first frame update
    public Transform itemMother;
    void Start()
    {
        for(int i=0;i<itemMother.childCount;i++){
            itemMother.GetChild(i).GetComponent<ItemScript>().objectID = i;
        }

        SetItemActivationState();
    }

    void SetItemActivationState(){
        
        // for(int i=0;i<honeyControl.childCount;i++){
        //     if(DBManager.instance.curData.honeyOverList.Contains(i)){
        //         honeyControl.GetChild(i).gameObject.SetActive(false);
        //     }
        //     //honeyControl.GetChild(i).GetComponent<ItemScript>().honeyID = i;
        // }
        for(int i=0;i<DBManager.instance.curData.getItemOverList.Count;i++){
            //if(DBManager.instance.curData.honeyOverList.Contains(i)){
                int id = DBManager.instance.curData.getItemOverList[i];
                itemMother.GetChild(id).gameObject.SetActive(false);
            //}
            //honeyControl.GetChild(i).GetComponent<ItemScript>().honeyID = i;
        }
    }
}
