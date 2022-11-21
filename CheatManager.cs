

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 
#if !DISABLESTEAMWORKS
using Steamworks;
#endif
public class CheatManager : MonoBehaviour
{
    public static CheatManager instance;
    public GameObject cheatPanel;
    public InputField cheat;
    public Transform checkPoint;
    public Transform checkPoint_Special;
    public Transform minigameParent;
    
    public int cmdPage;
    public int cmdMaxPage;
    public GameObject cheatResultObj;
    GUIStyle style_des, style_cmd,style_cmdResult, style0, style1;
    Rect rect_des, rect_cmd,rect_cmdResult, rect0, rect1;
    string COMMAND_KEY = "key";
    string COMMAND_ARGUMENTS = "arguments";
    string COMMAND_CHEATDESCRIPTION = "cheatDescription";
    string ITEM_ID = "ID";
    string ITEM_NAME = "name_kr";
    string MAP_ID = "ID";
    string MAP_NAME = "text_kr";
    string cmdListString;
    string itemListString;
    string mapListString;
    string endingListString;
    string antListString;
    string questListString;
    string trueEndingListString;
#if UNITY_EDITOR || alpha
    void Awake(){
        instance = this;
    }
    void Start()

    {
        cmdMaxPage = 2;

        cheat.onEndEdit.AddListener(delegate { GetCheat(); });

        int w = Screen.width, h = Screen.height;


        style_cmdResult = new GUIStyle();
        style_cmdResult.alignment = TextAnchor.LowerLeft;
        style_cmdResult.fontSize = h * 2 / 100;
        style_cmdResult.normal.textColor = Color.red;
        rect_cmdResult = new Rect(0, h - style_cmdResult.fontSize, w, style_cmdResult.fontSize);
        
        style_des = new GUIStyle();
        style_des.alignment = TextAnchor.MiddleCenter;
        style_des.fontSize = (int)(h * 0.025);
        style_des.normal.textColor = Color.white;
        style_des.fontStyle = FontStyle.Bold;
        rect_des = new Rect(0, h- style_des.fontSize, w, style_des.fontSize);

        style_cmd = new GUIStyle();
        style_cmd.alignment = TextAnchor.UpperLeft;
        style_cmd.fontSize = (int)(h * 0.02);
        style_cmd.normal.textColor = Color.cyan;
        style_cmd.fontStyle = FontStyle.Bold;
        rect_cmd = new Rect(20, 20, 0, style_cmd.fontSize);
        
        style0 = new GUIStyle();
        style0.alignment = TextAnchor.UpperLeft;
        style0.fontSize = (int)(h * 0.02);
        style0.normal.textColor = Color.cyan;
        style0.fontStyle = FontStyle.Bold;
        rect0 = new Rect(w * 0.33f, 0, 0, style0.fontSize);

        style1 = new GUIStyle();
        style1.alignment = TextAnchor.UpperLeft;
        style1.fontSize = (int)(h * 0.02);
        style1.normal.textColor = Color.cyan;
        style1.fontStyle = FontStyle.Bold;
        rect1 = new Rect(w * 0.66f, 0, w, style1.fontSize);

        int i = 0;
        cmdListString = "<color=#FFFF4D>[COMMAND] 명령어 입력 후 엔터</color>\n";
        foreach(var a in CSVReader.instance.data_command){

            string[] _arg = a[COMMAND_ARGUMENTS].ToString().Split(';');;
            string key = string.Format(a[COMMAND_KEY].ToString(), _arg);
            cmdListString += "<color=white>" + key + "</color> : " + a[COMMAND_CHEATDESCRIPTION];// + "\t";
            if(++i%1==0) cmdListString += "\n";
        }
        
        i = 0;

        itemListString = "<color=orange>[ITEM]</color>\n";
        foreach(var a in CSVReader.instance.data_item){
            itemListString += "<color=white>"+ a[ITEM_ID] + "</color> " + a[ITEM_NAME] + "\t";
            if(++i%2==0) itemListString += "\n";
        }
        
        i = 0;
        mapListString = "<color=#FF90EE90>[맵] : t [번호]</color>\n";
        foreach(var a in CSVReader.instance.data_map){
            int mapNum = (int)a[MAP_ID]*2;
            mapListString += "<color=white>" + mapNum + "</color>/" + "<color=white>" + (mapNum + 1) + "</color> " + a[MAP_NAME] + "\t";
            if(++i%2==0) mapListString += "\n";
        }
        i = 0;
        endingListString = "<color=#FF90EE90>[엔딩]</color>\n";
        foreach(var a in CSVReader.instance.data_collection){
            //int mapNum = (int)a[MAP_ID]*2;
            endingListString += "<color=white>" + a["ID"] + "</color> " + a["name_kr"];
            if(++i%1==0) endingListString += "\n";
        }
        i = 0;
        trueEndingListString = "<color=#FF90EE90>[진엔딩]</color>\n";
        foreach(var a in CSVReader.instance.data_collection){
            int index = (int)a["trueID"];
            if(index==0) continue;
            trueEndingListString += "<color=white>" + a["trueID"] + "</color> " + a["name_kr"];
            if(++i%1==0) trueEndingListString += "\n";
        }
    }
    public void InputCheat(string inputCheat){
        cheat.text = inputCheat;
        GetCheat();
        cheat.text = "";
    }

