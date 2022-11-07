using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
#if !DISABLESTEAMWORKS
using Steamworks;
#endif
public class DBManager : MonoBehaviour
{
    public static DBManager instance;
    [Header("[Game Info]━━━━━━━━━━━━━━━━━━━━━━━━━━━")]
    //public uint buildNum;
    //public uint buildSubNum;
    //public string buildDate;
    //public string buildVersion;
    public string language;
    string dataDirectory;
    [Header("[Game]")]
    
    [Header("[Game Settings]━━━━━━━━━━━━━━━━━━━━━━━━━━━")]
    public float maxDirtAmount = 100;
    public float dirtAmountPaneltyPerSeconds = 0.01f;
    public float dirtAlertAmount = 0.1f;//흙 부족 경고 시작 수치(%)
    public float defaultGetDirtAmount = 5;//흙아이템 충전량
    public float minimumDirtAmount = 15;//흙 고갈 사망 후 최초 지급량
    [Header("[Dialogue]")]
    public float waitTime_dialogueInterval;
    public float waitTime_dialogueTypingInterval;
    [Header("[Sound]")]
    public float bgmAdjustVal;
    public float sfxAdjustVal;
    public float bgmFadeValueInTrigger;
    public string bgmName0;
    public string bgmName1;
    public string bgmName2;
    [Header("[Steam]")]
    public bool achievementIsAvailable;
    [Header("[Contents On/Off]")]
    public bool dirtOnlyHUD;
    public bool activateHunter;
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
    public List<EndingCollection> cache_EndingCollectionDataList;//data_collection : sortOrder에 의해 정렬된 캐시
    public List<GameEndList> cache_GameEndDataList;
    public List<Coupon> cache_couponList;
    public List<QuestInfo> cache_questList;
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
        public int curDirtItemCount;






        [Space]
        public List<ItemList> itemList;// = new List<ItemList>();  //현재 보유한 아이템 ID 저장
        public List<int> trigOverList;
        public List<int> getItemOverList; // 맵 내 아이템 활성화/비활성화 체크 용도
        //public List<DirtBundleInfo> getDirtBundleOverList;
        public List<DirtBundleInfo> dirtBundleInfoList;
        public List<int> mapOverList;
        public List<QuestState> questStateList;//221008
        //public Dictionary<int,object> questStateDic;//221008
        [Header("Contents ━━━━━━━━━━━")]
        public int roulettePlayCount;


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
        
        [Header("Collections ━━━━━━━━━━━")]
        public List<ClearedEndingCollection> endingCollectionOverList = new List<ClearedEndingCollection>();
        public List<ClearedAntCollection> antCollectionOverList = new List<ClearedAntCollection>();
        public List<int> itemCollectionOverList = new List<int>();
        
        [Header("Options ━━━━━━━━━━━")]
        public float bgmVolume;
        public float sfxVolume;
        public bool onTalkingSound;
        public int languageValue;
        public bool isWindowedMode;
        public int resolutionValue;
        public int frameRateValue;
        public KeyCode jumpKey;
        public KeyCode interactKey;
        public KeyCode AddDirtKey;
        public KeyCode petKey;
        public int usedCouponRewardItemID;
        
        [Space]
        [Header("Steam Achievements ━━━━━━━━━━━")]
        //업적용
        public int useDirtCount;
        public int deathCount;//일반 죽음
        public int deathCount_noDirt;//흙고갈 죽음
        public int gameCount_success_0;//미니게임0 성공횟수(종이오리기)
        public List<int> itemPurchaseList;
    }
    
#region Non local data
    public void SaveDefaultData(){

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + dataDirectory +"/SaveFileDefault.dat");//nickName!="" ? File.Create(Application.persistentDataPath + "/SaveFile_"+nickName+".dat"): 

