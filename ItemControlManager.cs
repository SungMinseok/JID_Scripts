using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemControlManager : MonoBehaviour
{
    //public GameObject[] honeyObjects;
    // Start is called before the first frame update
    public Transform itemMother;
    public Transform dirtBundleMother;
    void Start()
    {
        for(int i=0;i<itemMother.childCount;i++){
            itemMother.GetChild(i).GetComponent<ItemScript>().objectID = i;
        }

        for(int i=0;i<dirtBundleMother.childCount;i++){
            dirtBundleMother.GetChild(i).GetComponent<DirtScript>().dirtBundleInfo.objectID = i;
        }

        SetItemActivationState();
    }

    void SetItemActivationState(){
        
        for(int i=0;i<DBManager.instance.curData.getItemOverList.Count;i++){
                int id = DBManager.instance.curData.getItemOverList[i];
                itemMother.GetChild(id).gameObject.SetActive(false);
        }
        for(int i=0;i<DBManager.instance.curData.getDirtBundleOverList.Count;i++){
            int id = DBManager.instance.curData.getDirtBundleOverList[i].objectID;
            int hp = DBManager.instance.curData.getDirtBundleOverList[i].curHp;
            if(hp==0){
                dirtBundleMother.GetChild(id).gameObject.SetActive(false);

            }
            else{
                dirtBundleMother.GetChild(id).GetComponent<DirtScript>().dirtBundleInfo.curHp = hp;
                dirtBundleMother.GetChild(id).GetComponent<DirtScript>().ResetSprite();

            }
        }
    }
}