    public void GetCheat()

    {

        if (cheat.text.Length > 0)

        {
           // DebugManager.instance.PrintDebug(cheat.text);

            string[] temp = cheat.text.Split('\x020');
            //Debug.Log(temp.Length);
            //int x = 0;
            // if(temp.Length == 1){
            //     return;
            // }
            
            switch(temp[0]){
                case "t":
                    if(temp[1]!=null){
                        if(checkPoint.childCount>int.Parse(temp[1])){
                            
                            PlayerManager.instance.transform.position = checkPoint.GetChild(int.Parse(temp[1])).position;
                            SceneController.instance.SetConfiner(int.Parse(temp[1])/2);
                            SoundManager.instance.SetBgmByMapNum(int.Parse(temp[1])/2);
                            DM("Activate teleport to location "+temp[1].ToString());
                        }   
                        else{
                            Debug.LogError("Error : Empty location "+temp[1].ToString());

                        }
                    }
                    break;

                case "additem":
                    if(temp[1]!=""){

                        if(temp[1]=="all"){
                            //for(int i=0;i<TextLoader.instance.dictionaryItemText.Count;i++){
                            for(int i=0;i<CSVReader.instance.itemAmount;i++){
                                InventoryManager.instance.AddItem(i,1);
                            }
                            break;
                        }
                        int itemID = int.Parse(temp[1]);

                        if(temp[2]!=""){

                            if(itemID <CSVReader.instance.itemAmount)
                                InventoryManager.instance.AddItem(int.Parse(temp[1]), int.Parse(temp[2]));
                        }
                        else{

                            //int itemID = int.Parse(temp[1]);
                            if(itemID <CSVReader.instance.itemAmount)
                                InventoryManager.instance.AddItem(int.Parse(temp[1]));
                        }

                    }
                    break;
                case "g":
                    if(temp[1]!=null && temp[1]!="2"){
                        int minigameNum = int.Parse(temp[1]);
                        if(minigameNum < minigameParent.childCount && !PlayerManager.instance.isPlayingMinigame){
                            
                            MinigameManager.instance.StartMinigame(minigameNum);
                            // PlayerManager.instance.isPlayingMinigame = true;
                            // var tempGame = minigameParent.GetChild(minigameNum).gameObject;
                            // tempGame.SetActive(!tempGame.activeSelf);

                        }

                        // if(checkPoint.GetChild(int.Parse(temp[1]))!=null){
                            
                        //     PlayerManager.instance.transform.position = checkPoint.GetChild(int.Parse(temp[1])).position;
                        //     SceneController.instance.SetConfiner(int.Parse(temp[1])/2);
                        //     DM("Activate teleport to location "+temp[1].ToString());
                        // }   
                        // else{
                        //     DM("Error : Empty location "+temp[1].ToString());

                        // }
                    }
                    else if(temp[1]=="2"){
                        PlayerManager.instance.MovePlayer(checkPoint_Special.GetChild(0).transform);
                        SceneController.instance.SetSomeConfiner(minigameParent.GetChild(2).GetComponent<Minigame2Script>().mapCollider);
                        MinigameManager.instance.nowMinigameNum = 2;
                        
                            PlayerManager.instance.isPlayingMinigame = true;
                            var tempGame = minigameParent.GetChild(2).gameObject;
                            tempGame.SetActive(!tempGame.activeSelf);

                    }
                    break;
                case "ec":
                    if(temp[1]!=null){
                        if(temp[1]=="all"){
                            for(int i=0;i<DBManager.instance.cache_EndingCollectionDataList.Count;i++){
                                DBManager.instance.EndingCollectionOver(i);
                            }
                        }
                        else{
                            int collectionID = int.Parse(temp[1]);
                            if(collectionID <DBManager.instance.cache_EndingCollectionDataList.Count){
                                Debug.Log("{0}번 컬렉션 획득");
                                DBManager.instance.EndingCollectionOver(int.Parse(temp[1]));
                            }
                            else{
                                Debug.Log("해당 컬렉션 없음");
                            }
                        }

                        MenuManager.instance.ResetCardOrder();

                    }
                    break;

                case "setdirt":
                    if(temp[1]!=null){
                        int chargePoint = int.Parse(temp[1]);
                        if(chargePoint>0){
                            if(chargePoint>=100) DBManager.instance.curData.curDirtAmount = DBManager.instance.maxDirtAmount;
                            else DBManager.instance.curData.curDirtAmount = DBManager.instance.maxDirtAmount * chargePoint * 0.01f;
                        }
                        MenuManager.instance.ResetCardOrder();
                    }
                    break;

                case "deletesavefile":
                    if(temp[1]!=null){

                            int fileNum = int.Parse(temp[1]);
                            if(fileNum>=0){
                                DBManager.instance.DeleteSaveFile(fileNum);
                            }

                    }
                    break;

                case "checkhaveitem" :

                    if(temp[1]==null) return;

                    int getInt = int.Parse(temp[1]);

                    //Debug.Log(InventoryManager.instance.CheckHaveItem(getInt));
                    
                    DM("Check whether you have the itemID : " + getInt.ToString() + " : "+ InventoryManager.instance.CheckHaveItem(getInt).ToString());
                    break;


                case "invincible" :

                    if(temp[1]==null) return;

                    //int getInt = int.Parse(temp[1]);
                    Debug.Log(temp[1]);

                    if(temp[1]=="on"){
                        PlayerManager.instance.isInvincible = true;
                        DM("Soil is not consumed anymore");
                    }
                    else{
                        PlayerManager.instance.isInvincible = false;
                        DM("Soil is consumed again");

                    }

                    break;

                case "completetrigger" :

                    if(temp[1]==null) return;

                   
                    DBManager.instance.TrigOver(int.Parse(temp[1]));
                    DM("Trigger Number "+temp[1]+" has been completed.");

                    break;

                // case "openshop" :

                //     //if(temp[1]==""||temp[2]==""||temp[3]=="") return;
                //     if(temp[1]=="") return;

                    
                //     string[] tempList = temp[1].Split(',');
                //     int[] itemList = Array.ConvertAll(tempList, s => int.Parse(s));
                //     ShopSales[] salesList = new ShopSales[itemList.Length]{};
                //     for(int i=0;i<itemList.Length;i++){
                //         salesList.Add
                //     }

                //     ShopManager.instance.OpenShopUI(1,"test",itemList);
                   
                //     // DBManager.instance.TrigOver(int.Parse(temp[1]));
                //     DM("Openshop itemList : "+ String.Join(",", itemList));

                //     break;
                case "addhoney" :

                    if(temp[1]=="") return;

                    DBManager.instance.curData.curHoneyAmount += int.Parse(temp[1]);

                    //DM("Openshop itemList : "+ String.Join(",", itemList));

                    break;
                case "gameend" :
                case "ge" :

                    if(temp[1]=="") return;

                    UIManager.instance.SetGameEndUI(int.Parse(temp[1]));
                    break;
                case "gameover" :

                    if(temp[1]=="") return;

                    UIManager.instance.SetGameOverUI(int.Parse(temp[1]));
                    break;
                case "deletelocal" :

                    //if(temp[1]=="") return;
                    DBManager.instance.ResetLocalData();
                    break;
                    
                case "resetendingcollection":
                    DBManager.instance.ResetEndingCollection();
                    MenuManager.instance.ResetCardOrder();
                    DM("resetendingcollection");

                    break;
                    
                case "additems":
                    for(int i=1;i<temp.Length;i++){
                        InventoryManager.instance.AddItem(int.Parse(temp[i]),1);
                    }

                    break;
                case "cts":
                    for(int i=1;i<temp.Length;i++){
                            
                        DBManager.instance.TrigOver(int.Parse(temp[i]));
                        DM("Trigger Number "+temp[i]+" has been completed.");
                    }

                    break;
                    
                case "super":
                    InventoryManager.instance.AddHoney(100000);
                    InventoryManager.instance.AddDirt(10000);
                    InventoryManager.instance.AddItem(5, 100);
                    InputCheat("additems 2 21 26 18 24 25");
                    break;
                case "removeitem":

                    if(temp[1]!=""){

                        // if(temp[1]=="all"){
                        //     //for(int i=0;i<TextLoader.instance.dictionaryItemText.Count;i++){
                        //     for(int i=0;i<CSVReader.instance.itemAmount;i++){
                        //         InventoryManager.instance.AddItem(i,1);
                        //     }
                        //     break;
                        // }
                        int itemID = int.Parse(temp[1]);

                        if(temp[2]!=""){

                            int itemAmount = int.Parse(temp[2]);
                            if(itemID <CSVReader.instance.itemAmount)
                                InventoryManager.instance.RemoveItem(itemID,itemAmount);
                                //InventoryManager.instance.AddItem(int.Parse(temp[1]), int.Parse(temp[2]));
                        }
                        else{

                            //int itemID = int.Parse(temp[1]);
                            //if(itemID <CSVReader.instance.itemAmount)
                            //    InventoryManager.instance.AddItem(int.Parse(temp[1]));
                        }

                    }
                    break;
                case "load":

                    if(temp[1]!=""){
                        int loadDataNum = int.Parse(temp[1]);
                        MenuManager.instance.Load(loadDataNum);
                    }
                    break;
                case "save":

                    if(temp[1]!=""){
                        int saveDataNum = int.Parse(temp[1]);
                        MenuManager.instance.Save(saveDataNum);
                    }
                    break;

                case "cleanupinventory":
                    InventoryManager.instance.CleanUpInventory();
                    break;
                case "cleanuptrigger":
                    DBManager.instance.curData.trigOverList.Clear();
                    break;
                
                case "resetantcollection":
                    DBManager.instance.ResetAntCollection();
                    MenuManager.instance.ResetAntCollectionUI();
                    DM("resetantcollection");

                    break;
                case "ac":
                    //DBManager.instance.ResetAntCollection();
                    for(int i=0;i<MenuManager.instance.antStickersMother.childCount;i++){
                        DBManager.instance.AntCollectionOver(i+1);
                    }
                    //MenuManager.instance.RefreshAntCollectionUI();
                    MenuManager.instance.ResetAntCollectionUI();
                    
                    DM("complete all ant collection");

                    break;
                    
                case "set" :

                    if(temp[1]=="") return;
                    switch(temp[1]){
                        case "0" :
                            InventoryManager.instance.AddItem(5,100);
                            InventoryManager.instance.AddItem(2,1);
                            InventoryManager.instance.AddItem(21,1);
                            InventoryManager.instance.AddItem(24,1);
                            InventoryManager.instance.AddItem(3,1);
                            InventoryManager.instance.AddItem(16,1);
                            InventoryManager.instance.AddItem(19,1);
                            break;
                    }
                    break;
                    
                case "acceptquest" :
                case "aq" :

                    if(temp[1]=="") return;

                    if(temp[1]=="all"){
                        for (int i = 0; i < DBManager.instance.cache_questList.Count;i++){
                            UIManager.instance.AcceptQuest(i);
                        }
                    }
                    else{
                        UIManager.instance.AcceptQuest(int.Parse(temp[1]));
                    }


                    break;

                case "completequest" :
                case "cq" :

                    if(temp[1]=="") return;

                    if(temp[1]=="all"){
                        for (int i = 0; i < DBManager.instance.cache_questList.Count;i++){
                            UIManager.instance.CompleteQuest(i);
                        }
                    }
                    else{
                        UIManager.instance.CompleteQuest(int.Parse(temp[1]));
                    }

                    break;

                case "resetprologue" :
                    DBManager.instance.localData.canSkipPrologue = false;
                    break;
                    
#region steam stats
#if !DISABLESTEAMWORKS
                    
                case "getstat" :

                    if(temp[1]=="") return;

                    SteamUserStats.RequestGlobalStats(60);
                    SteamUserStats.GetGlobalStat(temp[1], out long stat);
                    Debug.Log(temp[1] + " : " + stat);
                    
                    break;

                case "getcp" :

                    if(temp[1]=="") return;

                    SteamUserStats.RequestGlobalStats(60);

                    var tempCpCount = 0;
                    for(int i=int.Parse(temp[1]);i<int.Parse(temp[2]);i++){

                        SteamUserStats.GetGlobalStat("cp"+i.ToString(), out long stata);
                        tempCpCount = tempCpCount + (int)stata;
                    }
                    Debug.Log(string.Format("total cp {0}~{1} : {2}",temp[1],temp[2],tempCpCount));// temp[1] + " : " + stat);
                    
                    break;

                case "resetsteamach" :

                    for(int i=0;i<20;i++){
                        SteamUserStats.ClearAchievement("ach"+i.ToString());
                        Debug.Log("resetsteamach all");
                    }
                    break;

                case "setsteamach" :
                
                    if(temp[1]=="") return;
                    Debug.Log("Try to set achievement... : "+temp[1]);

                    SteamAchievement.instance.ApplyAchievements(int.Parse(temp[1]));
                    
                    
                    break;

#endif
#endregion
            }
//EndingCollectionOver
//DeleteSaveFile
//SetGameEndUI
            for(var a=0;a<temp.Length;a++){
                temp[a] = "";
            }
            cheat.text = "";
            //DebugManager.instance.cheatPanel.SetActive(false);

        }

    }



