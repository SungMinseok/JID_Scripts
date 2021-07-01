using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 
public class DBManager : MonoBehaviour
{
    public static DBManager instance;
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

    }

    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);
}