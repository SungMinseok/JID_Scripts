using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Steamworks;
public class DBManager : MonoBehaviour
{
    public static DBManager instance;
    [Header("[Game Info]━━━━━━━━━━━━━━━━━━━━━━━━━━━")]
    public uint buildNum;
    public uint buildSubNum;
    public string buildDate;
    public string buildVersion;
    public string language;
    public string dataDirectory;
    [Header("[Game]")]
    
    [Header("[Game Settings]━━━━━━━━━━━━━━━━━━━━━━━━━━━")]
    public float maxDirtAmount;
    public float dirtAmountPaneltyPerSeconds;
    [Header("[Dialogue]")]
    public float waitTime_dialogueInterval;
    public float waitTime_dialogueTypingInterval;
    [Header("[Sound]")]
    public float bgmAdjustVal;
    public float sfxAdjustVal;
    [Header("[Steam]")]
    public bool achievementIsAvailable;
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
    public List<Coupon> cache_couponList;
    [Header("[Sprites Files]━━━━━━━━━━━━━━━━━━━━━━━━━━━")]
    public Sprite[] endingCollectionSprites;
    //public Sprite[] itemSprites;
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
        public bool isSummoning;






        [Space]
        public List<ItemList> itemList;// = new List<ItemList>();  //현재 보유한 아이템 ID 저장
        public List<int> trigOverList;
        public List<int> getItemOverList; // 맵 내 아이템 활성화/비활성화 체크 용도
        //public List<DirtBundleInfo> getDirtBundleOverList;
        public List<DirtBundleInfo> dirtBundleInfoList;


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
        
        [Header("Collections")]
        public List<ClearedEndingCollection> endingCollectionOverList = new List<ClearedEndingCollection>();
        public List<ClearedAntCollection> antCollectionOverList = new List<ClearedAntCollection>();
        public List<int> itemCollectionOverList = new List<int>();
        
        [Header("Options")]
        public float bgmVolume;
        public float sfxVolume;
        public bool onTalkingSound;
        public int languageValue;
        public bool isWindowedMode;
        public int resolutionValue;
        public int frameRateValue;
        public KeyCode jumpKey;
        public KeyCode interactKey;
        public KeyCode petKey;
        public int usedCouponRewardItemID;
        
    }
    
#region Non local data
    public void SaveDefaultData(){

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + dataDirectory +"/SaveFileDefault.dat");//nickName!="" ? File.Create(Application.persistentDataPath + "/SaveFile_"+nickName+".dat"): 

        Debug.Log(Application.persistentDataPath);
        bf.Serialize(file, emptyData);
        file.Close();
    }
    public void LoadDefaultData(){
        
        BinaryFormatter bf = new BinaryFormatter();
        FileInfo fileCheck = new FileInfo(Application.persistentDataPath + dataDirectory +"/SaveFileDefault.dat");

        if(fileCheck.Exists){
            FileStream file = File.Open(Application.persistentDataPath + dataDirectory +"/SaveFileDefault.dat", FileMode.Open);
        
            if(file != null && file.Length >0){
                curData =(Data)bf.Deserialize(file);

                curData.curPlayCount ++;
            }
            
            file.Close();
        }
    }
    public void CallSave(int fileNum){

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + dataDirectory +"/SaveFile" + fileNum.ToString() +".dat");//nickName!="" ? File.Create(Application.persistentDataPath + "/SaveFile_"+nickName+".dat"): 
        
        //curData.
        curData.playerX = PlayerManager.instance.transform.position.x;
        curData.playerY = PlayerManager.instance.transform.position.y;
        curData.curEquipmentsID = PlayerManager.instance.equipments_id;


        //DirtBundle
        //DirtBundleInfo[] curDirtBundleInfo = ItemControlManager.instance.dirtBundleMother.GetComponentsInChildren<DirtBundleInfo>();

        foreach(DirtScript a in ItemControlManager.instance.dirtScriptList){
            curData.dirtBundleInfoList.Add(a.dirtBundleInfo);
            Debug.Log("dirtbundle info saved completely");
        }

        Debug.Log(Application.persistentDataPath);
        bf.Serialize(file, curData);
        file.Close();
    }
    public void CallLoad(int fileNum){

        BinaryFormatter bf = new BinaryFormatter();
        FileInfo fileCheck = new FileInfo(Application.persistentDataPath + dataDirectory +"/SaveFile" + fileNum.ToString() +".dat");

        if(fileCheck.Exists){
            FileStream file = File.Open(Application.persistentDataPath + dataDirectory +"/SaveFile" + fileNum.ToString() +".dat", FileMode.Open);
        
            if(file != null && file.Length >0){

                curData =(Data)bf.Deserialize(file);

                curData.curPlayCount ++;

                if(curData.getItemOverList == null) curData.getItemOverList = new List<int>();
                //if(curData.getDirtBundleOverList == null) curData.getDirtBundleOverList = new List<DirtBundleInfo>();
                if(curData.dirtBundleInfoList == null) curData.dirtBundleInfoList = new List<DirtBundleInfo>();
                
                //curData.honeyOverList.Add(-1);
            }
            
            file.Close();
        }

    }

