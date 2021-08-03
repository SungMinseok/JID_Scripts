

#if alpha
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 
public class CheatManager : MonoBehaviour
{
    public static CheatManager instance;
    public InputField cheat;
    public Transform checkPoint;
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
            
            switch(temp[0]){
                case "t":
                    if(temp[1]!=null){
                        if(checkPoint.GetChild(int.Parse(temp[1]))!=null){
                            
                            PlayerManager.instance.transform.position = checkPoint.GetChild(int.Parse(temp[1])).position;
                            SceneController.instance.SetConfiner(int.Parse(temp[1])/2);
                            DM("Activate teleport to location "+temp[1].ToString());
                        }   
                        else{
                            DM("Error : Empty location "+temp[1].ToString());

                        }
                    }
                    break;

                case "additem":
                    if(temp[1]!=null){
                        int itemID = int.Parse(temp[1]);
                        if(itemID < TextLoader.instance.dictionaryItemText.Count)
                            InventoryManager.instance.AddItem(int.Parse(temp[1]));
                        // if(checkPoint.GetChild(int.Parse(temp[1]))!=null){
                            
                        //     PlayerManager.instance.transform.position = checkPoint.GetChild(int.Parse(temp[1])).position;
                        //     SceneController.instance.SetConfiner(int.Parse(temp[1])/2);
                        //     DM("Activate teleport to location "+temp[1].ToString());
                        // }   
                        // else{
                        //     DM("Error : Empty location "+temp[1].ToString());

                        // }
                    }
                    break;
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
}
#endif