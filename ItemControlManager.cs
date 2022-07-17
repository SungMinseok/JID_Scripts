using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemControlManager : MonoBehaviour
{
    public static ItemControlManager instance;
    //public GameObject[] honeyObjects;
    // Start is called before the first frame update
    public Transform itemMother;
    public Transform dirtBundleMother;
    public List<DirtScript> dirtScriptList;
    void Awake(){
        instance = this;
    }
    void Start()
    {
        for(int i=0;i<itemMother.childCount;i++){
            itemMother.GetChild(i).GetComponent<ItemScript>().objectID = i;
        }

        for(int i=0;i<dirtBundleMother.childCount;i++){
            dirtBundleMother.GetChild(i).GetComponent<DirtScript>().dirtBundleInfo.objectID = i;
            dirtScriptList.Add(dirtBundleMother.GetChild(i).GetComponent<DirtScript>());
        }

        
        //for(int i=0;i<dirtBundleMother.childCount;i++){
        // foreach(DirtScript a in dirtScriptList){
        //     dirtScriptList.Add(a);
        // }

        SetItemActivationState();
    }

    void SetItemActivationState(){
        
        for(int i=0;i<DBManager.instance.curData.getItemOverList.Count;i++){
                int id = DBManager.instance.curData.getItemOverList[i];
                itemMother.GetChild(id).gameObject.SetActive(false);
        }
        for(int i=0;i<DBManager.instance.curData.dirtBundleInfoList.Count;i++){
            int id = DBManager.instance.curData.dirtBundleInfoList[i].objectID;
            int hp = DBManager.instance.curData.dirtBundleInfoList[i].curHp;
            float recreateCoolTime = DBManager.instance.curData.dirtBundleInfoList[i].recreateCoolTime;
            if(hp==0){
                dirtBundleMother.GetChild(id).gameObject.SetActive(false);

            }
            else{
                dirtBundleMother.GetChild(id).GetComponent<DirtScript>().dirtBundleInfo.curHp = hp;
                dirtBundleMother.GetChild(id).GetComponent<DirtScript>().ResetSprite();

                dirtBundleMother.GetChild(id).GetComponent<DirtScript>().dirtBundleInfo.recreateCoolTime = recreateCoolTime;

            }
        }
    }
}