#endregion  
    
    public bool CheckSaveFile(int fileNum){
        FileInfo fileCheck = new FileInfo(Application.persistentDataPath + dataDirectory +"/SaveFile" + fileNum.ToString() +".dat");

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

        FileStream file = File.Open(Application.persistentDataPath + dataDirectory +"/SaveFile" + fileNum.ToString() +".dat", FileMode.Open);

        var data = (Data)bf.Deserialize(file);

        file.Close();
        
        return data;
    }
    public void DeleteSaveFile(int fileNum){

        BinaryFormatter bf = new BinaryFormatter();
        FileInfo fileCheck = new FileInfo(Application.persistentDataPath + dataDirectory +"/SaveFile" + fileNum.ToString() +".dat");

        if(fileCheck.Exists){
            File.Delete(Application.persistentDataPath + dataDirectory +"/SaveFile" + fileNum.ToString() +".dat");

            Debug.Log(fileNum + "번 파일 제거 성공");
            //MenuManager.instance.ResetLoadSlots();

            //file.Close();
        }
        else{
            Debug.Log(fileNum + "번 파일 제거 불가 : 파일 없음");
        }
    }
    public void ResetAllData(){
        for(int i=0;i<20;i++){
            DeleteSaveFile(i);
        }

        ResetLocalData();
        
    }

