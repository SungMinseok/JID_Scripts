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
    [Header("[Game Info]━━━━━━━━━━━━━━━━━━━━━━━━━━━")]
    public uint buildNum;
    public string buildDate;
    public string buildVersion;
    public string language;
    [Header("[Game]")]
    
    [Header("[Game Settings]━━━━━━━━━━━━━━━━━━━━━━━━━━━")]
    public float maxDirtAmount;
    public float dirtAmountPaneltyPerSeconds;
    [Header("[Dialogue]")]
    public float waitTime_dialogueInterval;
    public float waitTime_dialogueTypingInterval;
    [Header("[Current Data]━━━━━━━━━━━━━━━━━━━━━━━━━━━")]

    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    public Data curData;

    [Header("[Local Data]━━━━━━━━━━━━━━━━━━━━━━━━━━━")]

    public LocalData localData;
    [Header("[Cache]━━━━━━━━━━━━━━━━━━━━━━━━━━━")]
    public List<Item> cache_ItemDataList;
    public List<EndingCollection> cache_EndingCollectionDataList;
    public List<GameEndList> cache_GameEndDataList;
    
    [Header("[Sprites Files]━━━━━━━━━━━━━━━━━━━━━━━━━━━")]
    public Sprite[] endingCollectionSprites;
    public Sprite[] itemSprites;
    public Sprite honeySprite;

    //public List<Item> cache_ItemDataList;




    
    [Header("[Empty Data]━━━━━━━━━━━━━━━━━━━━━━━━━━━")]
    public Data emptyData;




    [System.Serializable]
    public class Data{
        //public List<int> endingCollectionOverList = new List<int>();

        public float playerX, playerY;
        //public int curMapIndex;
        public int curMapNum;
        public string curPlayDate;
        public float curPlayTime;
        public int curPlayCount;
        public float curDirtAmount;
        public int curHoneyAmount;
        public int[] curEquipmentsID = new int[3];




        

        [Space]
        public List<ItemList> itemList;// = new List<ItemList>();  //현재 보유한 아이템 ID 저장
        public List<int> trigOverList;
        public List<int> getItemOverList;


        //public Vector2 screenSize;

        // public Data DeepCopy(){
        //     Data newCopy = new Data();
        //     newCopy.playerX = this.playerX;
        //     newCopy.playerY = this.playerY;
        //     newCopy.curMapNum = this.curMapNum;
        //     newCopy.curPlayDate = this.curPlayDate;
        //     newCopy.curPlayTime = this.curPlayTime;
        //     newCopy.curPlayCount = this.curPlayCount;
        //     newCopy.curDirtAmount = this.curDirtAmount;
        //     newCopy.curHoneyAmount = this.curHoneyAmount;
        //     newCopy.curEquipmentsID = this.curEquipmentsID;
        //     newCopy.itemList = this.itemList;
        //     newCopy.trigOverList = this.trigOverList;
        //     return newCopy;
        // }
    }
    
    [System.Serializable]//컬렉션 등(영구 저장_컴퓨터 귀속)
    public class LocalData{
        public List<ClearedEndingCollection> endingCollectionOverList = new List<ClearedEndingCollection>();

        
        [Header("Options")]
        public float bgmVolume;
        public float sfxVolume;
        public bool onTalkingSound;
        public int languageValue;
        public bool isWindowedMode;
        public int resolutionValue;
        public int frameRateValue;
        
    }
    public void SaveDefaultData(){

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveFileDefault.dat");//nickName!="" ? File.Create(Application.persistentDataPath + "/SaveFile_"+nickName+".dat"): 

        Debug.Log(Application.persistentDataPath);
        bf.Serialize(file, emptyData);
        file.Close();
    }
    public void LoadDefaultData(){
        
        BinaryFormatter bf = new BinaryFormatter();
        FileInfo fileCheck = new FileInfo(Application.persistentDataPath + "/SaveFileDefault.dat");

        if(fileCheck.Exists){
            FileStream file = File.Open(Application.persistentDataPath + "/SaveFileDefault.dat", FileMode.Open);
        
            if(file != null && file.Length >0){
                curData =(Data)bf.Deserialize(file);

                curData.curPlayCount ++;
            }
            
            file.Close();
        }
    }
    public void CallSave(int fileNum){

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveFile" + fileNum.ToString() +".dat");//nickName!="" ? File.Create(Application.persistentDataPath + "/SaveFile_"+nickName+".dat"): 
        
        //curData.
        curData.playerX = PlayerManager.instance.transform.position.x;
        curData.playerY = PlayerManager.instance.transform.position.y;
        curData.curEquipmentsID = PlayerManager.instance.equipments_id;

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

                curData.curPlayCount ++;

                if(curData.getItemOverList == null){
                    //Debug.Log("1111");
                    curData.getItemOverList = new List<int>();
                    //Debug.Log("2222");

                }
                //curData.honeyOverList.Add(-1);
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

        localData.endingCollectionOverList.Clear();

        BinaryFormatter bf = new BinaryFormatter();
        FileInfo fileCheck = new FileInfo(Application.persistentDataPath + "/LocalDataFile.dat");

        if(fileCheck.Exists){
            FileStream file = File.Open(Application.persistentDataPath + "/LocalDataFile.dat", FileMode.Open);
        
            if(file != null && file.Length >0){
                localData =(LocalData)bf.Deserialize(file);
            }

            //language = localData.language;
            
            file.Close();
        }

    }
    public void ResetLocalData(){

        BinaryFormatter bf = new BinaryFormatter();
        FileInfo fileCheck = new FileInfo(Application.persistentDataPath + "/LocalDataFile.dat");

        if(fileCheck.Exists){
            File.Delete(Application.persistentDataPath + "/LocalDataFile.dat");
            localData.endingCollectionOverList.Clear();
            Debug.Log("로컬 파일 초기화 성공");
            //MenuManager.instance.ResetLoadSlots();

            //file.Close();
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
    public bool CheckCompletedTrigs(int trigNum, int[] triggerNums, bool printDebug = true){
        List<int> tempList = new List<int>();

        for(int i=0;i<triggerNums.Length;i++){
            if(!DBManager.instance.CheckTrigOver(triggerNums[i])){
               // Debug.Log(trigNum +"번 트리거 실행 실패 : " + triggerNums[i] + "번 트리거 완료되지 않음");
               if(printDebug) Debug.Log(string.Format("TrigNum.{0} 실행 실패 : TrigNum.{1} 실행 미완료",trigNum, triggerNums[i]));
                tempList.Add(triggerNums[i]);
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
    public bool CheckIncompletedTrigs(int trigNum, int[] triggerNums, bool printDebug = true){
        List<int> tempList = new List<int>();

        for(int i=0;i<triggerNums.Length;i++){
            if(DBManager.instance.CheckTrigOver(triggerNums[i])){
                //Debug.Log(trigNum +"번 트리거 실행 실패 : " + triggerNums[i] + "번 트리거 완료됨");
                if(printDebug) Debug.Log(string.Format("TrigNum.{0} 실행 실패 : TrigNum.{1} 실행 완료",trigNum, triggerNums[i]));

                tempList.Add(triggerNums[i]);
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
    public bool CheckHaveItems(int trigNum, int[] haveItemNums, bool printDebug = true){
        List<int> tempList = new List<int>();

        for(int i=0;i<haveItemNums.Length;i++){
            if(!InventoryManager.instance.CheckHaveItem(haveItemNums[i])){
                //Debug.Log(trigNum +"번 트리거 실행 실패 : " + haveItemNums[i] + "번 아이템 미보유");
                if(printDebug) Debug.Log(string.Format("TrigNum.{0} 실행 실패 : itemID.{1}({2}) 미보유",trigNum, haveItemNums[i], DBManager.instance.cache_ItemDataList[haveItemNums[i]].name));

                tempList.Add(haveItemNums[i]);
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
        if(GetClearedEndingCollectionID(collectionNum) == -1){
            localData.endingCollectionOverList.Add(new ClearedEndingCollection(collectionNum,DBManager.instance.curData.curPlayDate.Substring(0,10),DBManager.instance.curData.curPlayCount));
            CallLocalDataSave();
        }
    }
    //엔딩 컬렉션 달성되었는지 확인
    public int GetClearedEndingCollectionID(int collectionNum){
        if(localData.endingCollectionOverList.Count == 0){
            return -1;
        }
        //Debug.Log(localData.endingCollectionOverList.Count);
        // if(localData.endingCollectionOverList.FindIndex(x => x.ID == collectionNum) != -1){
        //     return true;
        // }
        // else{
        //     return false;
        // }
        //ebug.Log(localData.endingCollectionOverList.FindIndex(x => x.ID == collectionNum));
        return localData.endingCollectionOverList.FindIndex(x => x.ID == collectionNum);
    }
    public void ResetEndingCollection(){
        localData.endingCollectionOverList.Clear();
        CallLocalDataSave();
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



        //Application.targetFrameRate = 60;
        //QualitySettings.vSyncCount = 0;

#if demo
        buildVersion = "demo";
#elif alpha
        buildVersion = "alpha";
#else
        buildVersion = "";
#endif

        SaveDefaultData();
        CallLocalDataLoad();

        //ApplyNewLanguage();
        
        ApplyItemInfo();
        ApplyCollectionInfo();
        //ApplyStoryInfo();
        //CallLocalDataLoad();
    }
    public void ApplyNewLanguage(bool resetUI){

        ApplyItemInfo();
        ApplyCollectionInfo();
        MenuManager.instance.ResetSaveSlots();
        MenuManager.instance.ResetLoadSlots();
        //ApplyStoryInfo();
        //ApplySysMsgText();


        if(resetUI){
            if(InventoryManager.instance != null) InventoryManager.instance.ResetInventory();

            if(MenuManager.instance != null){
                MenuManager.instance.ResetCardOrder();
                if(MenuManager.instance.menuPanel.activeSelf){

                    MenuManager.instance.menuPanel.SetActive(false);
                    MenuManager.instance.menuPanel.SetActive(true);
                }
                MenuManager.instance.settingPanel.SetActive(false);
                MenuManager.instance.settingPanel.SetActive(true);
            }
        }
    }

    void Start()
    {
    }
    void Update(){
        //System.DateTime dateTime = System.DateTime.Now;
        curData.curPlayDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }

    void FixedUpdate(){
        curData.curPlayTime += Time.fixedDeltaTime;
        

        if(PlayerManager.instance != null){

            if(PlayerManager.instance.isMoving && !PlayerManager.instance.isInvincible){
                if(curData.curDirtAmount>0) curData.curDirtAmount -= dirtAmountPaneltyPerSeconds;
            }
        }
    }
    
    void ApplyItemInfo(){
        //var a = TextLoader.instance.dictionaryItemText;
        var a = CSVReader.instance.data_item;
        cache_ItemDataList.Clear();
        for(int i=0; i<a.Count; i++){
            //itemDataList.Add(new Item(a[i].ID,a[i].name_kr,a[i].desc_kr,a[i].type,a[i].resourceID,a[i].isStack));
            cache_ItemDataList.Add(new Item(
                int.Parse(a[i]["ID"].ToString())
                ,a[i]["name_"+language].ToString()
                ,a[i]["desc_"+language].ToString()
                ,byte.Parse(a[i]["type"].ToString())
                ,int.Parse(a[i]["resourceID"].ToString())
                ,bool.Parse(a[i]["isStack"].ToString())
                ,int.Parse(a[i]["price"].ToString())
                ,int.Parse(a[i]["goldResourceID"].ToString())
            ));
        }
    }
    
    void ApplyCollectionInfo(){
        //var a = TextLoader.instance.dictionaryItemText;
        var a = CSVReader.instance.data_collection;
        cache_EndingCollectionDataList.Clear();

        for(int x=0; x<a.Count; x++){
            //itemDataList.Add(new Item(a[i].ID,a[i].name_kr,a[i].desc_kr,a[i].type,a[i].resourceID,a[i].isStack));

            for(int i=0; i<a.Count; i++){
                if(int.Parse(a[i]["sortOrder"].ToString())==x){

                    cache_EndingCollectionDataList.Add(new EndingCollection(
                        int.Parse(a[i]["ID"].ToString())
                        ,a[i]["name_"+language].ToString()
                        ,int.Parse(a[i]["resourceID"].ToString())
                        //byte.Parse(a[i]["type"].ToString()),
                        //int.Parse(a[i]["resourceID"].ToString()),
                        //bool.Parse(a[i]["isStack"].ToString())
                    ));

                    break;
                }

            }
        }
    }

    public void ApplyStoryInfo(){
        var reader0 = CSVReader.instance.data_collection;
        var reader1 = CSVReader.instance.data_story;
        //cache_GameEndDataList.Clear();
        for(int i=0;i<cache_GameEndDataList.Count;i++){
            cache_GameEndDataList[i].name = reader0[cache_GameEndDataList[i].endingCollectionNum]["name_"+language].ToString();

            for(int j=0;j<cache_GameEndDataList[i].stories.Length;j++){
//                Debug.Log(cache_GameEndDataList[i].stories.Length);
                cache_GameEndDataList[i].stories[j].descriptions = reader1[int.Parse(cache_GameEndDataList[i].stories[j].descriptions)]["text_"+language].ToString();
            }

        }
    }

    // public void ApplySysMsgText(){
    //     // for(int i=0;i<SceneController.instance.translateTexts.Count;i++){
    //     //     SceneController.instance.translateTexts[i].tra
    //     // }
    //     foreach(TranslateText t in SceneController.instance.translateTexts){
    //         t.ApplyTranslation();
    //     }
    
    // }

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
    public int price;    
    public int goldResourceID;
    public Sprite goldIcon;
    //public bool isStack;

    public enum ItemType{
        equip,
    }
    public Item(int a, string b, string c, byte d, int e, bool f, int g, int h){
        ID = a;
        name = b;
        description = c;
        type = d;
        resourceID = e;
        icon = DBManager.instance.itemSprites[resourceID];
        isStack = f;
        price = g;
        goldResourceID = h;
        goldIcon = DBManager.instance.itemSprites[goldResourceID];
    }
}



[System.Serializable]
public class EndingCollection{
    public int ID;
    public string name;
    public string description;

    
    public int resourceID;
    public Sprite sprite;


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
public class ClearedEndingCollection{
    public int ID;
    
    public string clearedDate;
    public int clearedPlayCount;

    public ClearedEndingCollection(int _ID, string _clearedDate, int _clearedPlayCount){
        ID = _ID;
        clearedDate = _clearedDate;
        clearedPlayCount = _clearedPlayCount;
        //description = c;
        //date = d;
        //count = e;
        // resourceID = c;
        // if(DBManager.instance.endingCollectionSprites.Length > resourceID)
        // sprite = DBManager.instance.endingCollectionSprites[resourceID];
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

[System.Serializable]
public class GameEndList{
    public string comment;
    public int endingCollectionNum; //컬렉션 내 엔딩 넘버(data_collection.csv) 컬렉션 UI 내 순서
    public int endingNum; //찐엔딩 넘버(임의의 값)
    public string name;
    public Story[] stories;
    //public Sprite[] storySprites;
    //public string[] storyDescriptions;
    // public ItemList(int _itemID, int _itemAmount){
    //     itemID = _itemID;
    //     itemAmount = _itemAmount;
    // }
}
[System.Serializable]
public class Story{
    public Sprite sprite;
    //[TextArea(2,4)]
    public string soundFileName;
    public string descriptions;
}