

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 
using Steamworks;
public class CheatManager : MonoBehaviour
{
    public static CheatManager instance;
    public InputField cheat;
    public Transform checkPoint;
    public Transform checkPoint_Special;
    public Transform minigameParent;
    
#if UNITY_EDITOR || alpha
    void Awake(){
        instance = this;
    }
    void Start()

    {
        cheat.onEndEdit.AddListener(delegate { GetCheat(); });
        

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
                    MenuManager.instance.RefreshAntCollectionUI();
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

                    
                case "getstat" :

                    if(temp[1]=="") return;

                    SteamUserStats.RequestGlobalStats(60);
                    SteamUserStats.GetGlobalStat(temp[1], out long stat);
                    Debug.Log(temp[1] + " : " + stat);
                    
                    break;
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



    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);

    // public void GetCheat22(){
    //     string[] temp = cheat.text.Split('\x020');
    //     DebugManager.instance.PrintDebug(temp);
        
    //     DebugManager.instance.PrintDebug(temp[0]);
    //     switch(temp[0]){
    //         case "teleport":
    //             switch(temp[1]){
    //                 case "0" :
    //                     DebugManager.instance.PrintDebug("0번이동");
    //                     break;
    //                 case "1" :
    //                     DebugManager.instance.PrintDebug("1번이동");
    //                     break;
    //             }
    //             break;
    //     }
    // }
    
    void Update(){
        if(DebugManager.instance.isDebugMode){
            if(Input.GetKeyDown(KeyCode.Return)){
                DebugManager.instance.cheatPanel.SetActive(!DebugManager.instance.cheatPanel.activeSelf);
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
    }
#endif
}