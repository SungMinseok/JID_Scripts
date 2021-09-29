using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 
public class DBManager : MonoBehaviour
{
    public static DBManager instance;
    public List<Item> itemDataList;









    [System.Serializable]
    public class Data{
        public List<int> trigOverList = new List<int>();
    }
    public Data data;
    public void CallSave(){

    }
    public void CallLoad(){

    }
    public void TrigOver(int trigNum){
        if(!CheckTrigOver(trigNum)){
            data.trigOverList.Add(trigNum);
        }
    }
    public bool CheckTrigOver(int trigNum){
        if(data.trigOverList.Contains(trigNum)){
            return true;
        }
        else{
            return false;
        }
    }
    public bool CheckCompletedTrigs(int trigNum, int[] completedTriggerNums){
        List<int> tempList = new List<int>();

        for(int i=0;i<completedTriggerNums.Length;i++){
            if(!DBManager.instance.CheckTrigOver(completedTriggerNums[i])){
                Debug.Log(trigNum +"번 트리거 실행 실패 : " + completedTriggerNums[i] + "번 트리거 완료되지 않음");
                tempList.Add(completedTriggerNums[i]);
            }
        }

        if(tempList.Count != 0){
            tempList.Clear();
            return false;
        }
        else{
            tempList.Clear();
            return true;
        }
    }
    void Awake(){
        //Application.targetFrameRate = 60;
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        ApplyItemInfo();
    }
    
    void ApplyItemInfo(){
        //var a = TextLoader.instance.dictionaryItemText;
        var a = CSVReader.instance.data_item;

        for(int i=0; i<a.Count; i++){
            //itemDataList.Add(new Item(a[i].ID,a[i].name_kr,a[i].desc_kr,a[i].type,a[i].resourceID,a[i].isStack));
            itemDataList.Add(new Item(int.Parse(a[i]["ID"].ToString()),a[i]["name_kr"].ToString(),a[i]["desc_kr"].ToString(),
            byte.Parse(a[i]["type"].ToString()),int.Parse(a[i]["resourceID"].ToString()),bool.Parse(a[i]["isStack"].ToString())));
        }
    }

    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);

    
}