//        Debug.Log(Application.persistentDataPath);
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
//            Debug.Log("dirtbundle info saved completely");
        }

        Debug.Log(Application.persistentDataPath);
        bf.Serialize(file, curData);
        file.Close();
    }
    public bool CheckFileExist(int fileNum){
        FileInfo fileCheck = new FileInfo(Application.persistentDataPath + dataDirectory +"/SaveFile" + fileNum.ToString() +".dat");
        if(fileCheck.Exists){
            return true;
        }
        else{
            return false;
        }
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
                if(curData.mapOverList == null) curData.mapOverList = new List<int>();
                if(curData.questStateList == null) curData.questStateList = new List<QuestState>();
                
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
        localData.itemPurchaseList.Clear();

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

            if(localData.itemPurchaseList == null) localData.itemPurchaseList = new List<int>();

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
            localData.itemCollectionOverList.Clear();
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
            
            //스팀업적 0 체크용
            Debug.Log("A : " + DBManager.instance.cache_EndingCollectionDataList.Count);
            if(DBManager.instance.localData.endingCollectionOverList.Count == DBManager.instance.cache_EndingCollectionDataList.Count){
                SteamAchievement.instance.ApplyAchievements(0);
            }
            
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
            
            if(DBManager.instance.localData.antCollectionOverList.Count == MenuManager.instance.antStickersMother.childCount){
                SteamAchievement.instance.ApplyAchievements(14);
            }
            
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
    
#region ItemCollection

    //엔딩 컬렉션 달성 등록 ( ID는 sysmsg 문서 내 301번 부터, -300값으로 적용 )
    public void ItemCollectionOver(int collectionNum){
        if(GetClearedItemCollectionIndex(collectionNum) == -1){
            //localData.itemCollectionOverList.Add(new ClearedItemCollection(collectionNum,DBManager.instance.curData.curPlayDate.Substring(0,10),DBManager.instance.curData.curPlayCount));
            localData.itemCollectionOverList.Add(collectionNum);
            UIManager.instance.hud_sub_collection_redDot.SetActive(true);
            
            // if(DBManager.instance.localData.antCollectionOverList.Count == MenuManager.instance.antStickersMother.childCount){
            //     SteamAchievement.instance.ApplyAchievements(14);
            // }
            
            CallLocalDataSave();
            DM("ItemCollectionOver : "+collectionNum);
        }
    }
    //컬렉션 달성되었는지 확인
    public int GetClearedItemCollectionIndex(int collectionNum){
        if(localData.itemCollectionOverList.Count == 0){
            return -1;
        }
        //return localData.itemCollectionOverList.FindIndex(x => x.ID == collectionNum);
        Debug.Log(localData.itemCollectionOverList.IndexOf(collectionNum));
        return localData.itemCollectionOverList.IndexOf(collectionNum);
    }
    public void ResetItemCollection(){
        localData.itemCollectionOverList.Clear();
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
        //buildVersion = "alpha";
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
        ApplyQuestInfo();
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
#if !DISABLESTEAMWORKS

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
#endif
    }
    void Update(){
        //System.DateTime dateTime = System.DateTime.Now;
        curData.curPlayDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }

    void FixedUpdate(){
        curData.curPlayTime += Time.fixedDeltaTime;
        

        if(PlayerManager.instance != null){

            if(PlayerManager.instance.isMoving 
            && !PlayerManager.instance.isActing 
            && !PlayerManager.instance.isInvincible 
            && !PlayerManager.instance.isPlayingMinigame
            ){
                if(curData.curDirtAmount>0) curData.curDirtAmount -= dirtAmountPaneltyPerSeconds;
            }
        }
    }
#region @ApplyResourceDataFile
    void ApplyItemInfo(){
        //var a = TextLoader.instance.dictionaryItemText;
        var a = CSVReader.instance.data_item;
        cache_ItemDataList.Clear();

        //speed,0.5|dirtBonus,0.1
        for(int i=0; i<a.Count; i++){
            var tempTotalItemStats = a[i]["stat"].ToString();
            List<ItemStat> curItemStatList = new List<ItemStat>();
            if(!string.IsNullOrEmpty(tempTotalItemStats)){
//                Debug.Log(i + "번 아이템 능력치 있어서 적용");
                var statSplit0 = tempTotalItemStats.Split('|');
//                Debug.Log("statSplit0" + statSplit0[0]);
                for(int j=0;j<statSplit0.Length;j++){
                    var statSplit1 = statSplit0[j].Split(',');
//                    Debug.Log("statSplit1" + statSplit1[0]);
//                    Debug.Log("statSplit1" + statSplit1[1]);
                    curItemStatList.Add(new ItemStat(statSplit1[0],float.Parse(statSplit1[1])));
                    //curItemStat.statName = statSplit1[0];
                    //curItemStat.statAmount = float.Parse(statSplit1[1]);
                }
            }

            //itemDataList.Add(new Item(a[i].ID,a[i].name_kr,a[i].desc_kr,a[i].type,a[i].resourceID,a[i].isStack));
            cache_ItemDataList.Add(new Item(
                int.Parse(a[i]["ID"].ToString())
                ,a[i]["name_"+language].ToString()
                ,a[i]["desc_"+language].ToString()
                ,byte.Parse(a[i]["type"].ToString())
                ,a[i]["resourceID"].ToString()
                ,bool.Parse(a[i]["isStack"].ToString())
                ,int.Parse(a[i]["price"].ToString())
                ,int.Parse(a[i]["goldID"].ToString())
                ,curItemStatList
            ));
        }
    }
    
    void ApplyCollectionInfo(){
        //var a = TextLoader.instance.dictionaryItemText;
        var a = CSVReader.instance.data_collection;
        cache_EndingCollectionDataList.Clear();

        // for(int x=0; x<a.Count; x++){
        //     for(int i=0; i<a.Count; i++){
        //         if(int.Parse(a[i]["sortOrder"].ToString())==x){

        //         cache_EndingCollectionDataList.Add(new EndingCollection(
        //             int.Parse(a[i]["ID"].ToString())
        //             ,a[i]["name_"+language].ToString()
        //             ,int.Parse(a[i]["resourceID"].ToString())
        //             ,int.Parse(a[i]["trueID"].ToString())
        //             ,int.Parse(a[i]["sortOrder"].ToString())
        //         ));
        //         }
        //     }
        // }

        
        for(int i=0; i<a.Count; i++){
            cache_EndingCollectionDataList.Add(new EndingCollection(
                int.Parse(a[i]["ID"].ToString())
                ,a[i]["name_"+language].ToString()
                ,int.Parse(a[i]["resourceID"].ToString())
                ,int.Parse(a[i]["trueID"].ToString())
                ,int.Parse(a[i]["sortOrder"].ToString())
            ));
        }
        
        cache_EndingCollectionDataList =
        cache_EndingCollectionDataList
        .OrderBy(x => x.sortOrder) //보상 수령 대기
        .ThenBy(x => x.ID) //진행 중
        .ToList()
        ;
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
    void ApplyQuestInfo(){
        //var a = TextLoader.instance.dictionaryItemText;
        var a = CSVReader.instance.data_quest;
        cache_questList.Clear();
        

        for(int i=0; i<a.Count; i++){
            //
            List<ItemList> curQuestRewardItemList = new List<ItemList>();
            var tempString = a[i]["rewardItems"].ToString();
            if(!string.IsNullOrEmpty(tempString)){
            //if(tempString.Contains("|")){
                var split0 = tempString.Split(';').ToList();
                for(int j=0;j<split0.Count;j++){
                    var split1 = split0[j].Split('|').ToList();
                    //Debug.Log(split1[0]);
                    //Debug.Log(split1[1]);
                    curQuestRewardItemList.Add(new ItemList(int.Parse(split1[0]),int.Parse(split1[1])));
                }
            }

            int curQuestMajorType = int.Parse(a[i]["majorType"].ToString());
            var tempObjectivesString0 = a[i]["objectives0"].ToString();
            var tempObjectivesString1 = a[i]["objectives1"].ToString();

            List<int> curObjectivesList0= new List<int>();
            List<ItemList> curObjectivesList1= new List<ItemList>();
            List<string> curObjectivesList_split0 = new List<string>();
            List<string> curObjectivesList_split1 = new List<string>();
            
            if(!string.IsNullOrEmpty(tempObjectivesString0)){ 
                curObjectivesList_split0 = tempObjectivesString0.Split(';').ToList();
            }
            if(!string.IsNullOrEmpty(tempObjectivesString1)){ 
                curObjectivesList_split1 = tempObjectivesString1.Split(';').ToList();
            }

            switch(curQuestMajorType){
                case 0 ://트리거 번호
                case 2 ://맵 번호
                case 3 ://흙더미 개수
                case 4 ://아이템
                    for(int j=0;j<curObjectivesList_split0.Count;j++){
                        curObjectivesList0.Add(int.Parse(curObjectivesList_split0[j]));
                    }
                    break;
                case 1://아이템 ID|사용횟수

                    for(int j=0;j<curObjectivesList_split1.Count;j++){
                        for(int k=0;k<curObjectivesList_split1.Count;k++){
                            var split1 = curObjectivesList_split1[k].Split('|').ToList();
                            curObjectivesList1.Add(new ItemList(int.Parse(split1[0]),int.Parse(split1[1])));
                        }
                    }
                    break;
            }


            int curTargetVal = 1;
            var tempTargetVal = a[i]["targetVal"].ToString();
            if(!string.IsNullOrEmpty(tempTargetVal)) curTargetVal = int.Parse(tempTargetVal);;

            cache_questList.Add(new QuestInfo(
                int.Parse(a[i]["ID"].ToString())
                ,a[i]["main_"+language].ToString()
                ,a[i]["iconResource"].ToString()
                ,int.Parse(a[i]["rewardHoney"].ToString())
                ,curQuestRewardItemList
                ,curObjectivesList0
                ,curObjectivesList1
                ,curTargetVal
                ,curQuestMajorType
            ));
            //curQuestRewardItemList.Clear();
        }
    }
#endregion

#region Check Map Data
///<summary>
///334
///</summary>
    public void MapOver(int mapNum){
        if(!CheckMapOver(mapNum)){
            curData.mapOverList.Add(mapNum);

            if(mapNum == 9 && curData.mapOverList.Count>=2){
                var molCount = curData.mapOverList.Count;
                if(curData.mapOverList[molCount-2]==8){
                    SteamAchievement.instance.ApplyAchievements(13);
                }
            }

            //0,1,2,7 맵 제외 모두 방문 시 업적 달성
            if(curData.mapOverList.Count >= 21/* CSVReader.instance.data_map.Count - 3 */
            &&CheckTrigOver(39)
            ){
                SteamAchievement.instance.ApplyAchievements(11);

            }
        }
    }
    public bool CheckMapOver(int mapNum){
        if(curData.mapOverList.Contains(mapNum)){
            return true;
        }
        else{
            return false;
        }
    }
#endregion

#region Check Item Purchase Data
    public void ItemPurchaseOver(int _id){
        if(!CheckItemPurchaseOver(_id)){
            localData.itemPurchaseList.Add(_id);

            // if(mapNum == 9 && curData.itemPurchaseList.Count>=2){
            //     var molCount = curData.itemPurchaseList.Count;
            //     if(curData.itemPurchaseList[molCount-2]==8){
            //         SteamAchievement.instance.ApplyAchievements(13);
            //     }
            // }

            //0,1,2 맵 제외 모두 방문 시 업적 달성
            if(localData.itemPurchaseList.Count >= 9){
                SteamAchievement.instance.ApplyAchievements(4);

            }
        }
    }
    public bool CheckItemPurchaseOver(int _id){
        if(localData.itemPurchaseList.Contains(_id)){
            return true;
        }
        else{
            return false;
        }
    }
#endregion

#region @퀘스트 체크
    public bool CheckQuestOver(int questID){
        if(curData.questStateList.Exists(x=>x.questID == questID)){
            return true;
        }
        else{
            return false;
        }
    }


#endregion
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
    public int goldID;
    public Sprite goldIcon;
    public List<ItemStat> itemStat;
    //public bool isStack;

    public enum ItemType{
        equip,
    }
    public Item(int a, string b, string c, byte d, string _resourceID, bool f, int g, int _goldID, List<ItemStat> _itemStat){
        ID = a;
        name = b;
        description = c;
        type = d;
        resourceID = _resourceID;
        icon = ResourceManager.instance.GetItemSprite(resourceID);//DBManager.instance.itemSprites[resourceID];
        isStack = f;
        price = g;
        goldID = _goldID;
        itemStat = _itemStat;
        //goldIcon = ResourceManager.instance.GetItemSprite(goldResourceID);//DBManager.instance.itemSprites[goldResourceID];
    }
}



[System.Serializable]
public class EndingCollection{
    public int ID;
    public string name;
    public string description;

    
    public int resourceID;
    public int trueID;
    public Sprite sprite;
    public int sortOrder;


    public EndingCollection(int a, string b, int c,int d,int e){
        ID = a;
        name = b;
        //description = c;
        //date = d;
        //count = e;
        resourceID = c;
        trueID = d;
        if(DBManager.instance.endingCollectionSprites.Length > resourceID)
        sprite = DBManager.instance.endingCollectionSprites[resourceID];
        sortOrder = e;
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
    public bool isRecognized;//false이면 redDot 표시 > 확인 후 true

    public ClearedAntCollection(int _ID, string _clearedDate, int _clearedPlayCount){
        ID = _ID;
        clearedDate = _clearedDate;
        clearedPlayCount = _clearedPlayCount;
    }
}

[System.Serializable]
public class ClearedItemCollection{
    public int ID;
    public string clearedDate;
    public int clearedPlayCount;

    public bool isRecognized;

    public ClearedItemCollection(int _ID, string _clearedDate = "", int _clearedPlayCount = -1){
        ID = _ID;
        clearedDate = _clearedDate != "" ? _clearedDate : DBManager.instance.curData.curPlayDate.Substring(0,10);
        clearedPlayCount = _clearedPlayCount != -1 ? _clearedPlayCount : DBManager.instance.curData.curPlayCount;
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
[System.Serializable]
public class QuestInfo{
    public int ID;
    public string mainText;
    public Sprite icon;
    public int rewardHoneyAmount;
    public List<ItemList> rewardItemList;
    public List<int> objectives0;
    public List<ItemList> objectives1;
    public int targetVal;//obj0, obj1... 이 있을 경우, 완료를 위한 목표값
    public int majorType;
    //public int rewardItemID;
    public QuestInfo(
        int _ID,
        string _mainText,
        string _iconResource,
        int _rewardHoneyAmount = 0,
        List<ItemList> _rewardItemList = null,
        List<int> _objectives0 = null,
        List<ItemList> _objectives1 = null,
        int _targetVal = 1,
        int _majorType = 0
    ){
        ID = _ID;
        mainText = _mainText;
        icon = ResourceManager.instance.GetItemSprite(_iconResource);
        rewardHoneyAmount = _rewardHoneyAmount;
        rewardItemList = _rewardItemList;
        objectives0 = _objectives0;
        objectives1 = _objectives1;
        targetVal = _targetVal;
        majorType = _majorType;
    }
}

//수락 > 완료 시 isCompleted > 보상 수령 시 gotReward
[System.Serializable]
public class QuestState{
    public int questID;
    public int progress;//진행도
    public List<int> progressList;



    public bool gotReward;
    public bool isCompleted;
    public QuestState(
        int _questID


        /*int _progress = 0,



        bool _gotReward = false, 
        bool _isCompleted = false*/
    
    ){
        questID = _questID;

        progress = 0;
        gotReward = false;
        isCompleted = false;
        progressList = new List<int>();

        // var questInfo = DBManager.instance.cache_questList.Find(x => x.ID == questID);
        // switch(questInfo.majorType){
        //     case 3 :
        //         progressList = questInfo.objectives_map;
        //         break;
        // }
    }
}