#region LocalData
    public void CallLocalDataSave(){

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + dataDirectory +"/LocalDataFile.dat");//nickName!="" ? File.Create(Application.persistentDataPath + "/SaveFile_"+nickName+".dat"): 
        

        //Debug.Log(Application.persistentDataPath);
        bf.Serialize(file, localData);
        file.Close();
    }
    public void CallLocalDataLoad()
    {

        localData.endingCollectionOverList.Clear();
        localData.antCollectionOverList.Clear();
        localData.itemCollectionOverList.Clear();

        BinaryFormatter bf = new BinaryFormatter();
        FileInfo fileCheck = new FileInfo(Application.persistentDataPath + dataDirectory + "/LocalDataFile.dat");

        if (fileCheck.Exists)
        {
            FileStream file = File.Open(Application.persistentDataPath + dataDirectory + "/LocalDataFile.dat", FileMode.Open);

            if (file != null && file.Length > 0)
            {
                localData = (LocalData)bf.Deserialize(file);
            }

            if (localData.antCollectionOverList == null)
                localData.antCollectionOverList = new List<ClearedAntCollection>();

            if (localData.itemCollectionOverList == null)
                localData.itemCollectionOverList = new List<int>();

            // if(localData.jumpKey==KeyCode.None){
            //     localData.jumpKey = KeyCode.Space;
            // }
            // if(localData.interactKey==KeyCode.None){
            //     localData.interactKey = KeyCode.E;
            // }
            //language = localData.language;

            file.Close();
        }

    }
    public void ResetLocalData(){

        BinaryFormatter bf = new BinaryFormatter();
        FileInfo fileCheck = new FileInfo(Application.persistentDataPath + dataDirectory +"/LocalDataFile.dat");

        if(fileCheck.Exists){
            File.Delete(Application.persistentDataPath + dataDirectory +"/LocalDataFile.dat");
            localData.endingCollectionOverList.Clear();
            localData.antCollectionOverList.Clear();
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
    public bool CheckNotHaveItems(int trigNum, int[] haveItemNums, bool printDebug = true){
        List<int> tempList = new List<int>();

        for(int i=0;i<haveItemNums.Length;i++){
            if(InventoryManager.instance.CheckHaveItem(haveItemNums[i])){
                if(printDebug) Debug.Log(string.Format("TrigNum.{0} 실행 실패 : itemID.{1}({2}) 보유",trigNum, haveItemNums[i], DBManager.instance.cache_ItemDataList[haveItemNums[i]].name));

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
            DM("EndingCollectionOver : "+collectionNum);
        }
    }
    //엔딩 컬렉션 달성되었는지 확인
    public int GetClearedEndingCollectionID(int collectionNum){
        if(localData.endingCollectionOverList.Count == 0){
            return -1;
        }
        return localData.endingCollectionOverList.FindIndex(x => x.ID == collectionNum);
    }
    public void ResetEndingCollection(){
        localData.endingCollectionOverList.Clear();
        CallLocalDataSave();
    }
#endregion

#region AntCollection

    //엔딩 컬렉션 달성 등록 ( ID는 sysmsg 문서 내 301번 부터, -300값으로 적용 )
    public void AntCollectionOver(int collectionNum){
        if(GetClearedAntCollectionIndex(collectionNum) == -1){
            localData.antCollectionOverList.Add(new ClearedAntCollection(collectionNum,DBManager.instance.curData.curPlayDate.Substring(0,10),DBManager.instance.curData.curPlayCount));
            UIManager.instance.hud_sub_collection_redDot.SetActive(true);
            
            
            CallLocalDataSave();
            DM("AntCollectionOver : "+collectionNum);
        }
    }
    //엔딩 컬렉션 달성되었는지 확인
    public int GetClearedAntCollectionIndex(int collectionNum){
        if(localData.antCollectionOverList.Count == 0){
            return -1;
        }
        return localData.antCollectionOverList.FindIndex(x => x.ID == collectionNum);
    }
    public void ResetAntCollection(){
        localData.antCollectionOverList.Clear();
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
        dataDirectory = "/" + buildVersion + "_" + buildNum;
#elif alpha
        buildVersion = "alpha";
        //dataDirectory = "/" + buildVersion + "_" + buildNum;
        dataDirectory = "";
#else
        buildVersion = "";
#endif


        if (!Directory.Exists(Application.persistentDataPath + dataDirectory))
        {
            Directory.CreateDirectory(Application.persistentDataPath + dataDirectory);
        }


        SaveDefaultData();
        CallLocalDataLoad();

        //ApplyNewLanguage();
        ApplyItemInfo();
        ApplyCollectionInfo();
        ApplyCouponInfo();
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
        SteamUserStats.RequestCurrentStats();
        SteamUserStats.RequestGlobalStats(60);
#if !alpha

        if(!File.Exists(Application.persistentDataPath + dataDirectory +"/LocalDataFile.dat")){
            Debug.Log("no");
            //게임 최초 실행 확인 token : fs
            SteamUserStats.GetStat("fs",out int fs);
            SteamUserStats.SetStat("fs",fs + 1);
            SteamUserStats.StoreStats();

        }
        
#endif
    }
    void Update(){
        //System.DateTime dateTime = System.DateTime.Now;
        curData.curPlayDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }

    void FixedUpdate(){
        curData.curPlayTime += Time.fixedDeltaTime;
        

        if(PlayerManager.instance != null){

            if(PlayerManager.instance.isMoving && !PlayerManager.instance.isActing && !PlayerManager.instance.isInvincible && !PlayerManager.instance.isPlayingMinigame){
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
                ,a[i]["resourceID"].ToString()
                ,bool.Parse(a[i]["isStack"].ToString())
                ,int.Parse(a[i]["price"].ToString())
                ,a[i]["goldResourceID"].ToString()
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
    void ApplyCouponInfo(){
        //var a = TextLoader.instance.dictionaryItemText;
        var a = CSVReader.instance.data_coupon;
        cache_couponList.Clear();
        for(int i=0; i<a.Count; i++){
            //itemDataList.Add(new Item(a[i].ID,a[i].name_kr,a[i].desc_kr,a[i].type,a[i].resourceID,a[i].isStack));
            cache_couponList.Add(new Coupon(
                a[i]["code"].ToString()
                ,int.Parse(a[i]["type"].ToString())                
                ,int.Parse(a[i]["rewardItemID"].ToString())
            ));
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
    public string resourceID;
    public bool isStack;
    public Sprite icon;
    public int price;    
    public string goldResourceID;
    public Sprite goldIcon;
    //public bool isStack;

    public enum ItemType{
        equip,
    }
    public Item(int a, string b, string c, byte d, string _resourceID, bool f, int g, string _goldResourceID){
        ID = a;
        name = b;
        description = c;
        type = d;
        resourceID = _resourceID;
        icon = ResourceManager.instance.GetItemSprite(resourceID);//DBManager.instance.itemSprites[resourceID];
        isStack = f;
        price = g;
        goldResourceID = _goldResourceID;
        goldIcon = ResourceManager.instance.GetItemSprite(goldResourceID);//DBManager.instance.itemSprites[goldResourceID];
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
    public bool isRecognized;

    public ClearedEndingCollection(int _ID, string _clearedDate, int _clearedPlayCount){
        ID = _ID;
        clearedDate = _clearedDate;
        clearedPlayCount = _clearedPlayCount;
    }
}
[System.Serializable]
public class ClearedAntCollection{
    public int ID;
    public string clearedDate;
    public int clearedPlayCount;
    public bool isRecognized;

    public ClearedAntCollection(int _ID, string _clearedDate, int _clearedPlayCount){
        ID = _ID;
        clearedDate = _clearedDate;
        clearedPlayCount = _clearedPlayCount;
    }
}

[System.Serializable]
public class ClearedItemCollection{
    public int itemID;
    public bool isRecognized;

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
    public int endingCollectionNum; //data_collection.csv의 ID
    public int endingNum; //찐엔딩 넘버(임의의 값)
    public string name;
    public string preSoundFileName;
    public string postSoundFileName;
    public string bgm;
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
    //public bool noSprite;
    //[TextArea(2,4)]
    public string soundFileName;
    public int soundOrder;
    public string descriptions;
}
[System.Serializable]
public class Coupon{
    public string code;
    public int type;
    public int rewardItemID;
    public Coupon(string _code, int _type, int _rewardItemID){
        code = _code;
        type = _type;
        rewardItemID = _rewardItemID;

    }
}