    public void DM(string msg){
        DebugManager.instance.PrintDebug(msg);
        cheatResultObj.SetActive(false);
        cheatResultObj.SetActive(true);
    } 


    
    void Update(){
        if(Input.GetKeyDown(KeyCode.Tab)){
            if(++cmdPage==cmdMaxPage){
                cmdPage = 0;
            }
        }
        if(Input.GetKeyDown(KeyCode.Return)){
            CheatManager.instance.cheatPanel.SetActive(!CheatManager.instance.cheatPanel.activeSelf);
            //if(PlayerManager.instance.canMove) PlayerManager.instance.canMove = !cheatPanel.activeSelf;
            CheatManager.instance.cheat.Select();
            CheatManager.instance.cheat.ActivateInputField();
            
        }
        if(Input.GetKeyDown(KeyCode.F10)){
            //SceneManager.LoadScene("warehouse");
            PlayerManager.instance.RevivePlayer();
            CheatManager.instance.InputCheat("t 0");
            CheatManager.instance.InputCheat("completetrigger 1");
            //ResetPlayerPos();
        }
    }

    void OnGUI(){
        if(cheatPanel.gameObject.activeSelf){
            GUI.backgroundColor = Color.black;

            string column0 = "";
            string column1 = "";

            GUI.Label(rect_des, "페이지 넘기기 : Tab", style_des); 
            GUI.Label(rect_cmd, cmdListString, style_cmd); 

            switch(cmdPage){
                case 0 :
                    column0 = itemListString;
                    column1 = mapListString;
                    break;
                case 1 :
                    column0 = endingListString;
                    column1 = trueEndingListString;
                    break;
            }
            GUI.Label(rect0, column0, style0); 
            GUI.Label(rect1, column1, style1); 
        }
        
    }
#endif
}