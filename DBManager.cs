using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class DBManager : MonoBehaviour
{
    public static DBManager instance;
    [Header("[Game Info] ─────────────────────")]
    public uint buildNum;
    public string buildDate;
    public string language;
    [Header("[Current Data] ─────────────────────")]

    public Data curData;

    [Header("[Local Data] ─────────────────────")]

    public LocalData localData;

    [Header("[Game Settings] ─────────────────────")]
    public float maxDirtAmount;
    public float dirtAmountPaneltyPerSeconds;
    [Header("[Cache] ─────────────────────")]
    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    public List<Item> cache_ItemDataList;
    public List<EndingCollection> cache_EndingCollectionDataList;
    
    [Header("[Sprites Files] ─────────────────────")]
    public Sprite[] endingCollectionSprites;
    public Sprite[] itemSprites;
    public Sprite honeySprite;

    //public List<Item> cache_ItemDataList;




    
    [Header("[Empty Data] ─────────────────────")]

    public Data emptyData;




    [System.Serializable]
    public class Data{
        //public List<int> endingCollectionOverList = new List<int>();

        public float playerX, playerY;
        public string curMapName;
        public int curMapNum;
        public string curPlayDate;
        public float curPlayTime;
        public int curPlayCount;
        public float curDirtAmount;
        public float curHoneyAmount;




        

        [Space]
        public List<ItemList> itemList;// = new List<ItemList>();  //현재 보유한 아이템 ID 저장
        public List<int> trigOverList = new List<int>();
    }
    
    [System.Serializable]//컬렉션 등(영구 저장_컴퓨터 귀속)
    public class LocalData{
        public List<int> endingCollectionOverList = new List<int>();
    }
    public void CallSave(int fileNum){

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveFile" + fileNum.ToString() +".dat");//nickName!="" ? File.Create(Application.persistentDataPath + "/SaveFile_"+nickName+".dat"): 
        
        //curData.
        curData.playerX = PlayerManager.instance.transform.position.x;
        curData.playerY = PlayerManager.instance.transform.position.y;

        Debug.Log(Application.persistentDataPath);
        bf.Serialize(file, curData);
        file.Close();
    }
    public void CallLoad(int fileNum){

        BinaryFormatter bf = new BinaryFormatter();
        FileInfo fileCheck = new FileInfo(Application.persistentDataPath + "/SaveFile" + fileNum.ToString() +".dat");

        if(fileCheck.Exists){
            FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + fileNum.ToString() +".dat", FileMode.Open);
        
            if(file != null && file.Length >0){
                curData =(Data)bf.Deserialize(file);
            }
            
            file.Close();
        }

    }
    public bool CheckSaveFile(int fileNum){
        FileInfo fileCheck = new FileInfo(Application.persistentDataPath + "/SaveFile" + fileNum.ToString() +".dat");

        if(fileCheck.Exists){
            //FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + fileNum.ToString() +".dat", FileMode.Open);
            return true;
        }
        else{
            return false;
        }
    }
    
    public Data GetData(int fileNum){
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + fileNum.ToString() +".dat", FileMode.Open);

        var data = (Data)bf.Deserialize(file);

        file.Close();
        
        return data;
    }
    public void DeleteSaveFile(int fileNum){

        BinaryFormatter bf = new BinaryFormatter();
        FileInfo fileCheck = new FileInfo(Application.persistentDataPath + "/SaveFile" + fileNum.ToString() +".dat");

        if(fileCheck.Exists){
            File.Delete(Application.persistentDataPath + "/SaveFile" + fileNum.ToString() +".dat");

            Debug.Log(fileNum + "번 파일 제거 성공");
            MenuManager.instance.ResetLoadSlots();

            //file.Close();
        }
        else{
            Debug.Log(fileNum + "번 파일 제거 불가 : 파일 없음");
        }
    }
#region LocalData
    public void CallLocalDataSave(){

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/LocalDataFile.dat");//nickName!="" ? File.Create(Application.persistentDataPath + "/SaveFile_"+nickName+".dat"): 
        

        //Debug.Log(Application.persistentDataPath);
        bf.Serialize(file, localData);
        file.Close();
    }
    public void CallLocalDataLoad(){

        BinaryFormatter bf = new BinaryFormatter();
        FileInfo fileCheck = new FileInfo(Application.persistentDataPath + "/LocalDataFile.dat");

        if(fileCheck.Exists){
            FileStream file = File.Open(Application.persistentDataPath + "/LocalDataFile.dat", FileMode.Open);
        
            if(file != null && file.Length >0){
                localData =(LocalData)bf.Deserialize(file);
            }
            
            file.Close();
        }

    }
#endregion

#region Triggers
    public void TrigOver(int trigNum){
        if(!CheckTrigOver(trigNum)){
            curData.trigOverList.Add(trigNum);
        }
    }
    public bool CheckTrigOver(int trigNum){
        if(curData.trigOverList.Contains(trigNum)){
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
#endregion

#region EndingCollection

    //엔딩 컬렉션 달성 등록
    public void EndingCollectionOver(int collectionNum){
        if(!CheckEndingCollectionOver(collectionNum)){
            localData.endingCollectionOverList.Add(collectionNum);
            UIManager.instance.gameOverNewImage.gameObject.SetActive(true);
            //CallLocalDataSave();
        }
    }
    //엔딩 컬렉션 달성되었는지 확인
    public bool CheckEndingCollectionOver(int collectionNum){
        if(localData.endingCollectionOverList.Contains(collectionNum)){
            return true;
        }
        else{
            return false;
        }
    }
#endregion
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

        ApplyItemInfo();
        ApplyCollectionInfo();
        //CallLocalDataLoad();
    }

    void Start()
    {
    }
    void Update(){
        //System.DateTime dateTime = System.DateTime.Now;
        curData.curPlayDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm");
    }

    void FixedUpdate(){
        curData.curPlayTime += Time.fixedDeltaTime;

        if(PlayerManager.instance != null){

            if(PlayerManager.instance.isMoving){
                if(curData.curDirtAmount>0) curData.curDirtAmount -= dirtAmountPaneltyPerSeconds;
            }
        }
    }
    
    void ApplyItemInfo(){
        //var a = TextLoader.instance.dictionaryItemText;
        var a = CSVReader.instance.data_item;

        for(int i=0; i<a.Count; i++){
            //itemDataList.Add(new Item(a[i].ID,a[i].name_kr,a[i].desc_kr,a[i].type,a[i].resourceID,a[i].isStack));
            cache_ItemDataList.Add(new Item(
                int.Parse(a[i]["ID"].ToString()),
                a[i]["name_"+language].ToString(),
                a[i]["desc_"+language].ToString(),
                byte.Parse(a[i]["type"].ToString()),
                int.Parse(a[i]["resourceID"].ToString()),
                bool.Parse(a[i]["isStack"].ToString())
            ));
        }
    }
    
    void ApplyCollectionInfo(){
        //var a = TextLoader.instance.dictionaryItemText;
        var a = CSVReader.instance.data_collection;

        for(int i=0; i<a.Count; i++){
            //itemDataList.Add(new Item(a[i].ID,a[i].name_kr,a[i].desc_kr,a[i].type,a[i].resourceID,a[i].isStack));
            cache_EndingCollectionDataList.Add(new EndingCollection(
                int.Parse(a[i]["ID"].ToString())
                ,a[i]["name_"+language].ToString()
                ,int.Parse(a[i]["resourceID"].ToString())
                //byte.Parse(a[i]["type"].ToString()),
                //int.Parse(a[i]["resourceID"].ToString()),
                //bool.Parse(a[i]["isStack"].ToString())
            ));
        }
    }


    // void OnApplicationQuit(){
        
    //     if(DBManager.instance != null) DBManager.instance.CallSave(0);
    // }
    // void OnApplicationPause(bool pause)
    // {
    //     if(DBManager.instance != null){
                
    //         if (pause)
    //         {
    //             isPaused = true;
    //             DBManager.instance.CallSave(0);
    //             /* 앱이 비활성화 되었을 때 처리 */    
    //         }
    //         else{
    //             if(isPaused){
    //                 isPaused = false;
    //             }
    //         }
    //     }
    // }
    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);

    
}


[System.Serializable]
public class Item{
    public int ID;
    public string name;
    public string description;
    public byte type;
    public int resourceID;
    public bool isStack;
    public Sprite icon;
    //public bool isStack;

    public enum ItemType{
        equip,
    }
    public Item(int a, string b, string c, byte d, int e, bool f){
        ID = a;
        name = b;
        description = c;
        type = d;
        resourceID = e;
        icon = DBManager.instance.itemSprites[resourceID];
        isStack = f;
    }
}



[System.Serializable]
public class EndingCollection{
    public int ID;
    public string name;
    public string description;

    
    public int resourceID;
    public Sprite sprite;

    [Space]
    
    public string clearedDate;
    public string clearedCount;

    public EndingCollection(int a, string b, int c){
        ID = a;
        name = b;
        //description = c;
        //date = d;
        //count = e;
        resourceID = c;
        if(DBManager.instance.endingCollectionSprites.Length > resourceID)
        sprite = DBManager.instance.endingCollectionSprites[resourceID];
    }
}

[System.Serializable]
public class ItemList{
    public int itemID;
    public int itemAmount;
    public ItemList(int _itemID, int _itemAmount){
        itemID = _itemID;
        itemAmount = _itemAmount;
    }
}